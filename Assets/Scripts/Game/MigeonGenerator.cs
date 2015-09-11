﻿using UnityEngine;
using System.Collections;

public class MigeonGenerator : MonoBehaviour {
	public Rigidbody migeon ;
	private GameObject parentMigeon ;
    public int moreMigeons = 10;

    private Transform player;
	// Use this for initialization
	void Start () {
        player = GameObject.Find("Player").transform;
		parentMigeon = GameObject.Find("migeons") ;
		for(int i = 0 ; i <= Genetics.GeneticCode.getNbDesign() ; i++){
			spawnMigeon (false,i) ;
		}
		for(int i = 0 ; i <= moreMigeons ; i++){
			spawnMigeon (false) ;
		}
	}
	
	void spawnMigeon(bool slave, int numDesign = -1){
		Vector3 position = Random.insideUnitSphere * 50 ;
		position.x = Mathf.Round (position.x) ;
		position.y = 0.35f ;
		position.z = Mathf.Round (position.z) ;
		while(Physics.CheckSphere(position, 3f,LayerMask.GetMask("Migeons"))){
			Debug.Log ("Someone already here !") ;   
			position = Random.insideUnitSphere * 10 ;
			position.y = 1.0f ;
		}
		      //if(hitColliders.Length > 0.1) //You have someone with a collider here
		float yRot = 90f*Random.Range (0,4) ;
		Quaternion rotation = Quaternion.Euler(0.0f,yRot,0.0f) ;
		Rigidbody migeon1 = (Rigidbody) Instantiate(migeon, position, rotation);
		if (numDesign >= 0) {
			migeon1.GetComponent<MigeonBehavior> ().code = new Genetics.GeneticCode ();
			migeon1.GetComponent<MigeonBehavior> ().code.fillWithDesigned (numDesign);
			Genetics.GeneticCode code = migeon1.GetComponent<MigeonBehavior>().code;
			Genetics.mutateOne(ref code);
		}
		migeon1.gameObject.transform.parent = parentMigeon.transform ;
		migeon1.GetComponent<MigeonBehavior>().isSlave = slave ;
	}

    public void backToWork(){
        Collider[] hitColliders = Physics.OverlapSphere(player.position, 30f, LayerMask.GetMask("Migeon"));
        int i = 0;
        while (i < hitColliders.Length) {
            hitColliders[i].SendMessage("backToWorkNow");
            i++;
        }
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown("m"))
        {
            backToWork();
        }
	}
}
