using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CollectablesGUI : MonoBehaviour
{
    [SerializeField]
    public Sprite collectableEmpty;
    public Sprite collectableFull;

    public Image[] collectableImages;

    private int collectableAmount = 3;

    private int currentCollectableAmount = 0;

    private void Update()
    {
        currentCollectableAmount = Player.Instance.collected;

        UpdateVisual();
    }

    public void UpdateVisual()
    {
        for (int i = 0; i < collectableAmount; i++)
        {
            if (i < currentCollectableAmount)
            {
                collectableImages[i].sprite = collectableFull;
            }
            else
            {
                collectableImages[i].sprite = collectableEmpty;
            }
        }
    }
}
