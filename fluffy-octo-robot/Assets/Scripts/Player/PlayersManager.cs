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


    private void Awake()
    {
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

            }
        };

        NetworkManager.Singleton.OnClientDisconnectCallback += (id) => {
            if (IsServer) {
                playersInGame.Value--;

                Debug.Log("Player Removed");
            }
        };
    }

    public void SerializeHexCells(List<HexCell> hexCells)
    {
        hexCellsSerialized.Clear();

        foreach (HexCell activeCell in hexCells)
        {
            hexCellsSerialized.Add(new SerializedNetworkHex(activeCell.coordinates.X, activeCell.coordinates.Z, activeCell.GetHeight()));
        }

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

}