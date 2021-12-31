using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.Netcode;

// mac multiple windows: open -na fluffy-test 

public class UIManager : MonoBehaviour {
   
    [SerializeField]
    private Button startPlayerButton;

    [SerializeField]
    private Button startGodButton;

    [SerializeField]
    private TextMeshProUGUI playerInGameText;

    private BattleSystem battleSystem;
    private HexCell hexCell;
    private HexGrid hexGrid;

    private void Update() {
        playerInGameText.text = $"Players in game: {PlayersManager.Instance.PlayersInGame}";
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
        // Debug.Log(PlayersManager.Instance.HexCellSize);

        hexGrid.SpawnTileServerRPC();
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
