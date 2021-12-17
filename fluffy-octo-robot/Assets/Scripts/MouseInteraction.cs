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
        if (TemporaryTurnControl.gameState == TemporaryTurnControl.GameState.GOD)
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

        if (TemporaryTurnControl.gameState == TemporaryTurnControl.GameState.HUMAN)
        {
            if (Input.GetMouseButtonDown(0))
            {
                HexCell pressedCell = SelectedHexCell();
                if (pressedCell && pressedCell.GetPropagating())
                {
                    
                    // Move Towards that Tile if possible

                    foreach (HexCoordinates activeCoordinates in Player.Instance.activeCell.GenerateCellCoordinatesInRadius(1))
                    {
                        HexCell activeCell = hexGrid.GetCell(activeCoordinates);


                        if (pressedCell == activeCell && Player.Instance.activeCell && pressedCell.GetHeight() - Player.Instance.activeCell.GetHeight() <= Player.Instance.maxWalkHeight)
                        {
                            
                            activeCell.RemovePlayer();
                            pressedCell.PlacePlayer();
                            return;
                        }

                    }
                }
            }
        }
    }

    protected IEnumerator HandleHover()
    {
        while(true)
        {
            // Refresh jeden zweiten Frame
            yield return new WaitForSeconds(Time.deltaTime*4);

            if (TemporaryTurnControl.gameState == TemporaryTurnControl.GameState.GOD)
            {
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
    }


    protected HexCell SelectedHexCell()
    {
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(inputRay, out hit))
        {
            
            // Fix den Raycast noch leicht zu verlängern, damit man auch die Wände berühren kann um eine Tile auszuwählen
            return GetHexFromPos(hit.point + inputRay.direction.normalized * 0.1f);
            
        }
        return null;
    }

    protected HexCell GetHexFromPos(Vector3 position)
    {
        position = transform.InverseTransformPoint(position);

        HexCoordinates coordinates = HexCoordinates.FromPosition(position);
        HexCell foundHexCell;


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
