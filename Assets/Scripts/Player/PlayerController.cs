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
		if (Physics.Raycast(transform.position, Camera.main.transform.forward, out hit, 2f, LayerMask.GetMask("Migeon")))
		{
			bool isCarried = hit.collider.GetComponent<MigeonBehavior>().carried;
			if (isCarried)
			{
				if (grabMigeonLabel != null)
				{
					grabMigeonLabel.SetActive(false);
				}
				if (releaseMigeonLabel != null)
				{
					releaseMigeonLabel.SetActive(true);
				}
				if (Input.GetKeyDown(KeyCode.E))
				{
					hit.collider.GetComponent<MigeonBehavior>().carried = false;
				}
			}
			else
			{
				if (grabMigeonLabel != null)
				{
					grabMigeonLabel.SetActive(true);
				}
				if (releaseMigeonLabel != null)
				{
					releaseMigeonLabel.SetActive(false);
				}
				if (Input.GetKeyDown(KeyCode.E))
				{
					hit.collider.GetComponent<MigeonBehavior>().carried = true;
				}
			}
		}
		else
		{
			if (grabMigeonLabel != null)
			{
				grabMigeonLabel.SetActive(false);
			}
			if (releaseMigeonLabel != null)
			{
				releaseMigeonLabel.SetActive(false);
			}
		}
	}
}
