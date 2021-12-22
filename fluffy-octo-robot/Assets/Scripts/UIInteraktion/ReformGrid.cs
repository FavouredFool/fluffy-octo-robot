using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReformGrid : MonoBehaviour
{

    public HexGrid hexGrid;

    public void SimulateGridReform()
    {
        List<SerializedTile> tempTileList = new List<SerializedTile> {
            new SerializedTile(new HexCoordinates(0, 0), 2),
            new SerializedTile(new HexCoordinates(1, 0), 1),
            new SerializedTile(new HexCoordinates(0, 1), 3),
            new SerializedTile(new HexCoordinates(1, 1), 1),
            new SerializedTile(new HexCoordinates(-2, -2), 5)
        };
            


        hexGrid.CreateCellsFromList(tempTileList);
    }
}
