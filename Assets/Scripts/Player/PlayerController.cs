using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {


	private GameObject grabMigeonLabel;
	private GameObject releaseMigeonLabel;


	// Use this for initialization
	void Start ()
	{
		grabMigeonLabel = GameObject.Find("GrabMigeonLabel");
		releaseMigeonLabel = GameObject.Find("ReleaseMigeonLabel");
	}
	
	// Update is called once per frame
	void Update ()
	{
		RaycastHit hit;
		if (Physics.Raycast(transform.position, transform.forward, out hit, 2f, LayerMask.GetMask("Migeon")))
		{
			bool isCarried = hit.collider.GetComponent<MigeonBehavior>().carried;
			if (isCarried)
			{
				grabMigeonLabel.SetActive(false);
				releaseMigeonLabel.SetActive(true);
				if (Input.GetKeyDown(KeyCode.E))
				{
					hit.collider.GetComponent<MigeonBehavior>().carried = false;
				}
			}
			else
			{
				grabMigeonLabel.SetActive(true);
				releaseMigeonLabel.SetActive(false);
				if (Input.GetKeyDown(KeyCode.E))
				{
					hit.collider.GetComponent<MigeonBehavior>().carried = true;
				}
			}
		}
		else
		{
			grabMigeonLabel.SetActive(false);
			releaseMigeonLabel.SetActive(false);
		}
	}
}
