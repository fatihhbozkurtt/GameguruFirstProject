using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class M_Grid : MonoSingleton<M_Grid>
{
    public CellClass[,] GridPlan;
    public GameObject CellParent;
    public GameObject CellPrefab;
    public TextMeshProUGUI MatchCounterText;
    public TMP_InputField GridSizeInputField;
    public int XSize;
    public int YSize;
    public float CellXLength;
    public float CellYLength;

    private int GridSize = 0;
    private int MatchCounter = 0;

    public void Start()
    {
        GridSize = 5;
        Builder();
    }
    public void Builder()
    {
        if (GridPlan != null)
        {
            for (int x = 0; x < GridPlan.GetLength(0); x++)
            {
                for (int y = 0; y < GridPlan.GetLength(1); y++)
                {
                    Destroy(GridPlan[x, y].CellObject);
                }
            }
        }

        XSize = GridSize;
        YSize = GridSize;

        GridPlan = new CellClass[XSize, YSize];
        for (int x = 0; x < XSize; x++)
        {
            for (int y = 0; y < YSize; y++)
            {
                GridPlan[x, y] = new CellClass();
                GameObject cloneCellObject = Instantiate(CellPrefab, Vector3.zero, CellPrefab.transform.rotation, CellParent.transform);
                cloneCellObject.transform.localPosition = new Vector3((CellXLength * x), 0, -(CellYLength * y));
                cloneCellObject.GetComponent<CellInfo>().Coordinates = new Vector2(x, y);

                GridPlan[x, y].PosX = x;
                GridPlan[x, y].PosY = y;
                GridPlan[x, y].CellObject = cloneCellObject;

                cloneCellObject.transform.localPosition -= new Vector3(((XSize - 1) / 2f) * CellXLength, 0, -(((YSize - 1) / 2f) * CellYLength));
            }
        }
        CellParent.transform.position = Vector3.zero;

        SetCameraPos();
    }

    private void SetCameraPos()
    {
        float height = (float)Screen.height;
        float width = (float)Screen.width;
        float screenRatio = height / width;

        Camera.main.orthographicSize = (screenRatio / 2f) * (GridSize * CellXLength);

        //----------
        float OffsetZ = 0;

        Ray ray = Camera.main.ScreenPointToRay(new Vector3(width / 2f, height, 0));
        Plane plane = new Plane(Vector3.up, Vector3.zero);
        float enter;
        if (plane.Raycast(ray, out enter))
        {
            Vector3 hitPoint = ray.GetPoint(enter);
            OffsetZ = hitPoint.z - (GridPlan[0, 0].CellObject.transform.position.z + (CellYLength / 2f));
        }

        Camera.main.transform.position -= new Vector3(0, 0, OffsetZ);
    }

    public void CheckAndDestroyMatches()
    {
        List<GameObject> matchingGroups = new List<GameObject>();
        for (int x = 0; x < XSize; x++)
        {
            for (int y = 0; y < YSize; y++)
            {
                List<GameObject> matchingGroup = new List<GameObject>();
                FindMatchingBlocks(GridPlan[x, y].CellObject, ref matchingGroup);

                if (matchingGroup.Count >= 3)
                {
                    matchingGroups = new List<GameObject>(matchingGroup);
                }
            }
        }

        for (int i = 0; i < matchingGroups.Count; i++)
        {
            CellInfo CI = matchingGroups[i].GetComponent<CellInfo>();

            GridPlan[(int)CI.Coordinates.x, (int)CI.Coordinates.y].IsOccupied = false;
            CI.Xobject.SetActive(false);
        }

        if (matchingGroups.Count > 0)
        {
            MatchCounter++;
            MatchCounterText.text = "Match Count: " + MatchCounter.ToString();
        }
    }
    private void FindMatchingBlocks(GameObject block, ref List<GameObject> matchingGroup)
    {
        if (matchingGroup.Contains(block))
            return;
        CellInfo cellObject = block.GetComponent<CellInfo>();
        if (cellObject != null && GridPlan[(int)cellObject.Coordinates.x, (int)cellObject.Coordinates.y].IsOccupied)
        {
            matchingGroup.Add(block);
            int x = (int)cellObject.Coordinates.x;
            int y = (int)cellObject.Coordinates.y;
            if (x > 0)
            {
                if (GridPlan[x - 1, y].IsOccupied)
                {
                    FindMatchingBlocks(GridPlan[x - 1, y].CellObject, ref matchingGroup);
                }
            }
            if (x < XSize - 1)
            {
                if (GridPlan[x + 1, y].IsOccupied)
                {
                    FindMatchingBlocks(GridPlan[x + 1, y].CellObject, ref matchingGroup);
                }
            }
            if (y > 0)
            {
                if (GridPlan[x, y - 1].IsOccupied)
                {
                    FindMatchingBlocks(GridPlan[x, y - 1].CellObject, ref matchingGroup);
                }
            }
            if (y < YSize - 1)
            {
                if (GridPlan[x, y + 1].IsOccupied)
                {
                    FindMatchingBlocks(GridPlan[x, y + 1].CellObject, ref matchingGroup);
                }
            }
        }
    }

    #region Button Functions
    public void RebuildButtonFunc()
    {
        Builder();
    }
    public void GridSizeChanged()
    {
        GridSize = int.Parse(GridSizeInputField.text);
        GridSize = Mathf.Max(1, GridSize);
    }
    #endregion
}
