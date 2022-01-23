using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Test : NetworkBehaviour
{
    public void disconnect()
    {
        Debug.Log("disconnect");
        NetworkManager.Singleton.Shutdown();
    }
}
