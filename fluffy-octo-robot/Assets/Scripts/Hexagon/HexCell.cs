using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using ObserverPattern;

public class HexCell : MonoBehaviour, IObserver
{

    public GameObject hexPrefab;
    public GameObject hexPreviewPrefab;
    public GameObject hexCellPreviewPrefab;

    public TMP_Text cellLabelPrefab;
    public HexCoordinates coordinates;

    [HideInInspector]
    public int height = 0;

    [HideInInspector]
    public Stack<GameObject> hexStack;

    HexGrid hexGrid;

    GameObject hexPreviewObj = null;
    GameObject hexCellPreviewObj = null;

    Canvas gridCanvas;
    TMP_Text label;

    bool propagating = false;


    protected void Awake()
    {
        hexStack = new Stack<GameObject>();
        //hexGrid = transform.parent.GetComponent<HexGrid>();
        // Bei Awake kann noch nicht über das Parentobjekt gegangen werden
        hexGrid = GameObject.Find("HexGrid").GetComponent<HexGrid>();

        gridCanvas = GetComponentInChildren<Canvas>();

        if (!propagating)
        {
            // Add Preview Prefab
            hexCellPreviewObj = Instantiate(hexCellPreviewPrefab, transform.position + new Vector3(0f, height * HexMetrics.hexHeight, 0f), Quaternion.identity, transform);
        }
    }

    protected void Start()
    {
        // Coordinate-Grids 
        DefineLabel();
    }

    protected void Update()
    {
        
    }

    protected void DefineLabel()
    {
        label = GetComponentInChildren<TMP_Text>();
        label.text = coordinates.ToStringOnSeperateLines();
        label.color = Color.black;
    }

    public void AddTile()
    {
        // Tile in Stack auf korrekter Höhe hinzufügen
        hexStack.Push(Instantiate(hexPrefab, transform.position + new Vector3(0f, height * HexMetrics.hexHeight, 0f), Quaternion.identity, transform));

        // Height des Konstrukts erhöhen
        SetHeight(height + 1);

        // Wenn vorher eine Tilepreview auf der Cell gezeigt wurde, soll diese geupdated werden
        if (hexPreviewObj)
            ShowTilePreview(true);

        // Propagating-Boolean abändern wenn nötig
        UpdatePropagating();
        
        if (Player.Instance && Player.Instance.activeCell == this)
        {
            // set player-character heigher
            Player.Instance.transform.position = Player.Instance.transform.position + new Vector3(0f, HexMetrics.hexHeight, 0f);
        }
        

    }

    public void RemoveTile()
    {

        if (hexStack.Count > 0)
        {
            // Can't remove block completely when player is on it
            if (Player.Instance && (Player.Instance.activeCell != this || height > 1) && this != hexGrid.GetStartCell())
            {
                // Tile in Stack auf korrekter Höhe hinzufügen
                Destroy(hexStack.Pop());

                // Height des Konstrukts erhöhen
                SetHeight(height - 1);

                // Wenn vorher eine Tilepreview auf der Cell gezeigt wurde, soll diese geupdated werden
                if (hexPreviewObj)
                    ShowTilePreview(true);

                // Propagating-Boolean abändern wenn nötig
                UpdatePropagating();

                if (Player.Instance.activeCell == this)
                {
                    // set player-character lower
                    Player.Instance.transform.position = Player.Instance.transform.position - new Vector3(0f, HexMetrics.hexHeight, 0f);

                }
            }
        } 
    }

    void UpdatePropagating()
    {
        if (height == 0 && propagating)
        {
            propagating = false;
            hexCellPreviewObj = Instantiate(hexCellPreviewPrefab, transform.position + new Vector3(0f, height * HexMetrics.hexHeight, 0f), Quaternion.identity, transform);

            // Update surrounding HexCells -> Remove Hexcells
            foreach (HexCoordinates activeCoordinates in GenerateCellCoordinatesInRadius(1))
            {
                HexCell activeCell = hexGrid.GetCell(activeCoordinates);
                
                if (activeCell && !activeCell.GetPropagating())
                {

                    // Cell exists and isnt propagating -> Check if it has other neighbours
                    bool neighbourFound = false;
                    
                    foreach (HexCoordinates neighbourCoordinates in activeCell.GenerateCellCoordinatesInRadius(1))
                    {
                        HexCell neighboursNeighbour = hexGrid.GetCell(neighbourCoordinates);

                        if (neighboursNeighbour && neighboursNeighbour != activeCell && neighboursNeighbour != this && neighboursNeighbour.GetPropagating())
                        {
                            neighbourFound = true;
                            //Debug.Log(activeCell.coordinates);
                            break;
                        }
                    }

                    if (!neighbourFound)
                    {
                        // Wichtig, beim Destroyed von HexCells, diese auch aus der Liste löschen
                        //hexGrid.GetCells().Remove(activeCell);
                        //Destroy(activeCell.gameObject);
                        hexGrid.RemoveCell(activeCell);
                        
                    }
                }
            }


        } else if (height != 0 && !propagating)
        {
            propagating = true;

            Destroy(hexCellPreviewObj);
            hexCellPreviewObj = null;

            // Update surrounding HexCells -> Add new HexCells
            foreach(HexCoordinates activeCoordinates in GenerateCellCoordinatesInRadius(1))
            {
                HexCell activeCell = hexGrid.GetCell(activeCoordinates);
                if (!activeCell)
                {
                    // Cell doesnt exist yet -> use Coordinates to create new Cell at position
                    Vector3 offsetCoordinates = HexCoordinates.ToOffsetCoordinates(activeCoordinates);

                    hexGrid.CreateCell((int)offsetCoordinates.x, (int)offsetCoordinates.z);

                }
            }
        }
    }

