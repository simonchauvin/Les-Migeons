using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {


	private GameObject grabMigeonLabel;
	private GameObject releaseMigeonLabel;

    Transform carriedMigeon = null;
    Transform migeonItWantsToFuck = null;
 
	// Use this for initialization
	void Start ()
	{
		grabMigeonLabel = GameObject.Find("GrabMigeonLabel");
		releaseMigeonLabel = GameObject.Find("ReleaseMigeonLabel");

        if (grabMigeonLabel != null)
            grabMigeonLabel.SetActive(false);

        if (releaseMigeonLabel != null)
            releaseMigeonLabel.SetActive(false);
	}
	
	// Update is called once per frame
	void Update ()
	{
        if(carriedMigeon == null)
        {
            RaycastHit hit;
            bool found = false;
            if(Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, 3f, LayerMask.GetMask("Migeon")))
                found = true;
            if(!found)
                if(Physics.Raycast(Camera.main.transform.position,  Quaternion.AngleAxis(-10, Vector3.up) * Camera.main.transform.forward, out hit, 3f, LayerMask.GetMask("Migeon")))
                    found = true;
            if(!found)
                if(Physics.Raycast(Camera.main.transform.position, Quaternion.AngleAxis(+10, Vector3.up) * Camera.main.transform.forward, out hit, 3f, LayerMask.GetMask("Migeon")))
                    found = true;

            if (found)
            {
                if (grabMigeonLabel != null)
                    grabMigeonLabel.SetActive(true);

                if (releaseMigeonLabel != null)
                    releaseMigeonLabel.SetActive(false);

                if (Input.GetKeyDown(KeyCode.E))
                {
                    carriedMigeon = hit.collider.transform;
                    carriedMigeon.GetComponent<MigeonBehavior>().carried = true;
                }
            }
            else
            {
                if (grabMigeonLabel != null)
                    grabMigeonLabel.SetActive(false);

                if (releaseMigeonLabel != null)
                    releaseMigeonLabel.SetActive(false);
            }
        }
        else
		{
            //On porte un migeon
            RaycastHit hit;
            if (Physics.Raycast(carriedMigeon.transform.position, carriedMigeon.transform.forward, out hit, 3f, LayerMask.GetMask("Migeon")))
                migeonItWantsToFuck = hit.transform;
            else
                migeonItWantsToFuck = null;


			if (grabMigeonLabel != null)
				grabMigeonLabel.SetActive(false);
			
			if (releaseMigeonLabel != null)
				releaseMigeonLabel.SetActive(true);
			
			if (Input.GetKeyDown(KeyCode.E))
			{
				carriedMigeon.GetComponent<MigeonBehavior>().carried = false;
                carriedMigeon = null;
                if (grabMigeonLabel != null)
				    grabMigeonLabel.SetActive(false);

			    if (releaseMigeonLabel != null)
				    releaseMigeonLabel.SetActive(false);

			}
		}
	}
}
