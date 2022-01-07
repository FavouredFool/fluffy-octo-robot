using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HexCell : MonoBehaviour
{

    public GameObject hexPrefab;
    public GameObject hexPreviewPrefab;
    public GameObject hexCellPreviewPrefab;

    public TMP_Text cellLabelPrefab;

    [HideInInspector]
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


    protected void Awake()
    {
        hexStack = new Stack<GameObject>();
        //hexGrid = transform.parent.GetComponent<HexGrid>();
        // Bei Awake kann noch nicht ueber das Parentobjekt gegangen werden
        hexGrid = GameObject.Find("HexGrid").GetComponent<HexGrid>();

        gridCanvas = GetComponentInChildren<Canvas>();

        // Add Preview Prefab
        hexCellPreviewObj = Instantiate(hexCellPreviewPrefab, transform.position + new Vector3(0f, height * HexMetrics.hexHeight, 0f), Quaternion.identity, transform);

    }

    protected void Start()
    {
        // Coordinate-Grids 
        DefineLabel();
    }

    /*
    private void InstantiatePlayer()
    {
        Debug.Log("instantiate Player");
        playerControl = FindObjectOfType<PlayerControl>();
    }

    private bool CheckIfPlayerIsInstantiated()
    {
        return playerControl == null;
    }
    */

    protected void DefineLabel()
    {
        label = GetComponentInChildren<TMP_Text>();
        label.text = coordinates.ToStringOnSeperateLines();
        label.color = Color.black;
    }

    public void AddTile()
    {
        // Tile in Stack auf korrekter Hoehe hinzufuegen
        hexStack.Push(Instantiate(hexPrefab, transform.position + new Vector3(0f, height * HexMetrics.hexHeight, 0f), Quaternion.identity, transform));

        // Height des Konstrukts erh�hen
        SetHeight(height + 1);
    }

    public void RemoveTile()
    {

        if (hexStack.Count > 0)
        {

            // Can't remove block completely when player is on it
            if (hexGrid.GetCell(Player.Instance.activeCellCoordinates) != this || height > 1)
            {
                // Tile in Stack popen
                Destroy(hexStack.Pop());

                // Height des Konstrukts verringern
                SetHeight(height - 1);

            }
        } 
    }

    public void AddTileManually()
    {
        // Height des Konstrukts erhöhen
        SetHeight(height + 1);

        // Reform World
        hexGrid.ReformWorld();
    }

    public void RemoveTileManually()
    {

        if (height > 0 && (height != 1 || !coordinates.Equals(hexGrid.GetStartCellCoordiantes())))
        {

            // Height des Konstrukts verringern
            SetHeight(height - 1);

            // Reform World
            hexGrid.ReformWorld();

        }
    }


    /*
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
                        // Wichtig, beim Destroyed von HexCells, diese auch aus der Liste l�schen
                        hexGrid.GetCells().Remove(activeCell);
                        Destroy(activeCell.gameObject);
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

    */

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

    public void PlacePlayerRebuild()
    {
        Player.Instance.activeCellCoordinates = coordinates;
        Player.Instance.transform.position = transform.position + new Vector3(0f, height * HexMetrics.hexHeight + HexMetrics.hexHeight / 2, 0f);
    }


    
    public void PlacePlayer()
    {
        Player.Instance.activeCellCoordinates = coordinates;
        Player.Instance.transform.position = transform.position + new Vector3(0f, height * HexMetrics.hexHeight + HexMetrics.hexHeight / 2, 0f);


        //Reform World
        hexGrid.ReformWorld();

        /*
        if (PlayersManager.Instance.CurrentGameState == GameState.HUMANTURN)
        {
            // calculate preview Tiles
            CalculatePreviewTilesForHuman(true);

        }
        */
        /*
        playerControl.activeCell = this;
        playerControl.transform.position = transform.position + new Vector3(0f, height * HexMetrics.hexHeight + HexMetrics.hexHeight / 2, 0f);

        if (PlayersManager.Instance.CurrentGameState.Equals(GameState.PLAYERTURN))
        {
            // calculate preview Tiles
            CalculatePreviewTilesForHuman(true);
        }
        */
    }
    

    public void CalculatePreviewTilesForHuman(bool active)
    {

        foreach (HexCoordinates activeCoordinates in hexGrid.GetCell(Player.Instance.activeCellCoordinates).GenerateCellCoordinatesInRadius(1))
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
        if (GetHeight() > 0 && hexGrid.GetCell(Player.Instance.activeCellCoordinates) != this)
        {
            return GetHeight() - hexGrid.GetCell(Player.Instance.activeCellCoordinates).GetHeight() <= Player.Instance.maxWalkHeight;
        }

        return false;
    }

/*
    public void RemovePlayer()
    {

        if (PlayersManager.Instance.CurrentGameState.Equals(GameState.HUMANTURN))
        {
            // calculate preview Tiles
            CalculatePreviewTilesForHuman(false);
        }

    }
*/

    public int GetHeight()
    {
        return height;
    }


    protected void SetHeight(int newHeight)
    {
        // Hoehe aendern:
        height = newHeight;

        // Change CanvasPosition
        gridCanvas.transform.localPosition = new Vector3(0f, (newHeight - 1/2f) * HexMetrics.hexHeight, 0f);
    }



}
