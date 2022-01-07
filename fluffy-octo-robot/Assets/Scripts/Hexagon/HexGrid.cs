using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class HexGrid : NetworkBehaviour
{

    public int size = 5;

    public HexCell cellPrefab;

    List<HexCell> cells;

    HexCoordinates startCellCoordinates = new HexCoordinates(0, 0);

    float gridRadius = 0f;

    List<HexCell> tempCells;

    public GameObject playerPrefab;


    protected void Awake()
    {

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

    public void ReformWorld()
    {
        // send cells to Networking
        PlayersManager.Instance.SerializeHexCells(cells);
        InstantiateTiles();
    }

    private void Update()
    {
        /*
        if (PlayersManager.Instance.SerializedHexCellSize > 0 && !initialLoad)
        {
            InitialSpawnTile();
        }
        */
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

    /*
    // This function would be called (is the host)
    [ServerRpc(RequireOwnership = false)]
    public void InitialSpawnTileServerRPC()
    {
        InitialSpawnTileClientRPC();
    }

    // This function do anything on every Client when its called
    [ClientRpc]
    public void InitialSpawnTileClientRPC()
    {
        InstantiateTiles();
    }
    */

    public void InstantiateTiles()
    {
        CreateCellsFromList(PlayersManager.Instance.SerializedHexCells);
    }

    /*
    private List<HexCell> ConvertNetworkListToTileList(NetworkList<SerializedNetworkHex> networkList)
    {
        List<HexCell> tempTileList = new();

        foreach (SerializedNetworkHex serializedNetworkHex in networkList)
        {
            tempTileList.Add(new HexCell(new HexCoordinates(serializedNetworkHex.X, serializedNetworkHex.Z), serializedNetworkHex.Height));
        }

        return tempTileList;
    }*/

    public void CreateCellsFromList(NetworkList<SerializedNetworkHex> newTileList)
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




        foreach (SerializedNetworkHex newTile in newTileList)
        {
            // 0 rausfiltern, da diese Tiles sowieso automatisch bei cell.AddTile() erzeugt werden
            if (newTile.Height != 0)
            {

                // Step 2: Build new Cells
                HexCell cell = CreateCellFromHexCoordinate(new HexCoordinates(newTile.X, newTile.Z));

                if (newTile.PlayerActive)
                {
                    Player.Instance.activeCellCoordinates = new HexCoordinates(newTile.X, newTile.Z);
                }

                // Step 3: Add Tiles 
                for (int i = 0; i < newTile.Height; i++)
                {
                    cell.AddTile();
                }
                
            }
        }

        // Add Tiles of Height 0

        tempCells = new List<HexCell>(cells);
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