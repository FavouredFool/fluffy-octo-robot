using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ObserverPattern;

public class TurnToggle : MonoBehaviour
{

    public Subject subject = new Subject();

    public void toggle(bool turn)
    {
        if (turn)
        {
            TemporaryTurnControl.gameState = TemporaryTurnControl.GameState.GOD;
        } else
        {
            TemporaryTurnControl.gameState = TemporaryTurnControl.GameState.HUMAN;
        }

        // Allen Observern bescheid geben
        subject.Notify();
    }
}
