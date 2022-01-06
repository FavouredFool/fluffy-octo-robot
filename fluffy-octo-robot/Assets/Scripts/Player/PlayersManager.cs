using FluffyRobot.Core.Singeltons;
using Unity.Netcode;
using UnityEngine;

public class PlayersManager : Singelton<PlayersManager> {

    [SerializeField]
    private NetworkVariable<int> playersInGame = new NetworkVariable<int>();

    [SerializeField]
    private NetworkVariable<GameState> currentGameState = new NetworkVariable<GameState>(GameState.START);

    [SerializeField]
    private NetworkList<SerializedNetworkHex> hexCells;

    private void Awake()
    {
        hexCells = new NetworkList<SerializedNetworkHex>();
    }

    public int PlayersInGame
    {
        get {
            return playersInGame.Value;
        }
    }

    public int SerializedHexCellSize
    {
        get
        {
            return hexCells.Count;
        }
    }

    public NetworkList<SerializedNetworkHex> SerializedHexCells
    {
        get
        {
            return hexCells;
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

                if (SerializedHexCellSize == 0)
                {
                    hexCells.Add(new SerializedNetworkHex(0, 2, 0));
                    hexCells.Add(new SerializedNetworkHex(1, 1, 0));
                    hexCells.Add(new SerializedNetworkHex(0, 3, 1));
                    hexCells.Add(new SerializedNetworkHex(1, 1, 1));
                    hexCells.Add(new SerializedNetworkHex(-2, 5, -2));
                }
            }
        };

        NetworkManager.Singleton.OnClientDisconnectCallback += (id) => {
            if (IsServer) {
                playersInGame.Value--;
            }
        };
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