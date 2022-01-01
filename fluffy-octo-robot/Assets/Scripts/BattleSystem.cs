using UnityEngine;
using UnityEngine.UI;
using ObserverPattern;
using Unity.Netcode;

public enum GameState
{
    START, PLAYERTURN, GODTURN, CORRUPTION, WON, LOST
}

public class BattleSystem : NetworkBehaviour
{
    public GameObject player;

    public Button finishPlayerTurn;
    public Button finishGodTurn;

    public Subject subject = new Subject();

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
    }

    private void Update()
    {
        finishPlayerTurn.gameObject.SetActive(IsHost && PlayersManager.Instance.CurrentGameState.Equals(GameState.PLAYERTURN));
        finishGodTurn.gameObject.SetActive(IsClient && PlayersManager.Instance.CurrentGameState.Equals(GameState.GODTURN));
    }

    public void SetupBattle()
    {
        PlayerTurn();
    }

    private void PlayerTurn()
    {
        PlayersManager.Instance.UpdateGameStateServerRpc(GameState.PLAYERTURN);

        subject.Notify();
    }

    private void GodTurn()
    {
        PlayersManager.Instance.UpdateGameStateServerRpc(GameState.GODTURN);

        subject.Notify();
    }
}
