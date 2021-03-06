using FluffyRobot.Core.Singeltons;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using System.Collections;
using System;

public enum Role
{
    HUMAN, GOD, SINGLE
}

public class PlayersManager : Singelton<PlayersManager> {

    private NetworkVariable<int> playersInGame = new NetworkVariable<int>();

    [SerializeField]
    private NetworkVariable<int> gridVersion = new NetworkVariable<int>(0);

    [SerializeField]
    private NetworkVariable<GameState> currentGameState = new NetworkVariable<GameState>(GameState.START);

    [SerializeField]
    private NetworkList<SerializedNetworkHex> hexCellsSerialized;

    private Role role;

    HexGrid hexGrid;

    private int bufferOverloadPrevention = 5;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void LoadMain()
    {
        Debug.Log(Resources.Load("Manager/PlayersManager"));
        PlayersManager main = GameObject.Instantiate(Resources.Load<PlayersManager>("Manager/PlayersManager"));
        GameObject.DontDestroyOnLoad(main);
    }

    private void Awake()
    {
        //DontDestroyOnLoad(this);

        hexCellsSerialized = new NetworkList<SerializedNetworkHex>();
    }

    public int PlayersInGame
    {
        get {
            return playersInGame.Value;
        }
    }

    public NetworkList<SerializedNetworkHex> SerializedHexCells
    {
        get
        {
            return hexCellsSerialized;
        }
    }

    public GameState CurrentGameState
    {
        get
        {
            return currentGameState.Value;
        }
    }

    public int CurrentGridVersion
    {
        get
        {
            return gridVersion.Value;
        }
    }

    public bool IsHuman
    {
        get
        {
            return role.Equals(Role.HUMAN);
        }
    }

    public bool IsGod
    {
        get
        {
            return role.Equals(Role.GOD);
        }
    }

    public bool IsSingle
    {
        get
        {
            return role.Equals(Role.SINGLE);
        }
    }

    private void Start() {

        NetworkManager.Singleton.OnClientConnectedCallback += (id) => {
            if (IsServer) {
                playersInGame.Value++;

                Debug.Log("Player Added");
                Debug.Log(playersInGame.Value);

                if (playersInGame.Value == 2)
                {
                    hexGrid = FindObjectOfType<HexGrid>();
                    hexGrid.ReformWorld();
                }
            }
        };

        NetworkManager.Singleton.OnClientDisconnectCallback += (id) => {
            if (IsServer)
            {
                playersInGame.Value = 0;

                Debug.Log("Player Removed");
            }
        };
    }

    public IEnumerator SerializeAndUpdateHexCells(List<HexCell> hexCells)
    {
        hexGrid = FindObjectOfType<HexGrid>();

        
        bool playerActive;

        SerializeClearHexCellListServerRpc();

        int counter = 0;
        // Convert to serialized List
        foreach (HexCell activeCell in hexCells)
        {
            // Hexes that dont hold tiles can be recreated locally
            if (activeCell.GetHeight() > 0)
            {
                counter++;
                if (counter % bufferOverloadPrevention == 0)
                {
                    yield return new WaitForEndOfFrame();
                }
                if (hexGrid.GetCell(Player.Instance.activeCellCoordinates) == activeCell)
                    playerActive = true;
                else
                    playerActive = false;



                SerializeAndUpdateHexCellsServerRpc(new SerializedNetworkHex(activeCell.coordinates.X, activeCell.coordinates.Z, activeCell.GetHeight(), playerActive, activeCell.GetRoundsTillCorrupted(), activeCell.cellBiome, activeCell.collectableActive));
            }
        }

        UpdateGridVersionServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    public void IncreasePlayersServerRpc()
    {
        Debug.Log("should remove player");
        if (IsServer)
        {
            playersInGame.Value = 0;

            Debug.Log("Player Removed");
        }
    }

    public void SetRole(Role role)
    {
        this.role = role;
    }

    public void UpdateGameState(GameState newGamestate)
    {
        currentGameState.Value = newGamestate;
    }

    [ServerRpc(RequireOwnership = false)]
    public void UpdateGameStateServerRpc(GameState newGamestate)
    {
        currentGameState.Value = newGamestate;
    }

    [ServerRpc(RequireOwnership = false)]
    public void UpdateGridVersionServerRpc()
    {
        gridVersion.Value++;
    }

    [ServerRpc(RequireOwnership = false)]
    public void SerializeAndUpdateHexCellsServerRpc(SerializedNetworkHex hex)
    {
       hexCellsSerialized.Add(hex);
    }

    [ServerRpc(RequireOwnership = false)]
    public void SerializeClearHexCellListServerRpc()
    {
        hexCellsSerialized.Clear();
    }
}