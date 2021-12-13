using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
    

public class HexCell : MonoBehaviour
{

    public GameObject hexPrefab;
    public GameObject hexPreviewPrefab;
    public GameObject hexCellPreviewPrefab;

    public TMP_Text cellLabelPrefab;
    public HexCoordinates coordinates;

    [HideInInspector]
    public int height = 0;

    [HideInInspector]
    public List<GameObject> hexStack;

    GameObject hexPreviewObj = null;
    GameObject hexCellPreviewObj = null;

    Canvas gridCanvas;
    TMP_Text label;

    bool propagating;



    protected void Start()
    {
        // Coordinate-Grids 
        gridCanvas = GetComponentInChildren<Canvas>();
        DefineLabel();

        hexStack = new List<GameObject>();
        /*
        for (int i = 0; i < Random.Range(1, 5); i++)
        {
            AddTile();
        }*/

        if (!propagating)
        {
            // Add Preview Prefab
            hexCellPreviewObj = Instantiate(hexCellPreviewPrefab, transform.position + new Vector3(0f, height * HexMetrics.hexHeight, 0f), Quaternion.identity, transform);
        }
        

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

    public void AddTile()
    {
        // Tile in Stack auf korrekter Höhe hinzufügen
        hexStack.Add(Instantiate(hexPrefab, transform.position + new Vector3(0f, height * HexMetrics.hexHeight, 0f), Quaternion.identity, transform));

        // Height des Konstrukts erhöhen
        SetHeight(height + 1);

        // Wenn vorher eine Tilepreview auf der Cell gezeigt wurde, soll diese geupdated werden
        if (hexPreviewObj)
        {
            ShowTilePreview(true);
        }

        // Propagating-Boolean abändern wenn nötig
        propagating = true;
        UpdatePropagating();

        
    }

    void UpdatePropagating()
    {
        // Propagating-Boolean abändern wenn nötig

        if (height == 0 && propagating)
        {
            propagating = false;
            hexCellPreviewObj = Instantiate(hexCellPreviewPrefab, transform.position + new Vector3(0f, height * HexMetrics.hexHeight, 0f), Quaternion.identity, transform);
        } else if (height != 0 && !propagating)
        {
            propagating = true;
            Destroy(hexCellPreviewObj);
            hexCellPreviewObj = null;

        }
    }


    public void ShowTilePreview(bool active)
    {
        // Activate / Deactivate Tilepreview of a Tile
        if (active)
        {
            if (!hexPreviewObj)
            {
                hexPreviewObj = Instantiate(hexPreviewPrefab, transform.position + new Vector3(0f, height * HexMetrics.hexHeight, 0f), Quaternion.identity, transform);
            } else
            {
                // Falls ShowTilePreview aufgerufen wird obwohl das hex schon exitiert, update die Hexposition
                ShowTilePreview(false);
                ShowTilePreview(true);
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
        gridCanvas.transform.localPosition = new Vector3(0f, (newHeight - 1/2f) * HexMetrics.hexHeight, 0f);
    }

}
