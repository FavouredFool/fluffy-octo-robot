using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class HexGrid : NetworkBehaviour
{

    public int size = 5;

    public HexCell cellPrefab;

    // Add serialazation
    // NetworkList<HexCell> hexCells = new NetworkList<HexCell>();

    List<HexCell> cells;

    HexCell startCell;

    float gridRadius = 0f;


    protected void Awake()
    {
        
        // initiale Capacity bereitstellen
        cells = new(TriangleNumber(size) + TriangleNumber(size - 1) - 2 * TriangleNumber(size / 2));

        // Terraingeneration
        startCell = CreateCell(0, 0);

        foreach (HexCoordinates coordinates in startCell.GenerateCellCoordinatesInRadius(size/2))
        {
            if (!coordinates.Equals(startCell.coordinates))
            {
                Vector3 offsetCoordinates = HexCoordinates.ToOffsetCoordinates(coordinates);

                CreateCell((int) offsetCoordinates.x, (int) offsetCoordinates.z);
            }
        }
        
    }

    protected void Start()
    {
        /*
        List<HexCell> initialCellsList = new(cells);

        // Very simple Mapgeneration
        foreach (HexCell cell in initialCellsList)
        {
            for (int i = 0; i < UnityEngine.Random.Range(1,4); i++)
            {
                cell.AddTile();
            }
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

    // This function would be called (is the host)
    [ServerRpc]
    public void SpawnTileServerRPC()
    {
        SpawnTileClientRPC();
    }

    // This function do anything on every Client when its called
    [ClientRpc]
    private void SpawnTileClientRPC()
    {
        Debug.Log("All Clients do a debug log");

        List<HexCell> initialCellsList = new(cells);

        // Very simple Mapgeneration
        foreach (HexCell cell in initialCellsList)
        {
            for (int i = 0; i < Random.Range(1, 4); i++)
            {
                cell.AddTile();
            }
        }
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

        foreach (SerializedTile newTile in newTileList)
        {
            // 0 rausfiltern, da diese Tiles sowieso automatisch bei cell.AddTile() erzeugt werden
            if (newTile.GetHeight() != 0)
            {
                Vector3 offsetCoordinates = HexCoordinates.ToOffsetCoordinates(newTile.GetCoordinates());

                // Step 2: Build new Cells
                HexCell cell = CreateCell((int)offsetCoordinates.x, (int)offsetCoordinates.z);

                // Step 3: Add Tiles 
                for (int i = 0; i < newTile.GetHeight(); i++)
                {
                    cell.AddTile();
                }
            }
        }
    }
}