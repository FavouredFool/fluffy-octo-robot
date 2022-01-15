using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TilePrefab : MonoBehaviour
{
    protected void Awake()
    {
        transform.localScale = new Vector3(HexMetrics.outerRadius, HexMetrics.outerRadius, HexMetrics.outerRadius);
        transform.localRotation = Quaternion.Euler(new Vector3(0, Random.Range(0, 5) * 60, 0));
    }
}
