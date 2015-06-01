using UnityEngine;
using System.Collections;
using System.IO;

public class Genetics {

    public class GeneticCode
    {
        public MA[] actions;
        public int nbRepeat = 0;

        private const int MIN_NB_ACTION_START = 5;
        private const int MAX_NB_ACTION_START = 10;

        private const int MIN_NB_REPEAT_START = 5;
        private const int MAX_NB_REPEAT_START = 10;

        public static MA[] bizarre = {MA.AVANCER,MA.PUT_CUBE,MA.JUMP,MA.PUT_CUBE,MA.JUMP,MA.PUT_CUBE,MA.JUMP,MA.AVANCER,MA.TURN_RIGHT,MA.PUT_CUBE,MA.JUMP,MA.PUT_CUBE,MA.JUMP,MA.PUT_CUBE,MA.JUMP};
        public static MA[] colimacon = { MA.PUT_CUBE, MA.JUMP, MA.PUT_CUBE, MA.JUMP, MA.PUT_CUBE, MA.JUMP, MA.TURN_LEFT };
        public static MA[] allerretour = { MA.PUT_CUBE, MA.JUMP, MA.PUT_CUBE, MA.JUMP, MA.PUT_CUBE, MA.JUMP, MA.PUT_CUBE, MA.JUMP, MA.PUT_CUBE, MA.JUMP, MA.TURN_LEFT, MA.TURN_LEFT };
        public static MA[] damier = { MA.TURN_LEFT, MA.TURN_LEFT, MA.JUMP, MA.PUT_CUBE, MA.JUMP, MA.PUT_CUBE, MA.JUMP};
        public static MA[] tree = { MA.TURN_LEFT, MA.TURN_LEFT, MA.PUT_CUBE, MA.TURN_LEFT, MA.PUT_CUBE, MA.JUMP};
		//public static MA[] plateforme = {MA.PUT_CUBE, MA.TURN_LEFT, MA.PUT_CUBE, MA.TURN_LEFT, MA.PUT_CUBE, MA.TURN_LEFT, MA.PUT_CUBE,MA.JUMP,MA.PUT_CUBE_UNDER,MA.AVANCER,MA.TURN_LEFT,MA.PUT_CUBE_UNDER,MA.AVANCER,MA.TURN_LEFT,MA.PUT_CUBE_UNDER,MA.AVANCER,MA.TURN_LEFT};
		public static MA[] plateforme = {MA.PUT_CUBE_UNDER,MA.JUMP, MA.PUT_CUBE_UNDER, MA.AVANCER,MA.PUT_CUBE_UNDER, MA.AVANCER,MA.PUT_CUBE_UNDER, MA.AVANCER,MA.PUT_CUBE_UNDER, MA.AVANCER,MA.PUT_CUBE_UNDER, MA.AVANCER,MA.TURN_LEFT,MA.PUT_CUBE_UNDER, MA.AVANCER,MA.TURN_LEFT,MA.PUT_CUBE_UNDER, MA.AVANCER,MA.PUT_CUBE_UNDER, MA.AVANCER,MA.PUT_CUBE_UNDER, MA.AVANCER,MA.PUT_CUBE_UNDER, MA.AVANCER,MA.PUT_CUBE_UNDER, MA.AVANCER,MA.TURN_RIGHT,MA.PUT_CUBE_UNDER, MA.AVANCER,MA.TURN_RIGHT};

        private static MA[][] designs = { bizarre, colimacon, allerretour, damier, tree, plateforme };

        public GeneticCode()
        {
            this.actions = new MA[Random.Range(MIN_NB_ACTION_START, MAX_NB_ACTION_START)];
            this.nbRepeat = Random.Range(MIN_NB_REPEAT_START, MAX_NB_REPEAT_START);
        }

        public GeneticCode createCopy()
        {
            GeneticCode code = new GeneticCode();
            code.actions = new MA[this.actions.Length];
            for (int i = 0; i < actions.Length; i++)
                code.actions[i] = actions[i];
            code.nbRepeat = nbRepeat;
            return code;
        }

        public void fill()
        {
            fillActionsEvaluated(this.actions);
        }

		public void initFromCode(GeneticCode code)
		{
			this.actions = new MA[code.actions.Length];
			for (int i = 0; i < code.actions.Length; i++)
				this.actions[i] = code.actions[i];
			this.nbRepeat = code.nbRepeat;
		}

        public void fillWithDesigned(int numDesign = -1)
        {
			int des = Random.Range(0, designs.Length);
			if(numDesign >=0)
				des = Mathf.Min(numDesign,designs.Length-1);
            this.actions = new MA[designs[des].Length];
            for (int i = 0; i < designs[des].Length; i++)
                this.actions[i] = designs[des][i];
        }

