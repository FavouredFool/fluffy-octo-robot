using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TemporaryTurnControl
{
    public enum GameState
    {
        START, HUMAN, GOD, WON, LOST
    }

    public static GameState gameState = GameState.GOD;


}
