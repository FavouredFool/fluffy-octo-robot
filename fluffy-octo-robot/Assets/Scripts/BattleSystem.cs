using UnityEngine;
using UnityEngine.UI;
using ObserverPattern;

public enum GameState
{
    START, PLAYERTURN, GODTURN, CORRUPTION, WON, LOST
}

public class BattleSystem : MonoBehaviour
{
    public GameState state;

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

        state = GameState.START;
    }

    public void SetupBattle()
    {
        PlayerTurn();
    }

    private void PlayerTurn()
    {
        state = GameState.PLAYERTURN;

        finishPlayerTurn.gameObject.SetActive(true);
        finishGodTurn.gameObject.SetActive(false);

        subject.Notify();
    }

    private void GodTurn()
    {
        state = GameState.GODTURN;

        finishPlayerTurn.gameObject.SetActive(false);
        finishGodTurn.gameObject.SetActive(true);

        subject.Notify();
    }

    public GameState GetState()
    {
        return state;
    }
}
