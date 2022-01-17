using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TilePrefab : MonoBehaviour
{
    protected void Awake()
    {
        transform.localScale = new Vector3(HexMetrics.outerRadius, HexMetrics.outerRadius, HexMetrics.outerRadius);

        // Create random rotation that is always the same based on position
        transform.localRotation = Quaternion.Euler(new Vector3(0, ((int)(transform.position.x * transform.position.z * 2048) % 5) * 60, 0));

        // Calculate Rotation based on Position
    }
}
