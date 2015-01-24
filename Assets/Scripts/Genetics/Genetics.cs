using UnityEngine;
using System.Collections;
using System.IO;

public class Genetics {

    public class GeneticCode
    {
        public MIGEON_ACTION[] actions;
        public int nbRepeat = 0;

        private const int MIN_NB_ACTION_START = 5;
        private const int MAX_NB_ACTION_START = 10;

        private const int MIN_NB_REPEAT_START = 5;
        private const int MAX_NB_REPEAT_START = 10;

        public GeneticCode()
        {
            this.actions = new MIGEON_ACTION[Random.Range(MIN_NB_ACTION_START, MAX_NB_ACTION_START)];
            this.nbRepeat = Random.Range(MIN_NB_REPEAT_START, MAX_NB_REPEAT_START);
        }

        public GeneticCode createCopy()
        {
            GeneticCode code = new GeneticCode();
            code.actions = new MIGEON_ACTION[this.actions.Length];
            for (int i = 0; i < actions.Length; i++)
                code.actions[i] = actions[i];
            code.nbRepeat = nbRepeat;
            return code;
        }

        public void fill()
        {
            fillActionsEvaluated(this.actions);
        }
       
        public string toString()
        {
            string chaine = "";
            chaine += nbRepeat;
            chaine += " : [";
            for (int i = 0; i < actions.Length-1; i++)
                chaine += actions[i] + " : ";
            chaine += actions[actions.Length - 1];
            chaine += "] = " + Genetics.scoreActions(actions);
            return chaine;
        }

        public void outputToDebug()
        {
            string chaine = this.toString();
            Debug.Log(chaine);
        }

        public void outputToFile(StreamWriter stream)
        {
            string chaine = this.toString();
            stream.WriteLine(chaine);
        }


    };

    public enum MIGEON_ACTION
    {
        AVANCER = 0,
        TURN_LEFT,
        TURN_RIGHT,
        JUMP,
        PUT_CUBE,
        NB_ACTIONS
    };

    private const int NB_TRIES_OPTIM = 100;
    private const float PROBA_MUT_ACTION = 0.3f;
    private const float PROBA_MUT_NB_REPEAT = 0.3f;
   
    public static GeneticCode makeGeneticCode()
    {
        GeneticCode code = new GeneticCode();
        code.fill();
        return code;
    }

    public static void mutate(ref GeneticCode code)
    {
        for (int i = 0; i < code.actions.Length; i++)
        {
            if(tirageAvecProba(PROBA_MUT_ACTION))
                code.actions[i] = randomAction();
        }

        if (tirageAvecProba(PROBA_MUT_NB_REPEAT))
            code.nbRepeat += tirageAvecProba(0.5f) ? -1 : 1;
    }

    public static GeneticCode crossOver(GeneticCode code1, GeneticCode code2)
    {
        int newSize = ((code1.actions.Length + code2.actions.Length) / 2) + (tirageAvecProba(0.4f) ? -1 : 1);
        GeneticCode code = new GeneticCode();
        code.nbRepeat = ((code1.nbRepeat + code2.nbRepeat) / 2) + (tirageAvecProba(0.4f) ? -1 : 1);

        for (int i = 0; i < code.actions.Length; i++)
        {
            if (i < code1.actions.Length && i < code2.actions.Length)
            {
                GeneticCode chosenCode = tirageAvecProba(0.5f) ? code1 : code2;
                code.actions[i] = chosenCode.actions[i];
            }
            else
            {
                if (i < code1.actions.Length)
                {
                    code.actions[i] = code1.actions[i];
                }
                else
                {
                    if (i < code2.actions.Length)
                    {
                        code.actions[i] = code2.actions[i];
                    }
                    else
                    {
                        code.actions[i] = randomAction();
                    }
                }
            }
        }

        return code;

    }

    public static bool tirageAvecProba(float proba)
    {
        if (Random.Range(0f, 1f) <= proba)
            return true;
        return false;
    }

