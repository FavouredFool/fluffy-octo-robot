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

                    case GameState.CORRUPTION:
                        GodTurn();
                        break;
                    default:
                        Debug.LogWarning("FEHLER");
                        break;
                }
            }
        });


        StartGame();
    }

    protected void Update()
    {
        endTurnButton.gameObject.SetActive(
            PlayersManager.Instance.CurrentGameState.Equals(GameState.START) ||
            PlayersManager.Instance.CurrentGameState.Equals(GameState.CORRUPTION) ||
            PlayersManager.Instance.CurrentGameState.Equals(GameState.GODTURN) && PlayersManager.Instance.IsGod ||
            PlayersManager.Instance.CurrentGameState.Equals(GameState.HUMANTURN) && PlayersManager.Instance.IsHuman
        );

        switch(PlayersManager.Instance.CurrentGameState)
        {
            case GameState.GODTURN:
                endTurnLabel.text = "End God-Turn";
                break;

            case GameState.HUMANTURN:
                endTurnLabel.text = "End Human-Turn";
                break;

            case GameState.START:
                endTurnLabel.text = "Start Game";
                break;

            case GameState.CORRUPTION:
                endTurnLabel.text = "End Corruption-Turn";
                break;

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

        endTurnLabel.text = "End Human-Turn";
    }

    private void GodTurn()
    {   
        PlayersManager.Instance.UpdateGameStateServerRpc(GameState.GODTURN);
        hexGrid.ReformWorld();

        actionPoints.ResetActionPoints();

        endTurnLabel.text = "End God-Turn";
    }

    private void CorruptionTurn()
    {
        PlayersManager.Instance.UpdateGameStateServerRpc(GameState.CORRUPTION);

        hexGrid.ReduceCorruptionTimer();
        hexGrid.CorruptRandomCells();
        hexGrid.ReformWorld();

        endTurnLabel.text = "End Corruption-Turn";

    }

    private void StartGame()
    {
        PlayersManager.Instance.UpdateGameState(GameState.START);

        endTurnLabel.text = "Start Game";

    }

    private void GameWon()
    {
        Debug.Log("SPIEL GEWONNEN!");
        SceneManager.LoadScene(sceneName: "MainMenu");
    }

    private void GameLost()
    {
        Debug.Log("SPIEL VERLOREN!");
        SceneManager.LoadScene(sceneName: "MainMenu");
    }
}
