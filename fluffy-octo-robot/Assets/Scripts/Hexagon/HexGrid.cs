using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class HexGrid : MonoBehaviour
{
    public int width = 5;
    public int height = 5;

    public HexCell cellPrefab;

    HexCell[] cells;

    

    private void Awake()
    {
        cells = new HexCell[1000];
        int middle;
        int line;

        for (int z = -height / 2, i = 0, lineCounter = 0; z <= height / 2; z++, lineCounter++)
        {
            if (lineCounter > (int)(height/2))
            {
                line = height-1 - lineCounter;
            }
            else
            {
                line = lineCounter;
            }
            Debug.Log(line);

            if (height % 2 != 0)
            {
                // Berechnung ob die aktive Zeile eine gerade oder ungerade Menge an Tiles hat
                if (((height - 1) % 4 == 0 && line % 2 == 0) || ((height -1) % 4 != 0 && line % 2 != 0))
                {
                    middle = 0;
                } else
                {
                        if ((line - (int)(height / 2f) + 1) % 4 == 0)
                        {
                            middle = -1;
                        } else
                        {
                            middle = 1;
                        }
                }
                    
                if (middle == 0)
                {
                    for (int x = Mathf.CeilToInt(-height / 4f) - Mathf.CeilToInt(line / 2f); x <= Mathf.FloorToInt(height / 4f) + Mathf.CeilToInt(line/2f); x++)
                    {
                        CreateCell(x, z, i++);
                    }
                } else
                {

                    if (lineCounter <= (int)height / 2)
                    {
                        if ((height - 1) % 4 == 0)
                        {

                            for (int x = Mathf.CeilToInt(-height / 4f) - Mathf.FloorToInt(line / 2f); x <= Mathf.CeilToInt(height / 4f) + Mathf.FloorToInt(line / 2f); x++)
                            {
                                CreateCell(x, z, i++);
                            }
                        }
                        else
                        {
                            for (int x = Mathf.CeilToInt(-height / 4f) - Mathf.FloorToInt(line / 2f); x <= Mathf.CeilToInt(height / 4f) + Mathf.CeilToInt(line / 2f); x++)
                            {
                                CreateCell(x, z, i++);
                            }
                        }
                    } else
                    {
                        if ((height - 1) % 4 == 0)
                        {

                            for (int x = Mathf.FloorToInt(-height / 4f) - Mathf.FloorToInt(line / 2f); x <= Mathf.FloorToInt(height / 4f) + Mathf.FloorToInt(line / 2f); x++)
                            {
                                CreateCell(x, z, i++);
                            }
                        }
                        else
                        {
                            for (int x = Mathf.FloorToInt(-height / 4f) - Mathf.FloorToInt(line / 2f); x <= Mathf.FloorToInt(height / 4f) + Mathf.CeilToInt(line / 2f); x++)
                            {
                                CreateCell(x, z, i++);
                            }
                        }
                    }

                    
                }


            } else
            {
                Debug.Log("FEHLER: KEINE UNGERADE HÖHE MITGEGEBEN");
            }
            


            
        }
    }

    


    void CreateCell(int x, int z, int i)
    {
        Vector3 position;
        position.x = (x + z * 0.5f - z / 2) * (HexMetrics.innerRadius * 2f);
        position.y = 0f;
        position.z = z * (HexMetrics.outerRadius * 1.5f);

        HexCell cell = cells[i] = Instantiate<HexCell>(cellPrefab);
        cell.GetComponent<HexCell>().coordinates = new Vector2(x, z);
        cell.transform.SetParent(transform, false);
        cell.transform.localPosition = position;
    }
}
