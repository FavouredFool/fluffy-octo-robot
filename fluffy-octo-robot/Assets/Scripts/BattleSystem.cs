using UnityEngine;
using UnityEngine.UI;
using ObserverPattern;
using Unity.Netcode;
using TMPro;

public enum GameState
{
    START, HUMANTURN, GODTURN, CORRUPTION, WON, LOST
}

public class BattleSystem : NetworkBehaviour
{
    public GameObject player;
    public HexGrid hexGrid;

    public Button endTurnButton;
    private TMP_Text endTurnLabel;

    private ActionPoints actionPoints;

    private void Awake()
    {
        endTurnLabel = endTurnButton.transform.GetChild(0).GetComponent<TMP_Text>();
        actionPoints = FindObjectOfType<ActionPoints>();
    }

    private void Start()
    {

        endTurnButton.onClick.AddListener(() => {
            if (PlayersManager.Instance.CurrentGameState.Equals(GameState.GODTURN)) {
                PlayerTurn();
            } else if (PlayersManager.Instance.CurrentGameState.Equals(GameState.HUMANTURN))
            {
                CorruptionTurn();
            } else if (PlayersManager.Instance.CurrentGameState.Equals(GameState.CORRUPTION))
            {
                GodTurn();
            } else if (PlayersManager.Instance.CurrentGameState.Equals(GameState.START))
            {
                GodTurn();
            }
        });


        StartGame();
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

        //hexGrid.ReduceCorruptionTimer();
        //hexGrid.CorruptRandomCells();

        endTurnLabel.text = "End Corruption-Turn";

    }

    private void StartGame()
    {
        PlayersManager.Instance.UpdateGameState(GameState.START);

        endTurnLabel.text = "Start Game";

    }
}
