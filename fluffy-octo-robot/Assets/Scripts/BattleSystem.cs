using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public enum GameState
{
    START, HUMANTURN, GODTURN, CORRUPTION, WON, LOST
}

public class BattleSystem : NetworkBehaviour
{
    public GameObject player;
    public HexGrid hexGrid;

    public Button endTurnButton;
    public Text endTurnLabel;

    private ActionPoints actionPoints;

    private void Awake()
    {
        actionPoints = FindObjectOfType<ActionPoints>();
    }

    private void Start()
    {

        endTurnButton.onClick.AddListener(() => {
            if (!hexGrid.blockActions)
            {
                switch (PlayersManager.Instance.CurrentGameState)
                {
                    case GameState.GODTURN:
                        PlayerTurn();
                        break;

                    case GameState.HUMANTURN:
                        CorruptionTurn();
                        break;

                    case GameState.START:
                        GodTurn();
                        break;

                   // case GameState.CORRUPTION:
                        //GodTurn();
                        //break;
                    default:
                        Debug.LogWarning("FEHLER");
                        break;
                }
            }
        });

        if (IsHost)
        {
            StartGame();
        }
        
    }

    protected void Update()
    {
        endTurnButton.gameObject.SetActive(
            PlayersManager.Instance.CurrentGameState.Equals(GameState.START) && IsHost ||
            PlayersManager.Instance.CurrentGameState.Equals(GameState.CORRUPTION) ||
            PlayersManager.Instance.CurrentGameState.Equals(GameState.GODTURN) && PlayersManager.Instance.IsGod ||
            PlayersManager.Instance.CurrentGameState.Equals(GameState.GODTURN) && PlayersManager.Instance.IsSingle ||
            PlayersManager.Instance.CurrentGameState.Equals(GameState.HUMANTURN) && PlayersManager.Instance.IsHuman ||
            PlayersManager.Instance.CurrentGameState.Equals(GameState.HUMANTURN) && PlayersManager.Instance.IsSingle
        );
        

        switch (PlayersManager.Instance.CurrentGameState)
        {
            case GameState.GODTURN:
                endTurnLabel.text = "End\nGod-Turn";
                break;

            case GameState.HUMANTURN:
                endTurnLabel.text = "End\nHuman-Turn";
                break;

            case GameState.START:
                endTurnLabel.text = "Start Game";
                break;

            //case GameState.CORRUPTION:
                //endTurnLabel.text = "End\nCorruption-Turn";
                //break;

            case GameState.WON:
                GameWon();
                break;

            case GameState.LOST:
                GameLost();
                break;

            default:
                endTurnLabel.text = "FEHLER";
                break;
        }

    }

    private void PlayerTurn()
    {
        PlayersManager.Instance.UpdateGameStateServerRpc(GameState.HUMANTURN);
        hexGrid.ReformWorld();

        actionPoints.ResetActionPoints();

    }

    private void GodTurn()
    {   
        PlayersManager.Instance.UpdateGameStateServerRpc(GameState.GODTURN);
        hexGrid.ReformWorld();

        actionPoints.ResetActionPoints();

    }

    private void CorruptionTurn()
    {
        PlayersManager.Instance.UpdateGameStateServerRpc(GameState.CORRUPTION);

        hexGrid.ReduceCorruptionTimer();
        hexGrid.CorruptRandomCells();

        GodTurn();
    }

    private void StartGame()
    {
        PlayersManager.Instance.UpdateGameState(GameState.START);

    }

    private void GameWon()
    {
        Debug.Log("SPIEL GEWONNEN!");
        //NetworkManager.Singleton.Shutdown();
        SceneManager.LoadScene(sceneName: "MainMenu");
    }

    private void GameLost()
    {
        Debug.Log("SPIEL VERLOREN!");
        //NetworkManager.Singleton.Shutdown();
        SceneManager.LoadScene(sceneName: "MainMenu");
    }
}
