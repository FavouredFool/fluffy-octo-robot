using UnityEngine;
using TMPro;
using Unity.Netcode;

// mac multiple windows: open -na fluffy-test 

public class UIManager : NetworkBehaviour {
   
    [SerializeField]
    private TextMeshProUGUI playerInGameText;

    private void Update() {
        playerInGameText.text = $"Players in game: {PlayersManager.Instance.PlayersInGame}";
    }

    private void Start() {
        //if (NetworkManager.Singleton.StartClient()) {
        //    StartAsGod("Client Started ...");
        //}
    }
}
