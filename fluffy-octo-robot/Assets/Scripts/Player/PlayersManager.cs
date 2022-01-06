using FluffyRobot.Core.Singeltons;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayersManager : Singelton<PlayersManager> {

    [SerializeField]
    private NetworkVariable<int> playersInGame = new NetworkVariable<int>();

    [SerializeField]
    private NetworkVariable<GameState> currentGameState = new NetworkVariable<GameState>(GameState.START);

    [SerializeField]
    private NetworkList<SerializedNetworkHex> hexCellsSerialized;

    private List<HexCell> cells;

    private void Awake()
    {
        cells = new();
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

    private void Start() {

        NetworkManager.Singleton.OnClientConnectedCallback += (id) => {

            if(IsServer) {

                playersInGame.Value++;

                Debug.Log("Player Added");

                /*
                if (SerializedHexCellSize == 0)
                {
                    hexCells.Add(new SerializedNetworkHex(0, 0, 2));
                    hexCells.Add(new SerializedNetworkHex(1, 0, 1));
                    hexCells.Add(new SerializedNetworkHex(0, 1, 3));
                    hexCells.Add(new SerializedNetworkHex(1, 1, 1));
                    hexCells.Add(new SerializedNetworkHex(-2, -2, 5));
                }
                

                hexCellsSerialized = SerializeHexCells(cells);
                */
            }
        };

        NetworkManager.Singleton.OnClientDisconnectCallback += (id) => {
            if (IsServer) {
                playersInGame.Value--;

                Debug.Log("Player Removed");
            }
        };
    }

    public NetworkList<SerializedNetworkHex> SerializeHexCells(List<HexCell> hexCells)
    {
        NetworkList<SerializedNetworkHex> tempList = new NetworkList<SerializedNetworkHex>();

        foreach (HexCell activeCell in hexCells)
        {
            tempList.Add(new SerializedNetworkHex(activeCell.coordinates.X, activeCell.coordinates.Z, activeCell.GetHeight()));
        }

        return tempList;
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

    public void UpdateHexCellsSerialized(List<HexCell> hexCells)
    {
        hexCellsSerialized = SerializeHexCells(hexCells);

    }
}