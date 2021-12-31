using FluffyRobot.Core.Singeltons;
using Unity.Netcode;

public class PlayersManager : Singelton<PlayersManager> {
    private NetworkVariable<int> playersInGame = new NetworkVariable<int>();
    // private NetworkList<HexTest> hexCells = new NetworkList<HexTest>();

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
            // return hexCells.Count;
            return 0;
        }
    }

    private void Start() {
        NetworkManager.Singleton.OnClientConnectedCallback += (id) => {
            if(IsServer) {
                playersInGame.Value++;
                // hexCells.Add(new HexTest(3, 4, 5));
            }
        };

        NetworkManager.Singleton.OnClientDisconnectCallback += (id) => {
            if (IsServer) {
                playersInGame.Value--;
            }
        };
    }
}
