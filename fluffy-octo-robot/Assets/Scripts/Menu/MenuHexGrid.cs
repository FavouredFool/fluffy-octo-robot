using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using Unity.Netcode;

public class MenuHexGrid : NetworkBehaviour
{
    public float rotationSpeed = 0.2f;
    

    public int size = 5;

    public MenuHexCell cellPrefab;

    List<MenuHexCell> cells;

    HexCoordinates startCellCoordinates = new HexCoordinates(0, 0);

    float gridRadius = 0f;


    protected void Awake()
    {

        // initiale Capacity bereitstellen
        cells = new(TriangleNumber(size) + TriangleNumber(size - 1) - 2 * TriangleNumber(size / 2));

        // Terraingeneration
        MenuHexCell startCell = CreateCellFromHexCoordinate(startCellCoordinates);

        foreach (HexCoordinates coordinates in startCell.GenerateCellCoordinatesInRadius(size / 2))
        {
            if (!coordinates.Equals(startCellCoordinates))
            {
                Vector3 offsetCoordinates = HexCoordinates.ToOffsetCoordinates(coordinates);

                CreateCell((int)offsetCoordinates.x, (int)offsetCoordinates.z);
            }


        }

        List<MenuHexCell> startCells = new List<MenuHexCell>(cells);
        foreach (MenuHexCell activeCell in startCells)
        {

            for (int i = 0; i < Random.Range(1, 5); i++)
            {
                activeCell.AddTile();
            }
        }

        StartCoroutine(StartMenu());
    }

    IEnumerator StartMenu()
    {
        yield return new WaitForSeconds(1f);

        // Hier könnte die Insel immer wieder ein wenig umgebaut werden

        /*
        while (true)
        {
            yield return new WaitForSeconds(1f);


        }
        */

    }


    private void Update()
    {
        // Rotate World
        transform.Rotate(new Vector3(0, rotationSpeed, 0));

    }

    public MenuHexCell CreateCell(int x, int z)
    {
        Vector3 position;
        position.x = (x + z / 2f - z / 2) * (HexMetrics.innerRadius * 2f);
        position.y = 0f;
        position.z = z * (HexMetrics.outerRadius * 1.5f);

        Debug.Log(cellPrefab);
        MenuHexCell cell = Instantiate(cellPrefab);
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



    protected int TriangleNumber(int i)
    {
        return (i * i + i) / 2;
    }


    public List<MenuHexCell> GetCells()
    {
        return cells;
    }

    public MenuHexCell GetCell(HexCoordinates searchCoordinates)
    {

        MenuHexCell foundHexCell;

        foreach (MenuHexCell activeCell in GetCells())
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

    MenuHexCell CreateCellFromHexCoordinate(HexCoordinates hexCoordinate)
    {
        Vector3 offsetCoordinates = HexCoordinates.ToOffsetCoordinates(hexCoordinate);
        return CreateCell((int)offsetCoordinates.x, (int)offsetCoordinates.z);
    }

    public HexCoordinates GetStartCellCoordiantes()
    {
        return startCellCoordinates;
    }
}