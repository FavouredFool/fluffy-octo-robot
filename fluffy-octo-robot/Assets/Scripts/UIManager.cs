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

    private BattleSystem battleSystem;

    private void Awake() {
        Cursor.visible = true;
    }

    private void Update() {
        playerInGameText.text = $"Players in game: {PlayersManager.Instance.PlayersInGame}";
    }

    private void Start() {
        battleSystem = FindObjectOfType<BattleSystem>();

        startHostButton.onClick.AddListener(() => {
            if (NetworkManager.Singleton.StartHost()) {
                StartGame("Host Started ...");
            } else {
                Debug.Log("Host could not be started ...");
            }
        });

        startServerButton.onClick.AddListener(() => {
            if (NetworkManager.Singleton.StartServer()) {
                StartGame("Server Started ...");
            } else {
                Debug.Log("Server could not be started ...");
            }
        });

        startClientButton.onClick.AddListener(() => {
            if (NetworkManager.Singleton.StartClient()) {
                StartGame("Client Started ...");
            } else {
                Debug.Log("Client could not be started ...");
            }
        });
    }

    private void StartGame(string message)
    {
        Debug.Log(message);

        startClientButton.gameObject.SetActive(false);
        startHostButton.gameObject.SetActive(false);
        startServerButton.gameObject.SetActive(false);

        battleSystem.SetupBattle();
    }
}
