using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {


	private GameObject grabMigeonLabel;
	private GameObject releaseMigeonLabel;

    Transform carriedMigeon = null;
    Transform migeonItWantsToFuck = null;
    float timeWantToFuck = 0;
 
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
	void LateUpdate ()
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
                    carriedMigeon.GetComponent<MigeonBehavior>().takeControl(true);
                    carriedMigeon.rigidbody.isKinematic = true;
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
            carriedMigeon.transform.LookAt(carriedMigeon.transform.position + transform.forward, Vector3.up); 
            Vector3 posMigeonInArms = Camera.main.transform.position + Camera.main.transform.forward * 2f;
            carriedMigeon.transform.position = posMigeonInArms;

            //On regarde s'il veut niquer
            if (migeonItWantsToFuck == null)
            {
                RaycastHit hit;
                if (Physics.Raycast(carriedMigeon.transform.position, carriedMigeon.transform.forward, out hit, 3f, LayerMask.GetMask("Migeon")))
                {
                    timeWantToFuck = 0;
                    migeonItWantsToFuck = hit.transform;
                    carriedMigeon.GetComponent<MigeonBehavior>().wantsToMate = true;
                }
            }

            if (migeonItWantsToFuck != null)
            {
                Vector3 directionMigeon = migeonItWantsToFuck.position - carriedMigeon.position;
                timeWantToFuck += Time.deltaTime;

                //Si trop grande distance
                if (directionMigeon.magnitude > 6.0f)
                {
                    migeonItWantsToFuck = null;
                    carriedMigeon.GetComponent<MigeonBehavior>().wantsToMate = false;
                }
                else
                {
                    if (directionMigeon.magnitude > 1)
                        directionMigeon.Normalize();

                    carriedMigeon.transform.position = Vector3.Lerp(posMigeonInArms, posMigeonInArms + directionMigeon, timeWantToFuck);
                    directionMigeon.y = 0;
                    carriedMigeon.transform.LookAt(carriedMigeon.transform.position + directionMigeon, Vector3.up); 
                } 
            }


			if (grabMigeonLabel != null)
				grabMigeonLabel.SetActive(false);
			
			if (releaseMigeonLabel != null)
				releaseMigeonLabel.SetActive(true);

            bool releaseMigeon = false;

            if (Input.GetButtonDown("Fire2"))
            {
                Genetics.GeneticCode code = carriedMigeon.GetComponent<MigeonBehavior>().code.createCopy();
                Genetics.mutate(ref code);

                Transform nouveauMigeon = GameObject.Instantiate(carriedMigeon, carriedMigeon.position + carriedMigeon.forward*2, Quaternion.identity) as Transform;
                nouveauMigeon.rigidbody.isKinematic = false;
                nouveauMigeon.GetComponent<MigeonBehavior>().code = code;
                nouveauMigeon.rigidbody.velocity = Vector3.up * 7f;
                releaseMigeon = true;
            }

            if (Input.GetButtonDown("Fire1"))
            {
                if (migeonItWantsToFuck)
                {
                    Genetics.GeneticCode code1 = carriedMigeon.GetComponent<MigeonBehavior>().code;
                    Genetics.GeneticCode code2 = migeonItWantsToFuck.GetComponent<MigeonBehavior>().code;
                    Genetics.GeneticCode code3 = Genetics.crossOver(code1, code2);
                    migeonItWantsToFuck.GetComponent<MigeonBehavior>().startJob();
                    Transform nouveauMigeon = GameObject.Instantiate(carriedMigeon, (carriedMigeon.position + migeonItWantsToFuck.position) / 2, Quaternion.identity) as Transform;
                    
                    nouveauMigeon.rigidbody.isKinematic = false;
                    nouveauMigeon.GetComponent<MigeonBehavior>().code = code3;
                    nouveauMigeon.rigidbody.velocity = Vector3.up * 7f;
                    releaseMigeon = true;   
                }
            }
			
			if (Input.GetKeyDown(KeyCode.E))
			{
                releaseMigeon = true;
			}

            if (releaseMigeon)
            {
                carriedMigeon.rigidbody.isKinematic = false;
                carriedMigeon.GetComponent<MigeonBehavior>().takeControl(false);
                carriedMigeon = null;
                migeonItWantsToFuck = null;
                carriedMigeon.GetComponent<MigeonBehavior>().wantsToMate = false;
                if (grabMigeonLabel != null)
                    grabMigeonLabel.SetActive(false);

                if (releaseMigeonLabel != null)
                    releaseMigeonLabel.SetActive(false);
            }
		}
	}
}
