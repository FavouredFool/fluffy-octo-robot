using UnityEngine;
using UnityEngine.UI;

public enum GameState
{
    START, PLAYERTURN, GODTURN, WON, LOST
}

public class BattleSystem : MonoBehaviour
{
    public GameState state;

    public GameObject player;
    public Transform tile;

    public Button finishPlayerTurn;
    public Button finishGodTurn;

    private void Start()
    {
        Debug.Log("start battle");

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
    }

    private void GodTurn()
    {
        state = GameState.GODTURN;

        finishPlayerTurn.gameObject.SetActive(false);
        finishGodTurn.gameObject.SetActive(true);
    }

    public GameState GetState()
    {
        return state;
    }
}
