using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private static Player instance;

    public static Player Instance { get { return instance; } }

    [HideInInspector]
    public HexCell activeCell = null;

    [HideInInspector]
    public int maxWalkHeight;


    private void Awake()
    {
        maxWalkHeight = 1;


        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        } else
        {
            instance = this;
        }
    }

   
    

}
