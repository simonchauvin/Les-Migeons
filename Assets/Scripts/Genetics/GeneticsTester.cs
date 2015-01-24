using UnityEngine;
using System.Collections;

public class GeneticsTester : MonoBehaviour {

    public Genetics.GeneticCode code;

	// Use this for initialization
	void Start () {
        code = Genetics.makeGeneticCode();
        for(int i=0;i<code.actions.Length;i++)
            Debug.Log(code.actions[i]);
	}
	
	// Update is called once per frame
	void Update () {
	    
	}
}
