using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Net ;

public class NKlog : MonoBehaviour {

	private StreamWriter writer ;

	// Use this for initialization
	void Awake () {
		//DontDestroyOnLoad(transform.gameObject);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void createFile(string fileName){
		if(Application.isWebPlayer){
			//online version
		}else{
			writer = new StreamWriter("Logs/"+fileName, true) ;
			writer.AutoFlush = true ;
			writer.WriteLine("<Events>");
		}
	}

    public void openFile(string fileName)
    {
        if (Application.isWebPlayer)
        {
            //online version
        }
        else
        {
            writer = new StreamWriter("Logs/" + fileName, true);
            writer.AutoFlush = true;
            writer.WriteLine("<Events>");
        }
    }

    public void writeContent(int eventid, string eventname, string x = "", string y = "", string z = "", string dirx = "", string diry = "", string dirz = "",  string param3 = "", string param4 = "", string param5 = "", string param6 = "", string param7 = ""){
		if(Application.isWebPlayer){
			//online version
		}else{
			string line ;
			if(x != ""){
				line = "<Event Time=\""+string.Format("{0:0.00000}", Time.time)+"\" Type=\""+eventid.ToString()+"\" Name=\""+eventname+"\" x=\""+x+"\" y=\""+y+"\" z=\""+z+"\"" +
                " dirx =\"" + dirx + "\" diry=\"" + diry + "\" dirz=\"" + dirz + "\"" +
                " Valeur0=\"" +param3+"\" Valeur1=\""+param4+"\" Valeur2=\""+param5+"\" Valeur3=\""+param6+"\" Valeur4=\""+param7+"\" />" ;
			}else{
				line = "<Event Time=\""+string.Format("{0:0.00000}", Time.time)+"\" Type=\""+eventid.ToString()+"\" Name=\""+eventname+"\"" +
					" Valeur0=\""+param3+"\" Valeur1=\""+param4+"\" Valeur2=\""+param5+"\" Valeur3=\""+param6+"\" Valeur4=\""+param7+"\" />" ;
			}
			writer.WriteLine(line);
		}

	}

	public void releaseFile(){
		if(Application.isWebPlayer){
			//online version
		}else{
			writer.WriteLine("</Events>");
			writer.Close() ;
		}

	}

	void OnDestroy(){
		releaseFile() ;
	}
	/*
	static public bool IsConnectionToServerOk () {
		WebRequest request = HttpWebRequest.Create("http://www.interaction-project.net/research/NKEvalTool/Logs/test.php");
		request.Method = "HEAD";
		WebResponse resp = null;
		try {
			resp = request.GetResponse();
		} catch {
			resp = null;
		}
		return resp != null;
	}
	*/
	
} 