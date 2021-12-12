using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
    

public class HexCell : MonoBehaviour
{

    public Color color;

    public GameObject hexPrefab;
    public TMP_Text cellLabelPrefab;

    [HideInInspector]
    public int height = 0;

    [HideInInspector]
    public List<GameObject> hexStack;

    Canvas gridCanvas;
    TMP_Text label;

    public HexCoordinates coordinates;

    

    protected void Start()
    {
        // Coordinate-Grids 
        gridCanvas = GetComponentInChildren<Canvas>();
        DefineLabel();

        hexStack = new List<GameObject>();

        AddTile();


    }

    protected void DefineLabel()
    {
        label = GetComponentInChildren<TMP_Text>();
        label.text = coordinates.ToStringOnSeperateLines();
        label.color = Color.black;
    }

    protected void AddTile()
    {
        // Tile in Stack auf korrekter Höhe hinzufügen
        hexStack.Add(Instantiate(hexPrefab, transform.position + new Vector3(0f, height * HexMetrics.hexHeight, 0f), Quaternion.identity, transform));
        hexStack[0].transform.SetParent(transform, false);

        // Height des Konstrukts erhöhen
        SetHeight(height + 1);
    }

    protected void SetHeight(int newHeight)
    {
        // Höhe ändern:
        height = newHeight;

        // Change CanvasPosition
        gridCanvas.transform.localPosition = new Vector3(0f, newHeight * HexMetrics.hexHeight, 0f);
    }

}
