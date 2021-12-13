using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseInteraction : MonoBehaviour
{
    public HexGrid hexGrid;

    // Start is called before the first frame update
    protected void Start()
    {
        StartCoroutine(HandleHover());
    }

    // Update is called once per frame
    protected void Update()
    {

        if (Input.GetMouseButtonDown(0))
        {
            HexCell activeHex = SelectedHexCell();
            if (activeHex)
            {
                Debug.Log(activeHex + " " + activeHex.coordinates);
            }
            
        }

    }

    protected IEnumerator HandleHover()
    {
        while(true)
        {
            yield return new WaitForSeconds(1f);
            HexCell activeHex = SelectedHexCell();
            if (activeHex)
            {
                Debug.Log(activeHex + " " + activeHex.coordinates);
            }
            
        }
        
    }


    protected HexCell SelectedHexCell()
    {
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(inputRay, out hit))
        {
            return GetHexFromPos(hit.point);
        }
        return null;
    }

    protected HexCell GetHexFromPos(Vector3 position)
    {
        position = transform.InverseTransformPoint(position);

        HexCoordinates coordinates = HexCoordinates.FromPosition(position);
        HexCell foundHexCell = null;


        // Für effiziente Lösung statt durchiterieren über alle Cells den korrekten Index mathematisch über die Coordinates finden.
        foreach (HexCell activeCell in hexGrid.GetCells())
        {
            if (activeCell.coordinates.Equals(coordinates))
            {
                foundHexCell = activeCell;
                return activeCell;
            }
        }
        return null;
    }

    
}
