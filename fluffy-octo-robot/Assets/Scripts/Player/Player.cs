using Unity.Netcode;
using UnityEngine;

public class Player : NetworkBehaviour
{
    private static Player instance;

    public static Player Instance { get { return instance; } }

    [HideInInspector]
    public HexCoordinates activeCellCoordinates;

    [HideInInspector]
    public int maxWalkHeight;

    private void Awake()
    {
        maxWalkHeight = 1;

        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        } else
        {
            instance = this;
        }
    }
}
