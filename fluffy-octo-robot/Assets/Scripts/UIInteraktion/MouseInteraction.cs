using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseInteraction : MonoBehaviour
{

    private ActionPoints actionPoints;

    public HexGrid hexGrid;

    HexCell hoveredHex;
    HexCell previouslyhoveredHex;

    // Start is called before the first frame update
    protected void Start()
    {
        actionPoints = FindObjectOfType<ActionPoints>();

        StartCoroutine(HandleHover());
    }

    // Update is called once per frame
    protected void Update()
    {
        if (!hexGrid.blockActions)
        {
            if (
                PlayersManager.Instance.CurrentGameState.Equals(GameState.GODTURN) &&
                (PlayersManager.Instance.IsGod || PlayersManager.Instance.IsSingle) &&
                actionPoints.GetCurrentActionPoints() != 0
            ) {
                if (Input.GetMouseButtonDown(0))
                {
                    HexCell activeHex = SelectedHexCell();
                    if (activeHex)
                    {
                        activeHex.AddTileManually();
                        
                    }
                }

                if (Input.GetMouseButtonDown(1))
                {
                    HexCell activeHex = SelectedHexCell();
                    if (activeHex)
                    {
                        activeHex.RemoveTileManually();
                    }
                }
            }
            else if (
                PlayersManager.Instance.CurrentGameState.Equals(GameState.HUMANTURN) &&
                (PlayersManager.Instance.IsHuman || PlayersManager.Instance.IsSingle) &&
                actionPoints.GetCurrentActionPoints() != 0
            ) {

                if (Input.GetMouseButtonDown(0))
                {
                    HexCell pressedCell = SelectedHexCell();
                    if (pressedCell && pressedCell.GetHeight() > 0)
                    {
                        // Move Towards that Tile if possible
                        foreach (HexCoordinates activeCoordinates in hexGrid.GetCell(Player.Instance.activeCellCoordinates).GenerateCellCoordinatesInRadius(1))
                        {
                            HexCell activeCell = hexGrid.GetCell(activeCoordinates);

                            if (pressedCell == activeCell && pressedCell.ValdiatePlacement())
                            {
                                pressedCell.PlacePlayer();
                                return;
                            }

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
            yield return new WaitForSeconds(Time.deltaTime * 2);
            
            if (
                PlayersManager.Instance.CurrentGameState.Equals(GameState.GODTURN) &&
                (PlayersManager.Instance.IsGod || PlayersManager.Instance.IsSingle) &&
                actionPoints.GetCurrentActionPoints() != 0
            ) {

                // ber???hrte Cell finden
                hoveredHex = SelectedHexCell();

                if (hoveredHex)
                {
                    int counter = 0;
                    foreach (HexCoordinates hexCoordinates in hexGrid.goalCellCoordinates)
                    {
                        if (hoveredHex.coordinates.Equals(hexCoordinates))
                        {
                            counter++;
                        }
                    }

                    if (!hoveredHex.coordinates.Equals(hexGrid.GetStartCellCoordiantes()) && counter == 0 && !Player.Instance.activeCellCoordinates.Equals(hoveredHex.coordinates))
                    {
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

                    } else if (previouslyhoveredHex)
                    {
                        previouslyhoveredHex.ShowTilePreview(false);
                    }
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
            
            // Fix den Raycast noch leicht zu verl???ngern, damit man auch die W???nde ber???hren kann um eine Tile auszuw???hlen
            return GetHexFromPos(hit.point + inputRay.direction.normalized * 0.1f);
            
        }
        return null;
    }

    protected HexCell GetHexFromPos(Vector3 position)
    {
        position = transform.InverseTransformPoint(position);

        HexCoordinates coordinates = HexCoordinates.FromPosition(position);
        HexCell foundHexCell;

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
