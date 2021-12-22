using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.Netcode;

// mac multiple windows: open -na fluffy-test 

public class UIManager : MonoBehaviour {
   
    [SerializeField]
    private Button startHostButton;

    [SerializeField]
    private Button startClientButton;

    [SerializeField]
    private TextMeshProUGUI playerInGameText;

    private BattleSystem battleSystem;
    private HexCell hexCell;

    private void Update() {
        playerInGameText.text = $"Players in game: {PlayersManager.Instance.PlayersInGame}";
    }

    private void Start() {
        battleSystem = FindObjectOfType<BattleSystem>();
        hexCell = FindObjectOfType<HexCell>();

        startHostButton.onClick.AddListener(() => {
            if (NetworkManager.Singleton.StartHost()) {
                StartGame("Host Started ...");
            }
        });

        startClientButton.onClick.AddListener(() => {
            if (NetworkManager.Singleton.StartClient()) {
                // StartGame("Client Started ...");

                startClientButton.gameObject.SetActive(false);
                startHostButton.gameObject.SetActive(false);

                battleSystem.SetupBattle();
            }
        });
    }

    private void StartGame(string message)
    {
        Debug.Log(message);

        hexCell.PlacePlayer();

        startClientButton.gameObject.SetActive(false);
        startHostButton.gameObject.SetActive(false);

        battleSystem.SetupBattle();
    }
}
