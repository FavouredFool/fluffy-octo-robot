using UnityEngine;
using TMPro;
using Unity.Netcode;

// mac multiple windows: open -na fluffy-test 

public class UIManager : NetworkBehaviour {
   
    [SerializeField]
    private TextMeshProUGUI playerInGameText;

    [SerializeField]
    private TextMeshProUGUI currentBattleState;

    private void Update() {
        playerInGameText.text = $"Players in game: {PlayersManager.Instance.PlayersInGame}";
        currentBattleState.text = $"Current Game state: {PlayersManager.Instance.CurrentGameState}";
    }

    private void Start() {
        //if (NetworkManager.Singleton.StartClient()) {
        //    StartAsGod("Client Started ...");
        //}
    }
}
