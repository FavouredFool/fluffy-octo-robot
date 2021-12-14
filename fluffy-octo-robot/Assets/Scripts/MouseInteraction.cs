using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseInteraction : MonoBehaviour
{
    public HexGrid hexGrid;
    private BattleSystem battleSystem;

    HexCell hoveredHex;
    HexCell previouslyhoveredHex;

    // Start is called before the first frame update
    protected void Start()
    {
        battleSystem = FindObjectOfType<BattleSystem>();
        StartCoroutine(HandleHover());
    }

    // Update is called once per frame
    protected void Update()
    {

        if (battleSystem.GetState().Equals(GameState.GODTURN))
        {
            if (Input.GetMouseButtonDown(0))
            {
                HexCell activeHex = SelectedHexCell();
                if (activeHex)
                {
                    activeHex.AddTile();
                }
            }

            if (Input.GetMouseButtonDown(1))
            {
                HexCell activeHex = SelectedHexCell();
                if (activeHex)
                {
                    activeHex.RemoveTile();
                }
            }
        }
        
    }

    protected IEnumerator HandleHover()
    {
        while(true)
        {
            // Refresh jeden zweiten Frame
            yield return new WaitForSeconds(Time.deltaTime * 4);

            if (battleSystem.GetState().Equals(GameState.GODTURN))
            {

                // ber�hrte Cell finden
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
    }


    protected HexCell SelectedHexCell()
    {
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(inputRay, out hit))
        {
            
            // Fix den Raycast noch leicht zu verl�ngern, damit man auch die W�nde ber�hren kann um eine Tile auszuw�hlen
            return GetHexFromPos(hit.point + inputRay.direction.normalized * 0.1f);
            
        }
        return null;
    }

    protected HexCell GetHexFromPos(Vector3 position)
    {
        position = transform.InverseTransformPoint(position);

        HexCoordinates coordinates = HexCoordinates.FromPosition(position);
        HexCell foundHexCell;


        // F�r effiziente L�sung statt durchiterieren �ber alle Cells den korrekten Index mathematisch �ber die Coordinates finden.
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
