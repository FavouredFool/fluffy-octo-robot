using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class HexGrid : NetworkBehaviour
{
    private int currentGridVersion;

    public int size = 5;

    public HexCell cellPrefab;

    public GameObject playerPrefab;

    public int corruptionDuration;

    public int corruptionDivision;

    List<HexCell> cells;

    HexCoordinates startCellCoordinates = new HexCoordinates(0, 0);

    float gridRadius = 0f;

    List<HexCell> tempCells;

    
    

    private void Start()
    {
        currentGridVersion = PlayersManager.Instance.CurrentGridVersion;

        Debug.Log(PlayersManager.Instance.CurrentGridVersion);
        Debug.Log(IsHost);
        Debug.Log(PlayersManager.Instance.SerializedHexCells.Count);

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
            activeCell.AddTile();
        }

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
        PlayersManager.Instance.SerializeAndUpdateHexCells(cells);
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
        int cellCorruptionAmount = Mathf.Max(GetCells().Count / corruptionDivision, 1);
        int outerLoopCounter = 0;
        do
        {
            outerLoopCounter++;
            do
            {
                cellToCorrupt = cells[UnityEngine.Random.Range(0, cells.Count - 1)];
                failSaveCounter++;
            } while ((cellToCorrupt == GetCell(Player.Instance.activeCellCoordinates) || cellToCorrupt.GetHeight() == 0 || cellToCorrupt == GetCell(startCellCoordinates) || cellToCorrupt.GetRoundsTillCorrupted() >= 0) && failSaveCounter <= 1000);

            if (failSaveCounter > 1000)
            {
                cellToCorrupt = null;
                Debug.LogWarning("Endlosschleife entkommen");
            }
            if (cellToCorrupt)
            {
                cellToCorrupt.CorruptCell(corruptionDuration);
            }

        } while (outerLoopCounter < cellCorruptionAmount);

        ReformWorld();

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

        ReformWorld();
    }


    public void CreateCellsFromList(NetworkList<SerializedNetworkHex> newHexList)
    {
        Debug.Log("HexGrid neu bauen");

        // Step 1: Delete entire grid and clear List

        // Delete Grid
        foreach (Transform child in transform) {
            Destroy(child.gameObject);
        }

        // Clear List
        GetCells().Clear();

        gridRadius = float.NegativeInfinity;

        foreach (SerializedNetworkHex newHex in newHexList)
        {
            // 0 rausfiltern, da diese Tiles sowieso automatisch bei cell.AddTile() erzeugt werden
            if (newHex.Height != 0)
            {
                // Step 2: Build new Cells
                HexCell cell = CreateCellFromHexCoordinate(new HexCoordinates(newHex.X, newHex.Z));
                cell.SetRoundsTillCorrupted(newHex.RoundsTillCorrupted);

                if (newHex.PlayerActive)
                {
                    Player.Instance.activeCellCoordinates = new HexCoordinates(newHex.X, newHex.Z);
                }

                // Step 3: Add Tiles 
                for (int i = 0; i < newHex.Height; i++)
                {
                    cell.AddTile();
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

        // Place Player
        GetCell(Player.Instance.activeCellCoordinates).PlacePlayerForRebuild();

        // Calculate Preview Tiles
        if (PlayersManager.Instance.CurrentGameState == GameState.HUMANTURN)
        {
            // calculate preview Tiles
            GetCell(Player.Instance.activeCellCoordinates).CalculatePreviewTilesForHuman(true);
        } 
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