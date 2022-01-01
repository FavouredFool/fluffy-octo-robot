using FluffyRobot.Core.Singeltons;
using Unity.Netcode;

public class PlayersManager : Singelton<PlayersManager> {
    private NetworkVariable<int> playersInGame = new NetworkVariable<int>();
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

    private void Start() {
        NetworkManager.Singleton.OnClientConnectedCallback += (id) => {
            if(IsServer) {
                playersInGame.Value++;

                if (SerializedHexCellSize == 0)
                {
                    hexCells.Add(new SerializedNetworkHex(0, 0, 2));
                    hexCells.Add(new SerializedNetworkHex(1, 0, 1));
                    hexCells.Add(new SerializedNetworkHex(0, 1, 3));
                    hexCells.Add(new SerializedNetworkHex(1, 1, 1));
                    hexCells.Add(new SerializedNetworkHex(-2, -2, 5));
                }
            }
        };

        NetworkManager.Singleton.OnClientDisconnectCallback += (id) => {
            if (IsServer) {
                playersInGame.Value--;
            }
        };
    }
}