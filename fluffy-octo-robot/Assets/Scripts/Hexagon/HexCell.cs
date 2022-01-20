using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HexCell : MonoBehaviour
{

    public GameObject[] hexPrefabs;

    public GameObject hexPreviewPrefab;
    public GameObject hexCellPreviewPrefab;

    public GameObject collectable;

    public TMP_Text cellLabelPrefab;
    public TMP_Text corruptionLabelPrefab;

    public Material corruptionMaterial;

    public GameObject numberCanvas;

    private ActionPoints actionPoints;

    [HideInInspector]
    public HexCoordinates coordinates;

    [HideInInspector]
    public int height = 0;

    [HideInInspector]
    public Stack<GameObject> hexStack;

    HexGrid hexGrid;

    GameObject hexPreviewObj = null;
    GameObject hexCellPreviewObj = null;

    TMP_Text label;

    int roundsTillCorrupted = -1;

    public enum Biome { GROUND, HOME, WOOD, STONE }

    public Biome cellBiome = Biome.WOOD;

    int collectedIndex;

    protected void Awake()
    {
        actionPoints = FindObjectOfType<ActionPoints>();
        hexStack = new Stack<GameObject>();
        //hexGrid = transform.parent.GetComponent<HexGrid>();
        // Bei Awake kann noch nicht ueber das Parentobjekt gegangen werden
        hexGrid = GameObject.Find("HexGrid").GetComponent<HexGrid>();


        // Add Preview Prefab
        hexCellPreviewObj = Instantiate(hexCellPreviewPrefab, transform.position + new Vector3(0f, height * HexMetrics.hexHeight, 0f), Quaternion.identity, transform);

    }

    protected void Start()
    {

        if (numberCanvas.activeSelf)
        {
            // Coordinate-Grids 
            DefineLabel();
        }
        

        for (int i=0; i<hexGrid.goalCellCoordinates.Count; i++)
        {
            if (hexGrid.goalCellCoordinates[i].Equals(coordinates))
            {
                collectedIndex = i;
                if (!hexGrid.goalCollected[i])
                {
                    collectable.SetActive(true);
                    break;
                }
            }
            collectable.SetActive(false);
        }

    }

    protected void DefineLabel()
    {
        label = GetComponentInChildren<TMP_Text>();
        label.text = coordinates.ToStringOnSeperateLines();
        label.color = Color.black;
    }

    public void AddTile(Biome biome)
    {
        // Tile in Stack auf korrekter Hoehe hinzufuegen
        hexStack.Push(Instantiate(hexPrefabs[(int)biome], transform.position + new Vector3(0f, height * HexMetrics.hexHeight, 0f), Quaternion.identity, transform));
        // Height des Konstrukts erhoehen
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

    public void AddTileNoReform()
    {
        // Biome wird gesetzt
        GameObject prefabToPlace;

        if (coordinates.Equals(hexGrid.GetStartCellCoordiantes()))
        {
            prefabToPlace = hexPrefabs[1];
            cellBiome = Biome.HOME;
        }
        else
        {
            int BiomeNumber = Random.Range(2, hexPrefabs.Length);
            prefabToPlace = hexPrefabs[BiomeNumber];
            cellBiome = (Biome)BiomeNumber;
        }

        // Tile in Stack auf korrekter Hoehe hinzufuegen
        hexStack.Push(Instantiate(prefabToPlace, transform.position + new Vector3(0f, height * HexMetrics.hexHeight, 0f), Quaternion.identity, transform));


        // Height des Konstrukts erhöhen
        SetHeight(height + 1);
    }

    public void AddTileManually()
    {
        if (actionPoints.GetCurrentActionPoints() == 0 || !PlayersManager.Instance.IsGod)
        {
            return;
        }
        
        /*
        // Biome wird gesetzt
        GameObject prefabToPlace;

        if (coordinates.Equals(hexGrid.GetStartCellCoordiantes()))
        {
            prefabToPlace = hexPrefabs[1];
        } else
        {
            int BiomeNumber = Random.Range(2, hexPrefabs.Length);
            prefabToPlace = hexPrefabs[BiomeNumber];
            cellBiome = (Biome) BiomeNumber;
        }
        */

        // Tile in Stack auf korrekter Hoehe hinzufuegen
        hexStack.Push(Instantiate(hexPrefabs[(int)cellBiome], transform.position + new Vector3(0f, height * HexMetrics.hexHeight, 0f), Quaternion.identity, transform));


        // Height des Konstrukts erhöhen
        SetHeight(height + 1);

        actionPoints.UpdateActionPoints();

        hexGrid.ReformWorld();
    }

    public void RemoveTileManually()
    {
        if (actionPoints.GetCurrentActionPoints() == 0 || !PlayersManager.Instance.IsGod)
        {
            return;
        }

        //if (height > 0 && (height != 1 || (!coordinates.Equals(hexGrid.GetStartCellCoordiantes()) && hexGrid.GetCell(Player.Instance.activeCellCoordinates) != this)))
        if (height > 0 && (!coordinates.Equals(hexGrid.GetStartCellCoordiantes()) && hexGrid.GetCell(Player.Instance.activeCellCoordinates) != this))
        {
            foreach (HexCoordinates hexCoordinates in hexGrid.goalCellCoordinates)
            {
                //if (!(height > 0 && (height != 1 || (!coordinates.Equals(hexCoordinates)))))
                if (!(height > 0 && (!coordinates.Equals(hexCoordinates))))
                {
                    return;
                }
            }

            // Height des Konstrukts verringern
            SetHeight(height - 1);

            actionPoints.UpdateActionPoints();

            // Reform World
            hexGrid.ReformWorld();

        }
    }


    public IEnumerable GenerateCellCoordinatesInRadius(int radius)
    {
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

    public IEnumerable GenerateCellCoordinatesOnBorderOfRadius(int radius)
    {
        int centerX = coordinates.X;
        int centerZ = coordinates.Z;

        for (int r = 0, z = centerZ - radius; z <= centerZ; z++, r++)
        {
            for (int x = centerX - r; x <= centerX + radius; x++)
            {
                if (r == 0 || x==centerX-r || x==centerX+radius)
                {
                    yield return (new HexCoordinates(x, z));
                }
                
            }
        }
        for (int r = 0, z = centerZ + radius; z > centerZ; z--, r++)
        {
            for (int x = centerX - radius; x <= centerX + r; x++)
            {
                if (r==0 || x == centerX - radius || x == centerX + r)
                {
                    yield return (new HexCoordinates(x, z));
                }
                
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

    public void PlacePlayerForRebuild()
    {
        Player.Instance.activeCellCoordinates = coordinates;
        Player.Instance.transform.position = transform.position + new Vector3(0f, height * HexMetrics.hexHeight + HexMetrics.hexHeight / 2, 0f);
    }

    
    public void PlacePlayer()
    {
        if (actionPoints.GetCurrentActionPoints() == 0 || PlayersManager.Instance.IsGod)
        {
            return;
        }

      
            Player.Instance.activeCellCoordinates = coordinates;
            Player.Instance.transform.position = transform.position + new Vector3(0f, height * HexMetrics.hexHeight + HexMetrics.hexHeight / 2, 0f);

            actionPoints.UpdateActionPoints();

        if (collectable.activeSelf)
        {
            Player.Instance.collected++;
            hexGrid.goalCollected[collectedIndex] = true;
            hexGrid.cellCorruptionAmount++;
        }

        if (hexGrid.GetStartCellCoordiantes().Equals(coordinates) && Player.Instance.collected >= 3)
        {
            PlayersManager.Instance.UpdateGameStateServerRpc(GameState.WON);
        }

        //Reform World
        hexGrid.ReformWorld();


    }

    public void CalculatePreviewTilesForHuman(bool active)
    {
        if (actionPoints.GetCurrentActionPoints() != 0 && PlayersManager.Instance.IsHuman)
        {
            foreach (HexCoordinates activeCoordinates in hexGrid.GetCell(Player.Instance.activeCellCoordinates).GenerateCellCoordinatesInRadius(1))
            {
                HexCell activeCell = hexGrid.GetCell(activeCoordinates);
                if (activeCell)
                {
                    if (active)
                    {
                        // Calculate if they should be on -> they are previously all turned off; no turning off necessary
                        if (activeCell.ValdiatePlacement())
                        {
                            activeCell.ShowTilePreview(true);
                        }
                    }
                    else
                    {
                        activeCell.ShowTilePreview(false);
                    }
                }
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

    public void CorruptCellWithoutRebuild(int corruptionDuration)
    {
        roundsTillCorrupted = corruptionDuration;

        if (roundsTillCorrupted == 0)
        {
            hexGrid.GetCells().Remove(this);
        }
    }

    public void CorruptCell(int corruptionDuration)
    {

        // Redraw Tile-Color / Change Material
        roundsTillCorrupted = corruptionDuration;

        if (roundsTillCorrupted == 0)
        {
            hexGrid.GetCells().Remove(this);
        }
    }

    public void CorruptCellForRebuild(int corruptionDuration)
    {
         
        roundsTillCorrupted = corruptionDuration;
        corruptionLabelPrefab.text = roundsTillCorrupted.ToString();

        // Redraw Tile-Color / Change Material
        foreach (GameObject activeTile in hexStack)
        {
            activeTile.GetComponent<Renderer>().material = corruptionMaterial;
        }

    }

    public int GetHeight()
    {
        return height;
    }


    protected void SetHeight(int newHeight)
    {
        // Hoehe aendern:
        height = newHeight;

        // Change CanvasPosition
        if (numberCanvas.activeSelf)
        {
            cellLabelPrefab.transform.parent.localPosition = new Vector3(0f, (newHeight - 1 / 2f) * HexMetrics.hexHeight, 0f);
        }
        
        corruptionLabelPrefab.transform.parent.localPosition = new Vector3(0f, (newHeight - 1 / 2f) * HexMetrics.hexHeight + 15, 0f);
        collectable.transform.localPosition = new Vector3(0f, (newHeight - 1 / 2f) * HexMetrics.hexHeight + 10, 0f);
    }

    public int GetRoundsTillCorrupted()
    {
        return roundsTillCorrupted;
    }

    public void SetRoundsTillCorrupted(int rounds)
    {
        roundsTillCorrupted = rounds;
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
