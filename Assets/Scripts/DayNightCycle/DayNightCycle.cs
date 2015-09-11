using UnityEngine;
using System.Collections;

public class DayNightCycle : MonoBehaviour {

    public int nbCube { get; set; }
    private int newCubeCount = 0;
    private int nbNewCube = 0;
    private float countStep = 0f;

    public Transform sun;
    private AutoIntensity sunScript;
    // Use this for initialization
    void Start () {
        nbCube = 0;
        sunScript = sun.GetComponent<AutoIntensity>();
	}
	
	// Update is called once per frame
	void Update () {
        if (countStep <= Time.time)
        {
            //print(transform.childCount);
            newCubeCount = transform.childCount;
            nbNewCube = newCubeCount - nbCube;
            nbCube = newCubeCount;
            countStep = Time.time + 0.1f;
        }

        if(nbNewCube > 0)
        {
            updateCycle(nbNewCube);
            nbNewCube = 0;
        }
    }

    void updateCycle(int nbCube)
    {
        for(int i = 0; i < nbCube; i++)
        {
            sunScript.moveSun(i);

        }
    }
}
