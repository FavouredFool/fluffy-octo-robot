using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexPrefab : MonoBehaviour
{
    protected void Awake()
    {
        transform.localScale = new Vector3(HexMetrics.outerRadius, HexMetrics.outerRadius, HexMetrics.outerRadius);
    }
}
