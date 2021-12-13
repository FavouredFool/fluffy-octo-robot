using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseInteraction : MonoBehaviour
{
    public HexGrid hexGrid;


    HexCell hoveredHex;
    HexCell previouslyhoveredHex;

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
            // Refresh jeden zweiten Frame
            yield return new WaitForSeconds(Time.deltaTime*2);

            // berührte Cell finden
            hoveredHex = SelectedHexCell();

            if (hoveredHex != previouslyhoveredHex)
            {
                // auf Cell Preview-Tile zeigen
                if (previouslyhoveredHex)
                {
                    previouslyhoveredHex.ShowTilePreview(false);
                }
                if (hoveredHex)
                {
                    hoveredHex.ShowTilePreview(true);
                }

                previouslyhoveredHex = hoveredHex;
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
