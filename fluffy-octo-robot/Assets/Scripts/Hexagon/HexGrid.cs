using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System.Collections;
using static HexCell;
using UnityEngine.SceneManagement;

public class HexGrid : NetworkBehaviour
{
    private int currentGridVersion;

    public int size = 5;

    public HexCell cellPrefab;

    public GameObject playerPrefab;

    public int startTileCorruptionDuration;

    public int corruptionMinDuration;
    public int corruptionMaxDuration;

    public int corruptionDivision;

    List<HexCell> cells;

    HexCoordinates startCellCoordinates = new HexCoordinates(0, 0);

    float gridRadius = 0f;

    List<HexCell> tempCells;

    [HideInInspector]
    public bool blockActions = false;
    
    private void Start()
    {
        currentGridVersion = PlayersManager.Instance.CurrentGridVersion;

        // initiale Capacity bereitstellen
        cells = new(TriangleNumber(size) + TriangleNumber(size - 1) - 2 * TriangleNumber(size / 2));

        // Terraingeneration
        HexCell startCell = CreateCellFromHexCoordinate(startCellCoordinates);

        Instantiate(playerPrefab);
        Player.Instance.activeCellCoordinates = startCellCoordinates;

        foreach (HexCoordinates coordinates in startCell.GenerateCellCoordinatesInRadius(size / 2))
        {
            if (!coordinates.Equals(startCellCoordinates))
            {
                Vector3 offsetCoordinates = HexCoordinates.ToOffsetCoordinates(coordinates);

                CreateCell((int)offsetCoordinates.x, (int)offsetCoordinates.z);
            }
        }

        List<HexCell> startCells = new List<HexCell>(cells);

        foreach (HexCell activeCell in startCells)
        {
            for (int i = 0; i<Random.Range(1, 5); i++)
            {
                activeCell.AddTileNoReform();
            }
        }

        // Corrupt HomeHex
        GetCell(startCellCoordinates).CorruptCell(startTileCorruptionDuration);

        ReformWorld();
    }

    private void Update()
    {
        if (PlayersManager.Instance.CurrentGridVersion != currentGridVersion)
        {
            currentGridVersion = PlayersManager.Instance.CurrentGridVersion;

            InstantiateTiles();
        }
    }

    public void ReformWorld()
    {
        // send cells to Networking
        blockActions = true;
        StartCoroutine(PlayersManager.Instance.SerializeAndUpdateHexCells(cells));
    }

    public HexCell CreateCell(int x, int z)
    {
        Vector3 position;
        position.x = (x + z / 2f - z / 2) * (HexMetrics.innerRadius * 2f);
        position.y = 0f;
        position.z = z * (HexMetrics.outerRadius * 1.5f);

        HexCell cell = Instantiate(cellPrefab);
        cell.coordinates = HexCoordinates.FromOffsetCoordinates(x, z);
        cell.transform.SetParent(transform, false);
        cell.transform.localPosition = position;

        cells.Add(cell);

        // gridRadius ggf. ab?ndern
        float distanceFromStart = Vector3.Distance(Vector3.zero, cell.transform.position);
        if (gridRadius < distanceFromStart)
        {
            gridRadius = distanceFromStart;
        }

        return cell;
    }


    public void RemoveCell(HexCell cell)
    {
        cells.Remove(cell);

        gridRadius = 0f;

        // Iterate through all cells to find new gridRadius
        foreach (HexCell activeCell in cells)
        {
            float distance = Vector3.Distance(Vector3.zero, activeCell.transform.position);

            if (gridRadius < distance)
            {
                gridRadius = distance;
            }
        }

        Destroy(cell.gameObject);
    }

    public void RemoveCellFromList(HexCell cell)
    {
        cells.Remove(cell);
    }


    protected int TriangleNumber(int i)
    {
        return (i*i + i) / 2;
    }


    public List<HexCell> GetCells()
    {
        return cells;
    }

    public HexCell GetCell(HexCoordinates searchCoordinates)
    {
        HexCell foundHexCell;

        foreach (HexCell activeCell in GetCells())
        {
            if (activeCell.coordinates.Equals(searchCoordinates))
            {
                foundHexCell = activeCell;

                return activeCell;
            }
        }

        return null;
    }

    public float GetGridRadius()
    {
        return gridRadius;
    }

    public void InstantiateTiles()
    {
        CreateCellsFromList(PlayersManager.Instance.SerializedHexCells);
    }