    private static float fillActionsEvaluated(MIGEON_ACTION[] actionsBase)
    {
        MIGEON_ACTION[] actions = new MIGEON_ACTION[actionsBase.Length];
        MIGEON_ACTION[] bestActions = new MIGEON_ACTION[actionsBase.Length];
        float bestScore = 0;
        bool firstGeneration = true;

        for(int i=0;i<NB_TRIES_OPTIM;i++)
        {
            fillActionsRandom(actions);
            float score = scoreActions(actions);
            if (firstGeneration || score > bestScore)
            {
                MIGEON_ACTION[] tmp = bestActions;
                bestActions = actions;
                actions = tmp;
                bestScore = score;
                firstGeneration = false;
            }
        }

        for (int i = 0; i < actionsBase.Length; i++)
        {
            actionsBase[i] = bestActions[i];
        }

        return bestScore;

    }

    private static void fillActionsRandom(MIGEON_ACTION[] actions)
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
        float score = scoreOnTurns(actions);
        score += scoreOnBalance(actions);
        score += yesPutJump(actions);
        score += noPutAndMove(actions);
        score += noDblAction(actions);
        score += yesPutCube(actions);

        return score / 6.0f;
    }

    private static float scoreOnTurns(MIGEON_ACTION[] actions)
    {
        float score = 1.0f;
        int lastTurn = -1;
        for (int i = 0; i < actions.Length; i++)
        {
            if (actions[i] == MIGEON_ACTION.TURN_LEFT || actions[i] == MIGEON_ACTION.TURN_RIGHT)
            {
                if (lastTurn >= 0)
                {
                    score -= ((float)(i - lastTurn)) / 10.0f;
                }
                lastTurn = i;
            }

        }

        return score;
    }

    //EVALUATORS

    private static float noPutAndMove(MIGEON_ACTION[] actions)
    {
        float score = 1.0f;
        for (int i = 0; i < actions.Length-1; i++)
        {
            if (actions[i] == MIGEON_ACTION.PUT_CUBE && actions[i+1] == MIGEON_ACTION.AVANCER)
            {
                score -= 1.0f / (float)(actions.Length-1);
            }

        }

        return score;
    }

    private static float yesPutJump(MIGEON_ACTION[] actions)
    {
        float score = 0.0f;
        for (int i = 0; i < actions.Length - 2; i++)
        {
            if (actions[i] == MIGEON_ACTION.PUT_CUBE && actions[i + 1] == MIGEON_ACTION.JUMP)
            {
                score += 1.0f / (float)(actions.Length - 1);
            }

        }

        return score;
    }

    private static float noDblAction(MIGEON_ACTION[] actions)
    {
        float score = 1.0f;
        for (int i = 0; i < actions.Length - 2; i++)
        {
            if (actions[i] == actions[i + 1])
            {
                score -= 1.0f / (float)(actions.Length - 1);
            }

        }

        return score;
    }

    private static float noJumpJump(MIGEON_ACTION[] actions)
    {
        float score = 1.0f;
        for (int i = 0; i < actions.Length - 2; i++)
        {
            if (actions[i] == MIGEON_ACTION.JUMP && actions[i + 1] == MIGEON_ACTION.JUMP)
            {
                score -= 1.0f / (float)(actions.Length - 1);
            }
        }
        return score;
    }

    private static float yesPutCube(MIGEON_ACTION[] actions)
    {
        float score = 0.0f;
        for (int i = 0; i < actions.Length - 2; i++)
        {
            if (actions[i] != MIGEON_ACTION.PUT_CUBE && actions[i + 1] == MIGEON_ACTION.PUT_CUBE)
            {
                score += 1.0f / (float)(actions.Length - 1);
            }
        }
        return score;
    }

    private static float scoreOnBalance(MIGEON_ACTION[] actions)
    {
        int [] nbEach = new int [(int)MIGEON_ACTION.NB_ACTIONS];
        for (int i = 0; i < actions.Length; i++)
        {
            nbEach[(int)actions[i]]++;
        }

        float moyenne = 0;
        for (int i = 0; i < (int)MIGEON_ACTION.NB_ACTIONS; i++)
            moyenne += nbEach[i];
        
        moyenne /= (float)MIGEON_ACTION.NB_ACTIONS;

        float variance = 0;
        for (int i = 0; i < (int)MIGEON_ACTION.NB_ACTIONS; i++)
            variance += Mathf.Abs(moyenne-nbEach[i]);
        variance /= (float)MIGEON_ACTION.NB_ACTIONS;

        return 1.0f - (variance / (float)actions.Length);
    }

    
}
