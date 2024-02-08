using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M_Input : MonoSingleton<M_Input>
{
    public LayerMask CellLayer;

    private bool TouchDown = false;
    private CellInfo TouchedCellInfo = null;

    public void FixedUpdate()
    {
        if (Input.GetMouseButton(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (!TouchDown)
            {
                TouchDown = true;
                if (Physics.Raycast(ray, out hit, 100, CellLayer))
                { 
                    if(hit.collider.gameObject.GetComponentInParent<CellInfo>())
                    {
                        TouchedCellInfo = hit.collider.gameObject.GetComponentInParent<CellInfo>();
                    }
                }
            }
        }
        else
        {
            if (TouchDown && TouchedCellInfo)
            {
                if (!M_Grid.instance.GridPlan[(int)TouchedCellInfo.Coordinates.x, (int)TouchedCellInfo.Coordinates.y].IsOccupied)
                {
                    TouchedCellInfo.Xobject.SetActive(true);
                    M_Grid.instance.GridPlan[(int)TouchedCellInfo.Coordinates.x, (int)TouchedCellInfo.Coordinates.y].IsOccupied = true;

                    M_Grid.instance.CheckAndDestroyMatches();
                }
            }

            TouchDown = false;
            TouchedCellInfo = null;
        }
    }
}