    public void CorruptRandomCells()
    {
        HexCell cellToCorrupt = null;
        int failSaveCounter = 0;

        int cellCount = 0;
        foreach (HexCell activeCell in cells)
        {
            if (activeCell.GetHeight() > 0)
            {
                cellCount++;
            }
        }

        int cellCorruptionAmount = Mathf.Max(cellCount / corruptionDivision, 1);
        int outerLoopCounter = 0;
        do
        {
            outerLoopCounter++;
            do
            {
                cellToCorrupt = cells[Random.Range(0, cells.Count - 1)];
                failSaveCounter++;
            } while ((cellToCorrupt == GetCell(Player.Instance.activeCellCoordinates) || cellToCorrupt.GetHeight() == 0 || cellToCorrupt == GetCell(startCellCoordinates) || cellToCorrupt.GetRoundsTillCorrupted() >= 0) && failSaveCounter <= 1000);

            if (failSaveCounter > 1000)
            {
                cellToCorrupt = null;
                Debug.LogWarning("Endlosschleife entkommen");
            }
            if (cellToCorrupt)
            {
                cellToCorrupt.CorruptCell(Random.Range(corruptionMinDuration, corruptionMaxDuration+1));
            }

        } while (outerLoopCounter < cellCorruptionAmount);

        

    }

    public void ReduceCorruptionTimer()
    {
        tempCells = new List<HexCell>(cells);
        foreach (HexCell activeCell in tempCells) {

            if (activeCell.GetRoundsTillCorrupted() > 0)
            {
                activeCell.CorruptCellWithoutRebuild(activeCell.GetRoundsTillCorrupted() - 1);
            }
        }
    }


    public void CreateCellsFromList(NetworkList<SerializedNetworkHex> newHexList)
    {
        // Step 1: Delete entire grid and clear List

        // Clear List
        GetCells().Clear();

        // Delete Grid
        foreach (Transform child in transform) {
            Destroy(child.gameObject);
        }

        gridRadius = float.NegativeInfinity;


        foreach (SerializedNetworkHex newHex in newHexList)
        {
            
            // 0 rausfiltern, da diese Tiles sowieso automatisch bei cell.AddTile() erzeugt werden
            // Step 2: Build new Cells
            HexCell cell = CreateCellFromHexCoordinate(new HexCoordinates(newHex.X, newHex.Z));
            cell.SetRoundsTillCorrupted(newHex.RoundsTillCorrupted);
            cell.cellBiome = newHex.Biome;
            

            if (newHex.PlayerActive)
            {
                Player.Instance.activeCellCoordinates = new HexCoordinates(newHex.X, newHex.Z);
            }

            // Step 3: Add Tiles 
            for (int i = 0; i < newHex.Height; i++)
            {

                if (i == newHex.Height-1)
                {
                    cell.AddTile(cell.cellBiome);
                } else
                {
                    cell.AddTile(Biome.GROUND);
                }
                
            }
        }

        tempCells = new List<HexCell>(cells);

        // Add Corruption
        foreach (HexCell activeCell in tempCells)
        {
            if (activeCell.GetRoundsTillCorrupted() >= 0)
            {
                activeCell.corruptionLabelPrefab.transform.parent.gameObject.SetActive(true);
                activeCell.CorruptCellForRebuild(activeCell.GetRoundsTillCorrupted());
            } else
            {
                activeCell.corruptionLabelPrefab.transform.parent.gameObject.SetActive(false);
            }
            
        }

        tempCells = new List<HexCell>(cells);


        // Add Tiles of Height 0
        foreach (HexCell activeCell in tempCells)
        {
            foreach (HexCoordinates hexCoordinate in activeCell.GenerateCellCoordinatesInRadius(1))
            {
                HexCell neighbour = GetCell(hexCoordinate);
                if (!neighbour)
                {
                    CreateCellFromHexCoordinate(hexCoordinate);
                }
            }
        }

        if (GetCell(startCellCoordinates).GetHeight() <= 0)
        {
            // Game Over
            PlayersManager.Instance.UpdateGameStateServerRpc(GameState.LOST);
        }

        if (GetCell(Player.Instance.activeCellCoordinates) && GetCell(Player.Instance.activeCellCoordinates).GetHeight() > 0)
        {
            // Place Player
            GetCell(Player.Instance.activeCellCoordinates).PlacePlayerForRebuild();

            // Calculate Preview Tiles
            if (PlayersManager.Instance.CurrentGameState == GameState.HUMANTURN)
            {
                // calculate preview Tiles
                GetCell(Player.Instance.activeCellCoordinates).CalculatePreviewTilesForHuman(true);
            }
        } else
        {
            // Game Over
            PlayersManager.Instance.UpdateGameStateServerRpc(GameState.LOST);
        }

        blockActions = false;
        
    }

    HexCell CreateCellFromHexCoordinate(HexCoordinates hexCoordinate)
    {
        Vector3 offsetCoordinates = HexCoordinates.ToOffsetCoordinates(hexCoordinate);

        return CreateCell((int)offsetCoordinates.x, (int)offsetCoordinates.z);
    }

    public HexCoordinates GetStartCellCoordiantes()
    {
        return startCellCoordinates;
    }
}