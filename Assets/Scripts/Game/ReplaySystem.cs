using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class ReplaySystem : MonoBehaviour {

    private bool logging = false ;
    private string logFunc = "" ;
    private NKlog nklog ;

    private string filename ;

    // Use this for initialization
    void Start () {
        DateTime thisDate1 = DateTime.Now;
        filename = "userLog" + thisDate1.ToString("yyyyMMdd_Hmmss") + ".txt";
        nklog = GetComponent<NKlog>();
        nklog.createFile(filename);
        startLog() ;
	}

    void startLog()
    {
        if (logFunc != "")
            InvokeRepeating(logFunc, 0, 0.3F);
        logging = true;
    }

    void stopLog()
    {
        if (logFunc != "")
            CancelInvoke(logFunc);
        logging = false;
    }

    public void eventToLog(int eventid, string eventname, Vector3 position, Vector3 direction, string param3 = "", string param4 = "", string param5 = "", string param6 = "", string param7 = "")
    {
        //event id : 0 = new cube (color + pos)
        //event id : 1 = new migeons (color + pos)
        //event id : 2 = migeons position + animation and sound to play ?
        //Debug.Log (eventname+","+param3) ;
        if (logging)
        {
            nklog.writeContent(eventid, eventname, position.x.ToString(), position.y.ToString(), position.z.ToString(), direction.x.ToString(), direction.y.ToString(), direction.z.ToString(), param3, param4, param5, param6, param7);
        }
    }

    void autoLog()
    {

    }

    // Update is called once per frame
    void Update () {
	
	}
}
