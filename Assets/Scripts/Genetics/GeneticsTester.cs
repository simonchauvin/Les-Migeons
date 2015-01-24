using UnityEngine;
using System.Collections;
using System.IO;

public class GeneticsTester : MonoBehaviour {

    public Genetics.GeneticCode code1;
    public Genetics.GeneticCode code2;

	// Use this for initialization
	void Start () {

        StreamWriter writer = new StreamWriter("genetic.txt", true);
        writer.AutoFlush = true;

        for (int i = 0; i < 100; i++)
        {
            code1 = Genetics.makeGeneticCode();
            code1.outputToFile(writer);
        }   

        writer.Close();

        code1.toString();
        Genetics.mutate(ref code1);
        code1.toString();
        code2 = Genetics.makeGeneticCode();
        code2.toString();
        Genetics.mutate(ref code2);
        code2.toString();


        Genetics.GeneticCode code3 = Genetics.crossOver(code1, code2);
        code1.toString();
        code2.toString();
        code3.toString();

        code3 = Genetics.crossOver(code1, code2);
        code3.toString();

        
	}




    // Update is called once per frame
    void Update()
    {
	    
	}
}
