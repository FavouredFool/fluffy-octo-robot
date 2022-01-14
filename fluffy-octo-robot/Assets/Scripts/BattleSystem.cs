using UnityEngine;
using UnityEngine.UI;
using ObserverPattern;
using Unity.Netcode;

public enum GameState
{
    START, HUMANTURN, GODTURN, CORRUPTION, WON, LOST
}

public class BattleSystem : NetworkBehaviour
{
    public GameObject player;
    public HexGrid hexGrid;

    public Button finishPlayerTurn;
    public Button finishGodTurn;

    private ActionPoints actionPoints;

    private void Awake()
    {
        actionPoints = FindObjectOfType<ActionPoints>();
    }

    private void Start()
    {
        finishPlayerTurn.gameObject.SetActive(false);
        finishGodTurn.gameObject.SetActive(false);

        finishPlayerTurn.onClick.AddListener(() => {
            GodTurn();
        });

        finishGodTurn.onClick.AddListener(() => {
            PlayerTurn();
        });

        PlayerTurn();
    }

    private void Update()
    {
        finishPlayerTurn.gameObject.SetActive(IsHost && PlayersManager.Instance.CurrentGameState.Equals(GameState.HUMANTURN));
        finishGodTurn.gameObject.SetActive(IsClient && PlayersManager.Instance.CurrentGameState.Equals(GameState.GODTURN));
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
}