    public IEnumerable GenerateCellCoordinatesInRadius(int radius)
    {
        // Step 1: Find all surrounding Hexes

        int centerX = coordinates.X;
        int centerZ = coordinates.Z;

        for (int r = 0, z = centerZ - radius; z <= centerZ; z++, r++)
        {
            for (int x = centerX - r; x <= centerX + radius; x++)
            {
                yield return (new HexCoordinates(x, z));
            }
        }
        for (int r = 0, z = centerZ + radius; z > centerZ; z--, r++)
        {
            for (int x = centerX - radius; x <= centerX + r; x++)
            {
                yield return (new HexCoordinates(x, z));
            }
        }
    }



    public void ShowTilePreview(bool active)
    {
        // Activate / Deactivate Tilepreview of a Tile
        if (active)
        {
            if (!hexPreviewObj)
            {
                hexPreviewObj = Instantiate(hexPreviewPrefab, transform.position + new Vector3(0f, height * HexMetrics.hexHeight, 0f), Quaternion.identity, transform);
            } else
            {
                // Falls ShowTilePreview aufgerufen wird obwohl das hex schon exitiert, update die Hexposition
                ShowTilePreview(false);
                ShowTilePreview(true);
            }
            
        } else
        {
            if (hexPreviewObj)
            {
                Destroy(hexPreviewObj);
                hexPreviewObj = null;
            }
        }

    }

    public void PlacePlayer()
    {
        Player.Instance.activeCell = this;
        Player.Instance.transform.position = transform.position + new Vector3(0f, height * HexMetrics.hexHeight + HexMetrics.hexHeight / 2, 0f);
        if (TemporaryTurnControl.gameState == TemporaryTurnControl.GameState.HUMAN)
        {
            // calculate preview Tiles
            CalculatePreviewTilesForHuman(true);

        }
    }

    public void CalculatePreviewTilesForHuman(bool active)
    {
        foreach (HexCoordinates activeCoordinates in Player.Instance.activeCell.GenerateCellCoordinatesInRadius(1))
        {
            HexCell activeCell = hexGrid.GetCell(activeCoordinates);

            if (active)
            {
                // Calculate if they should be on -> they are previously all turned off; no turning off necessary
                if (activeCell.ValdiatePlacement())
                {
                    activeCell.ShowTilePreview(true);
                }
            } else
            {
                activeCell.ShowTilePreview(false);
            }
            
            

        }

    }

    public bool ValdiatePlacement()
    {
        // Check if Player is allowed to be placed at that position based on his previous position
        if (propagating && Player.Instance.activeCell != this)
        {
            return GetHeight() - Player.Instance.activeCell.GetHeight() <= Player.Instance.maxWalkHeight;
        }
        return false;
    }

    public void RemovePlayer()
    {
        if (TemporaryTurnControl.gameState == TemporaryTurnControl.GameState.HUMAN)
        {
            // calculate preview Tiles
            CalculatePreviewTilesForHuman(false);
        }

        Player.Instance.activeCell = null;

        
    }

    public int GetHeight()
    {
        return height;
    }


    protected void SetHeight(int newHeight)
    {
        // Höhe ändern:
        height = newHeight;

        // Change CanvasPosition
        gridCanvas.transform.localPosition = new Vector3(0f, (newHeight - 1/2f) * HexMetrics.hexHeight, 0f);
    }

    public bool GetPropagating()
    {
        return propagating;
    }

    public void OnNotify()
    {
        // OnChange des Turnstates werden alle Preview-Cells zerstört und ggf. neue berechnet
        ShowTilePreview(false);

        if (TemporaryTurnControl.gameState == TemporaryTurnControl.GameState.HUMAN)
        {
            Player.Instance.activeCell.CalculatePreviewTilesForHuman(true);
        }
    }

    public void ClearStack()
    {
        foreach (GameObject tile in hexStack)
        {
            Destroy(tile);
        }
        hexStack.Clear();
        SetHeight(0);
        UpdatePropagating();
        CalculatePreviewTilesForHuman(false);
        CalculatePreviewTilesForHuman(true);
    }

}
