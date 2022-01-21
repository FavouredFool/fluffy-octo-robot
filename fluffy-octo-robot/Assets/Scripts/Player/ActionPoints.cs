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
        if (
            PlayersManager.Instance.CurrentGameState.Equals(GameState.GODTURN) && (PlayersManager.Instance.IsGod || PlayersManager.Instance.IsSingle) ||
            PlayersManager.Instance.CurrentGameState.Equals(GameState.HUMANTURN) && (PlayersManager.Instance.IsHuman || PlayersManager.Instance.IsSingle)
        ) {
            foreach (Image i in APImages)
            {
                i.enabled = true;
            }
        }
        else
        {
            foreach (Image i in APImages)
            {
                i.enabled = false;
            }
        }
        
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
