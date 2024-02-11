using UnityEngine;

public class M_Input : MonoSingleton<M_Input>
{
    public LayerMask CellLayer;

    private bool isTouchedDown = false;
    private CellController _touchedCell = null;

    public void FixedUpdate()
    {
        if (Input.GetMouseButton(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (!isTouchedDown)
            {
                isTouchedDown = true;
                if (Physics.Raycast(ray, out hit, 100, CellLayer))
                {
                    CellController cellController = hit.collider.gameObject.GetComponentInParent<CellController>();
                    if (cellController != null)
                    {
                        _touchedCell = cellController;
                    }
                }
            }
        }
        else
        {
            if (isTouchedDown && _touchedCell)
            {
                CellStatsContainer cellStats = GridManager.instance.GetCellStats(_touchedCell);
                if (!cellStats.IsOccupied)
                {
                    _touchedCell.SetCrossImage(activate: true);
                    cellStats.IsOccupied = true;
                    GridManager.instance.CheckAndDestroyMatches();
                }
            }

            isTouchedDown = false;
            _touchedCell = null;
        }
    }
}
