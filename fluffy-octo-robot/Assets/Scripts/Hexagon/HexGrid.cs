using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class HexGrid : NetworkBehaviour
{

    public int size = 5;

    public HexCell cellPrefab;

    List<HexCell> cells;

    HexCell startCell;

    float gridRadius = 0f;

    List<HexCell> tempCells;


    protected void Awake()
    {

        // initiale Capacity bereitstellen
        cells = new(TriangleNumber(size) + TriangleNumber(size - 1) - 2 * TriangleNumber(size / 2));

        // Terraingeneration
        startCell = CreateCell(0, 0);

        foreach (HexCoordinates coordinates in startCell.GenerateCellCoordinatesInRadius(size / 2))
        {
            if (!coordinates.Equals(startCell.coordinates))
            {
                Vector3 offsetCoordinates = HexCoordinates.ToOffsetCoordinates(coordinates);

                CreateCell((int)offsetCoordinates.x, (int)offsetCoordinates.z);
            }
        }

        List<HexCell> startCells = new List<HexCell>(cells);
        foreach (HexCell activeCell in startCells)
        {
            activeCell.AddTile();
            activeCell.AddTile();
        }

        ReformWorld();
        
    }

    public void ReformWorld()
    {
        // send cells to Networking
        PlayersManager.Instance.UpdateHexCellsSerialized(cells);
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
        CreateCellsFromList(ConvertNetworkListToTileList(PlayersManager.Instance.SerializedHexCells));
    }


    private List<SerializedTile> ConvertNetworkListToTileList(NetworkList<SerializedNetworkHex> networkList)
    {
        List<SerializedTile> tempTileList = new();

        foreach (SerializedNetworkHex serializedNetworkHex in networkList)
        {
            tempTileList.Add(new SerializedTile(new HexCoordinates(serializedNetworkHex.X, serializedNetworkHex.Z), serializedNetworkHex.Height));
        }

        return tempTileList;
    }

    public void CreateCellsFromList(List<SerializedTile> newTileList)
    {
        Debug.Log("HexGrid neu bauen");

        // Step 1: Delete entire grid and clear List

        // Delete Grid
        foreach (Transform child in transform) {
            Destroy(child.gameObject);
        }

        // Clear List
        GetCells().Clear();
        Debug.Log(newTileList.Count);

        gridRadius = float.NegativeInfinity;



        foreach (SerializedTile newTile in newTileList)
        {
            // 0 rausfiltern, da diese Tiles sowieso automatisch bei cell.AddTile() erzeugt werden
            if (newTile.GetHeight() != 0)
            {

                // Step 2: Build new Cells
                HexCell cell = CreateCellFromHexCoordinate(newTile.GetCoordinates());

                // Step 3: Add Tiles 

                for (int i = 0; i < newTile.GetHeight(); i++)
                {
                    cell.AddTile();
                }
                
            }
        }
        Debug.Log(cells.Count);

        // Add Tiles of Height 0

        tempCells = new List<HexCell>(cells);
        foreach (HexCell activeCell in tempCells)
        {
            foreach (HexCoordinates hexCoordinate in activeCell.GenerateCellCoordinatesInRadius(1))
            {
                HexCell neighbour = GetCell(hexCoordinate);
                if (!neighbour)
                {
                    HexCell cell = CreateCellFromHexCoordinate(hexCoordinate);
                }
            }
        }
        
        


    }

    HexCell CreateCellFromHexCoordinate(HexCoordinates hexCoordinate)
    {
        Vector3 offsetCoordinates = HexCoordinates.ToOffsetCoordinates(hexCoordinate);
        return CreateCell((int)offsetCoordinates.x, (int)offsetCoordinates.z);
    }
}