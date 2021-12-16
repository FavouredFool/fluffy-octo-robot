using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using System;

public class HexGrid : MonoBehaviour
{

    public int size = 5;

    public HexCell cellPrefab;
    public Player playerPrefab;

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
        List<HexCell> initialCellsList = new(cells);

        // Very simple Mapgeneration
        foreach (HexCell cell in initialCellsList)
        {
            for (int i = 0; i < UnityEngine.Random.Range(1,4); i++)
            {
                cell.AddTile();
            }
        }

        // Create Player on Map
        Player player = Instantiate(playerPrefab, transform);
        startCell.PlacePlayer(player);
    }

    protected void Update()
    {

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

        // gridRadius ggf. abändern
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

}


// Alter Code:

/**
 int line;

 if (size % 2 == 0)
     Debug.LogWarning("FEHLER: Keine ungerade Höhe mitgegeben");
 else
 {
     Func<float, int> floor_ceil_1;
     Func<float, int> floor_ceil_2;
     Func<float, int> floor_ceil_3;
     Func<float, int> floor_ceil_4;

     // Schleife pro Zeile
     for (int z = -size / 2, i = 0, lineCounter = 0; z <= size / 2; z++, lineCounter++)
     {

         // Lines umschreiben, um Hexagon nach oben (über der Mitte) wieder dünner werden zu lassen
         if (lineCounter > size / 2)
             line = size - 1 - lineCounter;
         else
             line = lineCounter;


         // Absolut schrecklicher Code aber ich habe keinen Plan wie man das mathematisch zusammenfasst. Es funktioniert!

         // Mathematisch prüfen, ob die Zeile eine gerade Menge Tiles hat
         if (((size - 1) % 4 == 0 && line % 2 == 0) || ((size - 1) % 4 != 0 && line % 2 != 0))
         {
             floor_ceil_1 = Mathf.CeilToInt;
             floor_ceil_2 = Mathf.CeilToInt;
             floor_ceil_3 = Mathf.FloorToInt;
             floor_ceil_4 = Mathf.CeilToInt;
         }
         else
         { 
             // Verzweigung: Ist die Reihe unter oder über der Mitte?
             if (lineCounter <= (int)size / 2)
             {
                 // Verzweigung: Ist das Hexagon von der Size: 1, 5, 9 [...] oder 3, 7, 11 [...]?
                 if ((size - 1) % 4 == 0)
                 {
                     floor_ceil_1 = Mathf.CeilToInt;
                     floor_ceil_2 = Mathf.FloorToInt;
                     floor_ceil_3 = Mathf.FloorToInt;
                     floor_ceil_4 = Mathf.CeilToInt;
                 }
                 else
                 {
                     floor_ceil_1 = Mathf.CeilToInt;
                     floor_ceil_2 = Mathf.FloorToInt;
                     floor_ceil_3 = Mathf.CeilToInt;
                     floor_ceil_4 = Mathf.CeilToInt;
                 }
             }
             else
             {
                 // Verzweigung: Ist das Hexagon von der Size: 1, 5, 9 [...] oder 3, 7, 11 [...]?
                 if ((size - 1) % 4 == 0)
                 {
                     floor_ceil_1 = Mathf.FloorToInt;
                     floor_ceil_2 = Mathf.FloorToInt;
                     floor_ceil_3 = Mathf.FloorToInt;
                     floor_ceil_4 = Mathf.FloorToInt;
                 } else
                 {
                     floor_ceil_1 = Mathf.FloorToInt;
                     floor_ceil_2 = Mathf.FloorToInt;
                     floor_ceil_3 = Mathf.FloorToInt;
                     floor_ceil_4 = Mathf.CeilToInt;
                 }
             }
         }

         // Die Methode, die die Cells erstellt. floor_ceil kann entweder CeilToInt oder FloorToInt sein und wird je nach Spalte - wie oben zu sehen - bestimmt.
         for (int x = floor_ceil_1(-size / 4f) - floor_ceil_2(line / 2f); x <= floor_ceil_3(size / 4f) + floor_ceil_4(line / 2f); x++)
             CreateCell(x, z);
     }
 }
 */