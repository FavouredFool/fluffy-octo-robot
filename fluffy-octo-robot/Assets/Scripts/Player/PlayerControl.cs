using Unity.Netcode;
using UnityEngine;

public class PlayerControl : NetworkBehaviour
{

    [HideInInspector]
    public HexCell activeCell = null;

    [HideInInspector]
    public int maxWalkHeight;

    private void Awake()
    {
        maxWalkHeight = 1;
    }
}