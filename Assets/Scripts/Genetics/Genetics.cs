using UnityEngine;
using System.Collections;

public class Genetics {

    public enum MIGEON_ACTION
    {
        AVANCER = 0,
        TURN_LEFT,
        TURN_RIGHT,
        JUMP,
        PUT_CUBE,
        NB_ACTIONS
    };

    private const int NB_TRIES_OPTIM = 20;

    public static MIGEON_ACTION[] createActions(int size)
    {
        MIGEON_ACTION[] actions = new MIGEON_ACTION[size];
        MIGEON_ACTION[] bestActions = new MIGEON_ACTION[size];
        float lastScore = 0;
        bool firstGeneration = true;

        for(int i=0;i<NB_TRIES_OPTIM;i++)
        {
            fillActions(actions);
            float score = scoreActions(actions);
            if(firstGeneration || score > lastScore)
            {
                MIGEON_ACTION[] tmp = bestActions;
                bestActions = actions;
                actions = bestActions;
            }
        }
        return actions;
    }

    private static void fillActions(MIGEON_ACTION[] actions)
    {
        for (int i = 0; i < actions.Length; i++)
        {
            actions[i] = randomAction();
        }
    }

    private static MIGEON_ACTION randomAction()
    {
        MIGEON_ACTION action = (MIGEON_ACTION)Random.Range(0, (int) MIGEON_ACTION.NB_ACTIONS);
        return action;
    }

    private static float scoreActions(MIGEON_ACTION[] actions)
    {
        float score = 1.0f;
        int lastTurn = -1;
        for (int i = 0; i < actions.Length; i++)
        {
            if (actions[i] == MIGEON_ACTION.TURN_LEFT || actions[i] == MIGEON_ACTION.TURN_RIGHT)
            {
                if (lastTurn >= 0)
                {
                    score -= (float)(i - lastTurn) / 10.0f;
                }
                lastTurn = i;
            }
            
        }

        return score;
    }

    
}
