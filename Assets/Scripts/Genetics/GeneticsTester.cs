using UnityEngine;
using System.Collections;

public class GeneticsTester : MonoBehaviour {

    public Genetics.MIGEON_ACTION[] actions;

	// Use this for initialization
	void Start () {
        actions = Genetics.createActions(10);
        for(int i=0;i<actions.Length;i++)
            Debug.Log(actions[i]);
	}
	
	// Update is called once per frame
	void Update () {
	    
	}
}
