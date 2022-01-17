using UnityEngine;
using TMPro;
using Unity.Netcode;
using UnityEngine.UI;

public class ActionPoints : NetworkBehaviour
{
    [SerializeField]
    public Sprite APEmpty;
    public Sprite APFull;

    public Image [] APImages;

    [SerializeField]
    [Range(1, 5)]
    private int actionPoints = 5;

    private int currentActionPoints;

    private void Awake()
    {
        currentActionPoints = actionPoints;

        foreach(Image activeImage in APImages)
        {
            activeImage.gameObject.SetActive(false);
        }
    }

    private void Start()
    {
        

        for (int i = actionPoints-1; i >= 0; i--)
        {
            APImages[i].gameObject.SetActive(true);
        }

    }

    private void Update()
    {

    }

    private bool CheckShowLabel()
    {
        return IsHost && PlayersManager.Instance.CurrentGameState.Equals(GameState.HUMANTURN) || IsClient && PlayersManager.Instance.CurrentGameState.Equals(GameState.GODTURN);
    }

    public void UpdateActionPoints()
    {
        currentActionPoints--;
        UpdateVisual();
    }

    public int GetCurrentActionPoints()
    {
        return currentActionPoints;
    }

    public void ResetActionPoints()
    {
        currentActionPoints = actionPoints;
        UpdateVisual();
    }

    public void UpdateVisual()
    {
        for (int i = 0; i < actionPoints; i++)
        {
            if (i < currentActionPoints)
            {
                APImages[i].sprite = APFull;
            }
            else
            {
                APImages[i].sprite = APEmpty;
            }
        }
    }
}
