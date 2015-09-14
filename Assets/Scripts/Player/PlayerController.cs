using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

public class PlayerController : MonoBehaviour {


	private GameObject grabMigeonLabel;
	private GameObject releaseMigeonLabel;
    private GameObject mateMigeonLabel;

    private Camera playerCamera;

    Transform carriedMigeon = null;
    Transform migeonItWantsToFuck = null;
    float timeWantToFuck = 0;
 
	// Use this for initialization
	void Start ()
	{
		grabMigeonLabel = GameObject.Find("GrabMigeonLabel");
		releaseMigeonLabel = GameObject.Find("ReleaseMigeonLabel");
        mateMigeonLabel = GameObject.Find("MateMigeonLabel");

        setLabel(MESSAGE.NONE);

        playerCamera = GameObject.Find("FirstPersonCharacter").GetComponent<Camera>();
	}

    private enum MESSAGE{
        NONE = 0,
        TAKE,
        RELEASE,
        MATE
    }

    void setLabel(MESSAGE mess)
    {
        if (grabMigeonLabel != null)
            grabMigeonLabel.SetActive(mess == MESSAGE.TAKE);

        if (releaseMigeonLabel != null)
            releaseMigeonLabel.SetActive(mess == MESSAGE.RELEASE);

        if (mateMigeonLabel != null)
            mateMigeonLabel.SetActive(mess == MESSAGE.MATE);
    }
	
	// Update is called once per frame
	void LateUpdate ()
	{
        if(carriedMigeon == null)
        {
            RaycastHit hit;
            bool found = false;
            if(Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, 3f, LayerMask.GetMask("Migeon")))
                found = true;
            if(!found)
                if(Physics.Raycast(playerCamera.transform.position,  Quaternion.AngleAxis(-10, Vector3.up) * playerCamera.transform.forward, out hit, 3f, LayerMask.GetMask("Migeon")))
                    found = true;
            if(!found)
                if(Physics.Raycast(playerCamera.transform.position, Quaternion.AngleAxis(+10, Vector3.up) * playerCamera.transform.forward, out hit, 3f, LayerMask.GetMask("Migeon")))
                    found = true;

            if (found)
            {
                setLabel(MESSAGE.TAKE);

                if (Input.GetKeyDown(KeyCode.E) || Input.GetButtonDown("Fire1"))
                {
                    carriedMigeon = hit.collider.transform;
                    carriedMigeon.GetComponent<MigeonBehavior>().takeControl(true);
                    carriedMigeon.GetComponent<Rigidbody>().isKinematic = true;
                }
            }
            else
            {
                setLabel(MESSAGE.NONE);
            }
        }
        else
		{
            //On porte un migeon
            carriedMigeon.transform.LookAt(carriedMigeon.transform.position + transform.forward, Vector3.up); 
            Vector3 posMigeonInArms = playerCamera.transform.position + playerCamera.transform.forward * 2f;
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
                setLabel(MESSAGE.MATE);
            else
                if(carriedMigeon)
                    setLabel(MESSAGE.RELEASE);

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

            bool releaseMigeon = false;

            if (CrossPlatformInputManager.GetButtonDown("Fire2"))
            {
                Genetics.GeneticCode code = carriedMigeon.GetComponent<MigeonBehavior>().code.createCopy();
                Genetics.mutateOne(ref code);

                Transform nouveauMigeon = GameObject.Instantiate(carriedMigeon, carriedMigeon.position + carriedMigeon.forward*2, Quaternion.identity) as Transform;
                nouveauMigeon.GetComponent<Rigidbody>().isKinematic = false;
                nouveauMigeon.GetComponent<MigeonBehavior>().code = code;
                nouveauMigeon.GetComponent<Rigidbody>().velocity = Vector3.up * 7f;
                releaseMigeon = true;
            }

            if (Input.GetButtonDown("Fire1") || Input.GetKeyDown(KeyCode.E))
            {
                if (migeonItWantsToFuck)
                {
                    Genetics.GeneticCode code1 = carriedMigeon.GetComponent<MigeonBehavior>().code;
                    Genetics.GeneticCode code2 = migeonItWantsToFuck.GetComponent<MigeonBehavior>().code;
                    Genetics.GeneticCode code3 = Genetics.crossOverAndOptim(code1, code2);
                    migeonItWantsToFuck.GetComponent<MigeonBehavior>().startJob();
                    Transform nouveauMigeon = GameObject.Instantiate(carriedMigeon, (carriedMigeon.position + migeonItWantsToFuck.position) / 2, Quaternion.identity) as Transform;
                    
                    nouveauMigeon.GetComponent<Rigidbody>().isKinematic = false;
                    nouveauMigeon.GetComponent<MigeonBehavior>().code = code3;
                    nouveauMigeon.GetComponent<Rigidbody>().velocity = Vector3.up * 7f;
                    releaseMigeon = true;   
                }
            }

            if (Input.GetKeyDown(KeyCode.E) || Input.GetButtonDown("Fire1"))
			{
                releaseMigeon = true;
			}

            if (releaseMigeon)
            {
                carriedMigeon.GetComponent<Rigidbody>().isKinematic = false;
                carriedMigeon.GetComponent<MigeonBehavior>().takeControl(false);
                carriedMigeon.GetComponent<MigeonBehavior>().wantsToMate = false;
                carriedMigeon = null;
                migeonItWantsToFuck = null;

                setLabel(MESSAGE.NONE);
            }
		}
	}
}
