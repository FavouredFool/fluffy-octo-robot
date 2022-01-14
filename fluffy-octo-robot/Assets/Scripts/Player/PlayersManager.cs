using FluffyRobot.Core.Singeltons;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayersManager : Singelton<PlayersManager> {

    [SerializeField]
    private NetworkVariable<int> playersInGame = new NetworkVariable<int>();

    [SerializeField]
    private NetworkVariable<int> gridVersion = new NetworkVariable<int>(0);

    [SerializeField]
    private NetworkVariable<GameState> currentGameState = new NetworkVariable<GameState>(GameState.START);

    [SerializeField]
    private NetworkList<SerializedNetworkHex> hexCellsSerialized;

    private void Awake()
    {
        DontDestroyOnLoad(this);

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

    private void Start() {

        NetworkManager.Singleton.OnClientConnectedCallback += (id) => {

            if(IsServer) {

                playersInGame.Value++;

                Debug.Log("Player Added");

            }
        };

        NetworkManager.Singleton.OnClientDisconnectCallback += (id) => {
            if (IsServer) {
                playersInGame.Value--;

                Debug.Log("Player Removed");
            }
        };
    }

    public void SerializeAndUpdateHexCells(List<HexCell> hexCells)
    {
        HexGrid hexGrid = FindObjectOfType<HexGrid>();
        bool playerActive;

        SerializeClearHexCellListServerRpc();

        // Convert to serialized List
        foreach (HexCell activeCell in hexCells)
        {
            if (hexGrid.GetCell(Player.Instance.activeCellCoordinates) == activeCell)
                playerActive = true;
            else
                playerActive = false;

            SerializeAndUpdateHexCellsServerRpc(new SerializedNetworkHex(activeCell.coordinates.X, activeCell.coordinates.Z, activeCell.GetHeight(), playerActive, activeCell.GetRoundsTillCorrupted()));
        }

        UpdateGridVersionServerRpc();
        Debug.Log("Should Update Grid");
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