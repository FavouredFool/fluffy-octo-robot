using UnityEngine;
using TMPro;
using Unity.Netcode;

public class ActionPoints : NetworkBehaviour
{
    [SerializeField]
    private TextMeshProUGUI playerInGameText;

    [SerializeField]
    private int actionPoints = 5;

    private int currentActionPoints;

    private void Awake()
    {
        currentActionPoints = actionPoints;
    }

    private void Start()
    {
        playerInGameText.gameObject.SetActive(false);
    }

    private void Update()
    {
        playerInGameText.gameObject.SetActive(CheckShowLabel());

        if (CheckShowLabel())
        {
            playerInGameText.text = $"Remaining Action Points: {currentActionPoints}";
        }
    }

    private bool CheckShowLabel()
    {
        return IsHost && PlayersManager.Instance.CurrentGameState.Equals(GameState.HUMANTURN) || IsClient && PlayersManager.Instance.CurrentGameState.Equals(GameState.GODTURN);
    }

    public void UpdateActionPoints()
    {
        currentActionPoints--;
    }

    public int GetCurrentActionPoints()
    {
        return currentActionPoints;
    }

    public void ResetActionPoints()
    {
        currentActionPoints = actionPoints;
    }
}
