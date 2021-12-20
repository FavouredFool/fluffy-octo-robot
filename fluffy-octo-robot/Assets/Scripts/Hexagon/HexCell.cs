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

    bool propagating = false;

    private BattleSystem battleSystem;
    private PlayerControl playerControl;


    protected void Awake()
    {
        hexStack = new Stack<GameObject>();
        //hexGrid = transform.parent.GetComponent<HexGrid>();
        // Bei Awake kann noch nicht �ber das Parentobjekt gegangen werden
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
        battleSystem = FindObjectOfType<BattleSystem>();

        // Coordinate-Grids 
        DefineLabel();
    }

    private void InstantiatePlayer()
    {
        playerControl = FindObjectOfType<PlayerControl>();
    }

    private bool CheckIfPlayerIsInstantiated()
    {
        return playerControl == null;
    }

    protected void DefineLabel()
    {
        label = GetComponentInChildren<TMP_Text>();
        label.text = coordinates.ToStringOnSeperateLines();
        label.color = Color.black;
    }

    public void AddTile()
    {
        // Tile in Stack auf korrekter H�he hinzuf�gen
        hexStack.Push(Instantiate(hexPrefab, transform.position + new Vector3(0f, height * HexMetrics.hexHeight, 0f), Quaternion.identity, transform));

        // Height des Konstrukts erh�hen
        SetHeight(height + 1);

        // Wenn vorher eine Tilepreview auf der Cell gezeigt wurde, soll diese geupdated werden
        if (hexPreviewObj)
            ShowTilePreview(true);

        // Propagating-Boolean ab�ndern wenn n�tig
        UpdatePropagating();
        
        if (playerControl && playerControl.activeCell == this)
        {
            // set player-character heigher
            playerControl.transform.position = playerControl.transform.position + new Vector3(0f, HexMetrics.hexHeight, 0f);
        }
        

    }

    public void RemoveTile()
    {

        if (hexStack.Count > 0)
        {
            if (CheckIfPlayerIsInstantiated())
            {
                InstantiatePlayer();
            }

            // Can't remove block completely when player is on it
            if (playerControl && (playerControl.activeCell != this || height > 1))
            {
                // Tile in Stack auf korrekter H�he hinzuf�gen
                Destroy(hexStack.Pop());

                // Height des Konstrukts erh�hen
                SetHeight(height - 1);

                // Wenn vorher eine Tilepreview auf der Cell gezeigt wurde, soll diese geupdated werden
                if (hexPreviewObj)
                    ShowTilePreview(true);

                // Propagating-Boolean ab�ndern wenn n�tig
                UpdatePropagating();

                if (playerControl.activeCell == this)
                {
                    // set player-character lower
                    playerControl.transform.position = playerControl.transform.position - new Vector3(0f, HexMetrics.hexHeight, 0f);

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

    public void FirstPlayerPlace()
    {
        InstantiatePlayer();

        playerControl.activeCell = this;
        playerControl.transform.position = transform.position + new Vector3(0f, (height + 1) * HexMetrics.hexHeight + HexMetrics.hexHeight / 2, 0f);
    }

    public void PlacePlayer()
    {
        if (CheckIfPlayerIsInstantiated())
        {
            InstantiatePlayer();
        }
        Debug.Log("Place Player " + height);

        playerControl.activeCell = this;
        playerControl.transform.position = transform.position + new Vector3(0f, height * HexMetrics.hexHeight + HexMetrics.hexHeight / 2, 0f);

        if (battleSystem.GetState().Equals(GameState.PLAYERTURN))
        {
            // calculate preview Tiles
            CalculatePreviewTilesForHuman(true);
        }
    }

    public void CalculatePreviewTilesForHuman(bool active)
    {
        if (CheckIfPlayerIsInstantiated())
        {
            InstantiatePlayer();
        }

        foreach (HexCoordinates activeCoordinates in playerControl.activeCell.GenerateCellCoordinatesInRadius(1))
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
        if (CheckIfPlayerIsInstantiated())
        {
            InstantiatePlayer();
        }

        // Check if Player is allowed to be placed at that position based on his previous position
        if (propagating && playerControl.activeCell != this)
        {
            return GetHeight() - playerControl.activeCell.GetHeight() <= playerControl.maxWalkHeight;
        }
        return false;
    }

    public void RemovePlayer()
    {
        if (CheckIfPlayerIsInstantiated())
        {
            InstantiatePlayer();
        }

        if (battleSystem.GetState().Equals(GameState.PLAYERTURN))
        {
            // calculate preview Tiles
            CalculatePreviewTilesForHuman(false);
        }

        playerControl.activeCell = null;

        
    }

    public int GetHeight()
    {
        return height;
    }


    protected void SetHeight(int newHeight)
    {
        // H�he �ndern:
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
        // OnChange des Turnstates werden alle Preview-Cells zerst�rt und ggf. neue berechnet
        ShowTilePreview(false);

        if (CheckIfPlayerIsInstantiated())
        {
            InstantiatePlayer();
        }

        if (battleSystem.GetState().Equals(GameState.PLAYERTURN))
        {
            playerControl.activeCell.CalculatePreviewTilesForHuman(true);
        }
    }

}
