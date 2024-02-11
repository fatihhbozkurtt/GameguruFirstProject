using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GridManager : MonoSingleton<GridManager>
{
    [Header("References")]
    public CellStatsContainer[,] GridPlan;
    [SerializeField] GameObject CellParent;
    [SerializeField] GameObject CellPrefab;
    [SerializeField] TextMeshProUGUI MatchCounterText;
    [SerializeField] TMP_InputField GridSizeInputField;

    [Header("Configuration")]
    public float CellXLength;
    public float CellYLength;


    [Header("Debug")]
    [SerializeField] int desiredRowCount;
    [SerializeField] int desiredColumnCount;
    int _gridSize = 0;
    int _matchCounter = 0;

    public void Start()
    {
        _gridSize = 5;
        GenerateGrid();
    }
    public void GenerateGrid()
    {
        if (GridPlan != null)
        {
            DestroyPreviousGrid();
        }

        desiredRowCount = _gridSize;
        desiredColumnCount = _gridSize;

        GridPlan = new CellStatsContainer[desiredRowCount, desiredColumnCount];
        for (int x = 0; x < desiredRowCount; x++)
        {
            for (int y = 0; y < desiredColumnCount; y++)
            {
                GridPlan[x, y] = new CellStatsContainer();
                GameObject cloneCellObject = Instantiate(CellPrefab, Vector3.zero, CellPrefab.transform.rotation, CellParent.transform);
                cloneCellObject.transform.localPosition = new Vector3(CellXLength * x, 0, -(CellYLength * y));
                cloneCellObject.GetComponent<CellController>().SetCoordinates(x, y);

                GridPlan[x, y].PosX = x;
                GridPlan[x, y].PosY = y;
                GridPlan[x, y].CellObject = cloneCellObject;

                cloneCellObject.transform.localPosition -= new Vector3(((desiredRowCount - 1) / 2f) * CellXLength, 0, -(((desiredColumnCount - 1) / 2f) * CellYLength));
            }
        }
        CellParent.transform.position = Vector3.zero;

        SetCameraPos();
    }

    private void DestroyPreviousGrid()
    {
        for (int x = 0; x < GridPlan.GetLength(0); x++)
        {
            for (int y = 0; y < GridPlan.GetLength(1); y++)
            {
                Destroy(GridPlan[x, y].CellObject);
            }
        }
    }

    private void SetCameraPos()
    {
        float height = Screen.height;
        float width = Screen.width;
        float screenRatio = height / width;

        Camera.main.orthographicSize = (screenRatio / 2f) * (_gridSize * CellXLength);

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
        for (int x = 0; x < desiredRowCount; x++)
        {
            for (int y = 0; y < desiredColumnCount; y++)
            {
                List<GameObject> matchingGroup = new List<GameObject>();
                FindMatchingCells(GridPlan[x, y].CellObject, ref matchingGroup);

                if (matchingGroup.Count >= 3)
                {
                    matchingGroups = new List<GameObject>(matchingGroup);
                }
            }
        }

        for (int i = 0; i < matchingGroups.Count; i++)
        {
            CellController cellController = matchingGroups[i].GetComponent<CellController>();

            GetCellStats(cell: cellController).IsOccupied = false;
            cellController.SetCrossImage(activate: false);
        }

        if (matchingGroups.Count > 0)
        {
            _matchCounter++;
            MatchCounterText.text = "Match Count: " + _matchCounter.ToString();
        }
    }
    private void FindMatchingCells(GameObject block, ref List<GameObject> matchingGroup)
    {
        if (matchingGroup.Contains(block)) return;

        CellController cell = block.GetComponent<CellController>();

        if (cell != null && GetCellStats(cell: cell).IsOccupied)
        {
            matchingGroup.Add(block);
            int x = (int)cell.GetCoordinates().x;
            int y = (int)cell.GetCoordinates().y;

            if (x > 0)
            {
                if (GridPlan[x - 1, y].IsOccupied)
                {
                    FindMatchingCells(GridPlan[x - 1, y].CellObject, ref matchingGroup);
                }
            }
            if (x < desiredRowCount - 1)
            {
                if (GridPlan[x + 1, y].IsOccupied)
                {
                    FindMatchingCells(GridPlan[x + 1, y].CellObject, ref matchingGroup);
                }
            }
            if (y > 0)
            {
                if (GridPlan[x, y - 1].IsOccupied)
                {
                    FindMatchingCells(GridPlan[x, y - 1].CellObject, ref matchingGroup);
                }
            }
            if (y < desiredColumnCount - 1)
            {
                if (GridPlan[x, y + 1].IsOccupied)
                {
                    FindMatchingCells(GridPlan[x, y + 1].CellObject, ref matchingGroup);
                }
            }
        }
    }

    public CellStatsContainer GetCellStats(CellController cell)
    {
        return GridPlan[(int)cell.GetCoordinates().x, (int)cell.GetCoordinates().y];
    }

    #region Button Functions
    public void RebuildButtonFunc()
    {
        GenerateGrid();
    }
    public void GridSizeChanged()
    {
        _gridSize = int.Parse(GridSizeInputField.text);
        _gridSize = Mathf.Max(1, _gridSize);
    }
    #endregion
}
