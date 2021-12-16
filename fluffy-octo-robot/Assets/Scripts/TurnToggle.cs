using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnToggle : MonoBehaviour
{
    public void toggle(bool turn)
    {
        if (turn)
        {
            TemporaryTurnControl.gameState = TemporaryTurnControl.GameState.GOD;
        } else
        {
            TemporaryTurnControl.gameState = TemporaryTurnControl.GameState.HUMAN;
        }
    }
}
