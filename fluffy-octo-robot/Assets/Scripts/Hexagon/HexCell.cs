using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
    

public class HexCell : MonoBehaviour
{

    public GameObject hexPrefab;
    public GameObject hexPreviewPrefab;

    public TMP_Text cellLabelPrefab;

    [HideInInspector]
    public int height = 0;

    [HideInInspector]
    public List<GameObject> hexStack;

    GameObject hexPreviewObj;

    Canvas gridCanvas;
    TMP_Text label;

    public HexCoordinates coordinates;

    float showTilePreviewDuration = 0f;
    float showTilePreviewStarttime = float.NegativeInfinity;

    

    protected void Start()
    {
        // Coordinate-Grids 
        gridCanvas = GetComponentInChildren<Canvas>();
        DefineLabel();

        hexStack = new List<GameObject>();

        AddTile();

    }

    protected void Update()
    {
        
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

        // Height des Konstrukts erhöhen
        SetHeight(height + 1);
    }

    public void ShowTilePreview(bool active)
    {
        // Activate / Deactivate Tilepreview of a Tile
        if (active)
        {
            if (!hexPreviewObj)
            {
                hexPreviewObj = Instantiate(hexPreviewPrefab, transform.position + new Vector3(0f, height * HexMetrics.hexHeight, 0f), Quaternion.identity, transform);
            }
            
        } else
        {
            if (hexPreviewObj)
            {
                Destroy(hexPreviewObj);
                hexPreviewObj = null;
            }
        }

    }


    protected void SetHeight(int newHeight)
    {
        // Höhe ändern:
        height = newHeight;

        // Change CanvasPosition
        gridCanvas.transform.localPosition = new Vector3(0f, newHeight * HexMetrics.hexHeight, 0f);
    }

}