		public static int getNbDesign(){
			return designs.Length;
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

    public enum MA
    {
        AVANCER = 0,
        TURN_LEFT,
        TURN_RIGHT,
        JUMP,
        PUT_CUBE,
		PUT_CUBE_UNDER,
        NB_ACTIONS
    };

    private const int NB_TRIES_OPTIM = 100;
    private const float PROBA_MUT_ACTION = 0.3f;
    private const float PROBA_MUT_NB_REPEAT = 0.3f;
   
    public static GeneticCode makeGeneticCode()
    {
        GeneticCode code = new GeneticCode();

        if (tirageAvecProba(0.3f))
        {
            code.fillWithDesigned();
            if (tirageAvecProba(0.3f))
            {
				mutateAndOptim(ref code);
            }
        }
        else
            code.fill();

        return code;
    }

	public static float mutateAndOptim(ref GeneticCode code)
	{
		float bestScore = 0.0f;

		GeneticCode copyCode = code.createCopy ();

		for (int i=0; i<NB_TRIES_OPTIM; i++) {
			mutate(ref copyCode);
			float score = scoreActions (code.actions);
			if(score > bestScore){
				bestScore = score;
				code.initFromCode(copyCode);
			}
		}

		return bestScore;

	}

    private static void mutate(ref GeneticCode code)
    {
        for (int i = 0; i < code.actions.Length; i++)
        {
            if(tirageAvecProba(PROBA_MUT_ACTION))
                code.actions[i] = randomAction();
        }

        if (tirageAvecProba(PROBA_MUT_NB_REPEAT))
            code.nbRepeat += tirageAvecProba(0.5f) ? 1 : 3;
    }

	public static void mutateOne(ref GeneticCode code)
	{
		int action = Random.Range (0, code.actions.Length-1);
		code.actions[action] = randomAction();
		if (tirageAvecProba(PROBA_MUT_NB_REPEAT))
			code.nbRepeat += tirageAvecProba(0.5f) ? 1 : 3;
	}

	public static GeneticCode crossOverAndOptim(GeneticCode code1, GeneticCode code2)
	{
		float bestScore = 0.0f;
		
		GeneticCode codeCrossBest = crossOver(code1,code2);
		
		for (int i=0; i<NB_TRIES_OPTIM; i++) {
			GeneticCode codeCross = crossOver(code1,code2);
			float score = scoreActions (codeCross.actions);
			if(score > bestScore){
				bestScore = score;
				codeCrossBest = codeCross;
			}
		}
		
		return codeCrossBest;
	}

	private static GeneticCode crossOver(GeneticCode code1, GeneticCode code2)
    {
        int newSize = ((code1.actions.Length + code2.actions.Length) / 2) + (tirageAvecProba(0.4f) ? -1 : 1);
        GeneticCode code = new GeneticCode();
        code.nbRepeat = ((code1.nbRepeat + code2.nbRepeat) / 2) + (tirageAvecProba(0.4f) ? 1 : 3);

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

    private static float fillActionsEvaluated(MA[] actionsBase)
    {
        MA[] actions = new MA[actionsBase.Length];
        MA[] bestActions = new MA[actionsBase.Length];
        float bestScore = 0;
        bool firstGeneration = true;

        for(int i=0;i<NB_TRIES_OPTIM;i++)
        {
            fillActionsRandom(actions);
            float score = scoreActions(actions);
            if (firstGeneration || score > bestScore)
            {
                MA[] tmp = bestActions;
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

    private static void fillActionsRandom(MA[] actions)
    {
        for (int i = 0; i < actions.Length; i++)
        {
            actions[i] = randomAction();
        }
    }

    private static MA randomAction()
    {
        MA action = (MA)Random.Range(0, (int) MA.NB_ACTIONS);
        return action;
    }

    private static float scoreActions(MA[] actions)
    {
        float score = scoreOnTurns(actions);
        score += scoreOnBalance(actions);
        score += yesPutJump(actions);
        score += noPutAndMove(actions);
        score += noDblAction(actions);
        score += yesPutCube(actions);

        return score / 6.0f;
    }

    private static float scoreOnTurns(MA[] actions)
    {
        float score = 1.0f;
        int lastTurn = -1;
        for (int i = 0; i < actions.Length; i++)
        {
            if (actions[i] == MA.TURN_LEFT || actions[i] == MA.TURN_RIGHT)
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

    private static float noPutAndMove(MA[] actions)
    {
        float score = 1.0f;
        for (int i = 0; i < actions.Length-1; i++)
        {
            if (actions[i] == MA.PUT_CUBE && actions[i+1] == MA.AVANCER)
            {
                score -= 1.0f / (float)(actions.Length-1);
            }

        }

        return score;
    }

    private static float yesPutJump(MA[] actions)
    {
        float score = 0.0f;
        for (int i = 0; i < actions.Length - 2; i++)
        {
            if (actions[i] == MA.PUT_CUBE && actions[i + 1] == MA.JUMP)
            {
                score += 1.0f / (float)(actions.Length - 1);
            }

        }

        return score;
    }

    private static float noDblAction(MA[] actions)
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

    private static float noJumpJump(MA[] actions)
    {
        float score = 1.0f;
        for (int i = 0; i < actions.Length - 2; i++)
        {
            if (actions[i] == MA.JUMP && actions[i + 1] == MA.JUMP)
            {
                score -= 1.0f / (float)(actions.Length - 1);
            }
        }
        return score;
    }

    private static float yesPutCube(MA[] actions)
    {
        float score = 0.0f;
        for (int i = 0; i < actions.Length - 2; i++)
        {
            if (actions[i] != MA.PUT_CUBE && actions[i + 1] == MA.PUT_CUBE)
            {
                score += 1.0f / (float)(actions.Length - 1);
            }
        }
        return score;
    }

    private static float scoreOnBalance(MA[] actions)
    {
        int [] nbEach = new int [(int)MA.NB_ACTIONS];
        for (int i = 0; i < actions.Length; i++)
        {
            nbEach[(int)actions[i]]++;
        }

        float moyenne = 0;
        for (int i = 0; i < (int)MA.NB_ACTIONS; i++)
            moyenne += nbEach[i];
        
        moyenne /= (float)MA.NB_ACTIONS;

        float variance = 0;
        for (int i = 0; i < (int)MA.NB_ACTIONS; i++)
            variance += Mathf.Abs(moyenne-nbEach[i]);
        variance /= (float)MA.NB_ACTIONS;

        return 1.0f - (variance / (float)actions.Length);
    }

    
}
