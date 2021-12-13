using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.Netcode;

public class UIManager : MonoBehaviour {
    [SerializeField]
    private Button startServerButton;

    [SerializeField]
    private Button startHostButton;

    [SerializeField]
    private Button startClientButton;

    [SerializeField]
    private TextMeshProUGUI playerInGameText;

    private void Awake() {
        Cursor.visible = true;
    }

    private void Update() {
        playerInGameText.text = $"Players in game: {PlayersManager.Instance.PlayersInGame}";
    }

    private void Start() {
        startHostButton.onClick.AddListener(() => {
            if (NetworkManager.Singleton.StartHost()) {
                Debug.Log("Host started ...");
            } else {
                Debug.Log("Host could not be started ...");
            }
        });

        startServerButton.onClick.AddListener(() => {
            if (NetworkManager.Singleton.StartServer()) {
                Debug.Log("Server started ...");
            } else {
                Debug.Log("Server could not be started ...");
            }
        });

        startClientButton.onClick.AddListener(() => {
            if (NetworkManager.Singleton.StartClient()) {
                Debug.Log("Client started ...");
            } else {
                Debug.Log("Client could not be started ...");
            }
        });
    }
}
