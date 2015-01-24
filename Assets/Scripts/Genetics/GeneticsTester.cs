using UnityEngine;
using System.Collections;

public class GeneticsTester : MonoBehaviour {

    public Genetics.GeneticCode code1;
    public Genetics.GeneticCode code2;

	// Use this for initialization
	void Start () {
        code1 = Genetics.makeGeneticCode();
        code1.showCodeDebug();
        Genetics.mutate(ref code1);
        code1.showCodeDebug();
        code2 = Genetics.makeGeneticCode();
        code2.showCodeDebug();
        Genetics.mutate(ref code2);
        code2.showCodeDebug();


        Genetics.GeneticCode code3 = Genetics.crossOver(code1, code2);
        code1.showCodeDebug();
        code2.showCodeDebug();
        code3.showCodeDebug();

        code3 = Genetics.crossOver(code1, code2);
        code3.showCodeDebug();
	}




    // Update is called once per frame
    void Update()
    {
	    
	}
}
