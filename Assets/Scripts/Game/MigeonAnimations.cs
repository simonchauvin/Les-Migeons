﻿using UnityEngine;
using System.Collections;

public class MigeonAnimations : MonoBehaviour {

	private Animator animator;

	private MigeonBehavior behaviorScript;

	// Use this for initialization
	void Start ()
	{
		animator = GetComponentInChildren<Animator>();
		behaviorScript = GetComponentInChildren<MigeonBehavior>();
	}
	
	// Update is called once per frame
	void Update ()
	{
		animator.SetBool("carried", behaviorScript.carried);
		animator.SetBool("isGoingForward", behaviorScript.isGoingForward || behaviorScript.isTurning);
		animator.SetBool("noJob", behaviorScript.wait);
		animator.SetBool("awaiting", behaviorScript.waitForPlayer);
        animator.SetBool("wantsToMate", behaviorScript.wantsToMate);
	}
}
