using FluffyRobot.Core.Singeltons;
using Unity.Netcode;

public class PlayersManager : Singelton<PlayersManager> {
    private NetworkVariable<int> playersInGame = new NetworkVariable<int>();

    public int PlayersInGame {
        get {
            return playersInGame.Value;
        }
    }

    private void Start() {
        NetworkManager.Singleton.OnClientConnectedCallback += (id) => {
            if(IsServer) {
                playersInGame.Value++;
            }
        };

        NetworkManager.Singleton.OnClientDisconnectCallback += (id) => {
            if (IsServer) {
                playersInGame.Value--;
            }
        };
    }
}
