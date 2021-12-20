using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorruptionButton : MonoBehaviour
{
    public HexGrid hexGrid;
    public void DestroyStack()
    {
        hexGrid.CorruptRandomCell();
    }
}
