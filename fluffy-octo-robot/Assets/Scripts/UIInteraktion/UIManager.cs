using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.Netcode;

// mac multiple windows: open -na fluffy-test 

public class UIManager : NetworkBehaviour {
   
    [SerializeField]
    private Button startPlayerButton;

    [SerializeField]
    private Button startGodButton;

    [SerializeField]
    private TextMeshProUGUI playerInGameText;


    [SerializeField]
    private TextMeshProUGUI currentBattleState;

    private BattleSystem battleSystem;
    private HexCell hexCell;
    private HexGrid hexGrid;

    private void Update() {
        playerInGameText.text = $"Players in game: {PlayersManager.Instance.PlayersInGame}";
        currentBattleState.text = $"Current Game state: {PlayersManager.Instance.CurrentGameState}";
    }

    private void Start() {
        battleSystem = FindObjectOfType<BattleSystem>();
        hexCell = FindObjectOfType<HexCell>();
        hexGrid = FindObjectOfType<HexGrid>();

        startPlayerButton.onClick.AddListener(() => {
            if (NetworkManager.Singleton.StartHost()) {
                StartAsPlayer("Host Started ...");
            }
        });

        startGodButton.onClick.AddListener(() => {
            if (NetworkManager.Singleton.StartClient()) {
                StartAsGod("Client Started ...");
            }
        });
    }

    private void StartAsPlayer(string message)
    {
        Debug.Log(message);

        // host can call the server method
        //hexGrid.InitialSpawnTileServerRPC();
        hexCell.PlacePlayer();

        DisableStartButtonAndStartGame();
    }

    private void StartAsGod(string message)
    {
        Debug.Log(message);

        DisableStartButtonAndStartGame();
    }

    private void DisableStartButtonAndStartGame()
    {
        startGodButton.gameObject.SetActive(false);
        startPlayerButton.gameObject.SetActive(false);

        battleSystem.SetupBattle();
    }
}
