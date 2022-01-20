using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MenuHexCell : MonoBehaviour
{

    public GameObject[] hexPrefabs;

    public GameObject hexPreviewPrefab;
    public GameObject hexCellPreviewPrefab;

    public TMP_Text cellLabelPrefab;

    public GameObject numberGrid;

    [HideInInspector]
    public HexCoordinates coordinates;

    [HideInInspector]
    public int height = 0;

    [HideInInspector]
    public Stack<GameObject> hexStack;


    MenuHexGrid menuHexGrid;

    GameObject hexPreviewObj = null;
    GameObject hexCellPreviewObj = null;

    Canvas gridCanvas;
    TMP_Text label;


    protected void Awake()
    {
        hexStack = new Stack<GameObject>();
        //hexGrid = transform.parent.GetComponent<HexGrid>();
        // Bei Awake kann noch nicht ueber das Parentobjekt gegangen werden
        menuHexGrid = GameObject.Find("MenuHexGrid").GetComponent<MenuHexGrid>();

        gridCanvas = GetComponentInChildren<Canvas>();

        // Add Preview Prefab
        hexCellPreviewObj = Instantiate(hexCellPreviewPrefab, transform.position + new Vector3(0f, height * HexMetrics.hexHeight, 0f), Quaternion.identity, transform);

    }

    protected void Start()
    {
        // Coordinate-Grids 
        DefineLabel();
    }


    protected void DefineLabel()
    {
        if (numberGrid.activeSelf)
        {
            label = GetComponentInChildren<TMP_Text>();
            label.text = coordinates.ToStringOnSeperateLines();
            label.color = Color.black;
        }
        
    }

    public void AddTile()
    {
        if (GetHeight() == 0)
        {
            Destroy(hexCellPreviewObj);
        }


        GameObject prefabToPlace;
        if (menuHexGrid.GetStartCellCoordiantes().Equals(coordinates))
        {
            prefabToPlace = hexPrefabs[0];
        } else
        {
            prefabToPlace = hexPrefabs[Random.Range(1, hexPrefabs.Length)];
        }

        // Tile in Stack auf korrekter Hoehe hinzufuegen
        hexStack.Push(Instantiate(prefabToPlace, transform.position + new Vector3(0f, height * HexMetrics.hexHeight, 0f), Quaternion.identity, transform));
        


        // Height des Konstrukts erh�hen
        SetHeight(height + 1);
    }



    public IEnumerable GenerateCellCoordinatesInRadius(int radius)
    {
        // Step 1: Find all surrounding Hexes

        int centerX = coordinates.X;
        int centerZ = coordinates.Z;

        for (int r = 0, z = centerZ - radius; z <= centerZ; z++, r++)
        {
            for (int x = centerX - r; x <= centerX + radius; x++)
            {
                yield return (new HexCoordinates(x, z));
            }
        }
        for (int r = 0, z = centerZ + radius; z > centerZ; z--, r++)
        {
            for (int x = centerX - radius; x <= centerX + r; x++)
            {
                yield return (new HexCoordinates(x, z));
            }
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

    public int GetHeight()
    {
        return height;
    }


    protected void SetHeight(int newHeight)
    {
        // Hoehe aendern:
        height = newHeight;

        // Change CanvasPosition
        if (numberGrid.activeSelf)
        {
            gridCanvas.transform.localPosition = new Vector3(0f, (newHeight - 1 / 2f) * HexMetrics.hexHeight, 0f);
        }
        
    }





}
