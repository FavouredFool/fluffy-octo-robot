using FluffyRobot.Core.Singeltons;
using Unity.Netcode;

public class PlayersManager : Singelton<PlayersManager> {
    private NetworkVariable<int> playersInGame = new NetworkVariable<int>();
    private NetworkList<HexTest> hexCells;

    private void Awake()
    {
        hexCells = new NetworkList<HexTest>();
    }

    public int PlayersInGame
    {
        get {
            return playersInGame.Value;
        }
    }

    public int HexCellSize
    {
        get
        {
            return hexCells.Count;
        }
    }

    public NetworkList<HexTest> HexTests
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

                if (HexCellSize == 0)
                {
                    hexCells.Add(new HexTest(0, 0, 2));
                    hexCells.Add(new HexTest(1, 0, 1));
                    hexCells.Add(new HexTest(0, 1, 3));
                    hexCells.Add(new HexTest(1, 1, 1));
                    hexCells.Add(new HexTest(-2, -2, 5));
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