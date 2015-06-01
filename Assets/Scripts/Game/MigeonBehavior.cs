using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Rigidbody))]
[RequireComponent (typeof (AudioSource))] 
public class MigeonBehavior : MonoBehaviour {

	private Transform player;

	public Genetics.GeneticCode code { get; set; }
	protected int stepAction = 0 ;
	protected int repeatAction = 0 ;
	protected float distToFloor = 0;

	protected float autoMoveDistance = 1.0f ;
	protected float speed = 1f ;
	protected float speedRotation = 0.5f ;
	public bool jobToDo {get; private set; }
	protected Vector3 target ;
    protected Vector3 targetJump;
    protected Vector3 eulerAngleTarget;
    //protected Transform playerTransform;
	//protected Vector3 playerPos ;

    public bool isGoingForward { get; private set; }
	public bool isTurning  {get; private set; }
	public bool isJumping  {get; private set; }
	public bool isFalling  {get; private set; }
	public bool wait {get; private set; }
    public bool waitForPlayer {get; private set; }
    public bool wantsToMate { get; set; }
    public bool followPlayer { get; private set; }
	
    public AudioClip[] whatDoWeDo ;
    public AudioClip[] putCubeSounds ;
    public AudioClip[] jumpSounds;
    public AudioClip errorSound;
    public AudioClip backSound;

	public Material cubeMaterial;

	//protected bool inPlayerVicinity = false ;
	public bool isSlave = false ;
	
	public bool carried { get; set; }
	protected bool wasCarried = false ;
	
	protected GameObject parentCube ;
	
	protected Color myBlaze ;

	private float timeBlockedJump = 0.0f;
	private const float timeBlockedJumpMax = 0.5f;
	
   
	// Use this for initialization
	void Start () {
        jobToDo = true;
        wait = true;
        Invoke("endWait", 3.0f);

        if (code == null) //si pas deja set par un instantiate
            code = Genetics.makeGeneticCode();

        distToFloor = GetComponent<Rigidbody>().GetComponent<Collider>().bounds.size.y / 2f;
        player = GameObject.Find("Player").transform;
		carried = false ;
		parentCube = GameObject.Find("cubes") ;
		if(isSlave){
			myBlaze = new Color(0.1f,0.1f,0.9f,0.5f) ;
            jobToDo = false;
            followPlayer = true;
		}else{
			myBlaze = new Color(Random.Range(0.6f,1.0f),Random.value,Random.Range(0.0f,0.5f),0.5f) ;
		}

        transform.GetChild(0).GetChild(1).GetComponent<Renderer>().material.color = myBlaze;
	}

    public void takeControl(bool take)
    {
        carried = take;
        if (carried)
        {    
            wasCarried = true;
            jobToDo = false;
            isJumping = false;
            isFalling = false;
            isTurning = false;
            isGoingForward = false;
            wait = false ;
            waitForPlayer = false;
            wantsToMate = false;
        }
        else if (wasCarried == true)
        {
            wait = true;
            Invoke("endWait", 3.0f);
            wasCarried = false;
            snapToFloor();     
            startJob();
        }
    }
	

	
	void nextStep() {
		wait = true ;
        stepAction++;
        Invoke("endWait", 0.5f);
	}
	
	void endWait(){
		wait = false ;
	}
	
	// Update is called once per frame
	void FixedUpdate() {
        if (carried || wait)
            return;
		
		if(jobToDo && !waitForPlayer){
			doYourJob();
		}else{
			if(transform.position.y > distToFloor+0.2f || isJumping || isTurning){
                if (isTurning){
                    turn (1) ;
                }else if(jump(true)){
					turn (1) ;
				}
            }else{
                waitForPlayer = true;
				transform.LookAt(player.transform);
                if (!GetComponent<AudioSource>().isPlaying) { 
                    GetComponent<AudioSource>().clip = whatDoWeDo[Random.Range(0, whatDoWeDo.Length)];
                    GetComponent<AudioSource>().PlayDelayed(Random.Range(5f, 10f));
                }
            }
			
	        if(isSlave && followPlayer){
                if(Vector3.Distance(player.position, transform.position) > 2.0f){
                    goForward(5.0f, player.position) ;
                }
            }

			/*if(isSlave && !isGoingForward && !inPlayerVicinity){
				Debug.Log("going to my master") ;
				goForward(5.0f, playerPos) ;
			}*/
				
		}
		
		
		RaycastHit hit;
		Physics.Raycast(GetComponent<Rigidbody>().transform.position, -Vector3.up, out hit) ;
		if(!isFalling && hit.distance >= 1f){
			isFalling = true ;
		}else if(isFalling && GetComponent<Rigidbody>().velocity.y < 0.1f && hit.distance <= 1f){
			snapToFloor() ;
			isFalling = false ;
		}else if(isFalling){
			GetComponent<Rigidbody>().AddForce(-Vector3.up*2f,ForceMode.Impulse) ;
		}
	}

	public void startJob(){
		stepAction = 0 ;
		repeatAction = 0 ;
		jobToDo = true ;
        waitForPlayer = false ;
        GetComponent<AudioSource>().PlayOneShot(backSound);
	}

    public void backToWorkNow(){
        if (waitForPlayer){
            startJob();
        }
    }

	void doYourJob(){
		
		switch(code.actions[stepAction]){
			case Genetics.MA.AVANCER :
				if(goForward(autoMoveDistance)){
					 
					nextStep() ;
				}
			break;
			case Genetics.MA.TURN_LEFT :
				if(turn (1)) {
					nextStep() ;
				}
				break ;
			case Genetics.MA.TURN_RIGHT :
				if(turn (2)) {
					nextStep() ;
				}
			break;
			case Genetics.MA.JUMP :
				if(jump ()) {
					nextStep() ;
				}
			break;
			case Genetics.MA.PUT_CUBE :
				if(createBlock()) {
					nextStep() ;
				}
				break;
			case Genetics.MA.PUT_CUBE_UNDER :
				if(createBlock(true)) {
					nextStep() ;
				}
				break;
		}

		if(stepAction >= code.actions.Length){
			stepAction = 0 ;
			repeatAction++ ;
			if(repeatAction >= code.nbRepeat){
				jobToDo = false ;
			}
		}    
	}
	
	bool canIGo(Vector3 direction, float distance){
        RaycastHit hit;
        if(Physics.Raycast(GetComponent<Rigidbody>().transform.position, direction,out hit, distance)){
			return false ;
		}
		return true ;
	}

	bool willFall(){
		RaycastHit hit;
		if(Physics.Raycast(transform.position + transform.forward + transform.up, -transform.up,out hit, 3.0f)){
			return false ;
		}
		return true ;
	}

	bool isCube(Vector3 pos, float dist)
	{
		if (Physics.CheckSphere (pos, dist))
			return true;
		return false;

		/*RaycastHit hit;
		if(Physics.Raycast(pos - Vector3.up, -Vector3.up,out hit, 1.0f)){
			return true ;
		}
		return false ;*/
	}
	
	bool goForward(float moveDistance = 5.0f, Vector3 targetToGo = default(Vector3)){
		if(!isGoingForward){
			if(willFall())
				return true;
			if(targetToGo.magnitude > 0.1f){
				target = Vector3.Normalize(targetToGo-GetComponent<Rigidbody>().position)*5.0f ;
				Debug.Log ("going to "+target) ;
			}else{
				target = transform.forward*moveDistance + GetComponent<Rigidbody>().position ;
				target.x = Mathf.Round(target.x) ;
				target.y = Mathf.Round(target.y) ;
				target.z = Mathf.Round(target.z) ;
			}
			isGoingForward = true ; 
		}
		target.y = GetComponent<Rigidbody>().transform.position.y ;
		Vector3 dir = Vector3.Normalize(target-GetComponent<Rigidbody>().position) ;
		if(!canIGo(dir, moveDistance+0.1f)){
			isGoingForward = false ;
            GetComponent<AudioSource>().PlayOneShot(errorSound);
			return true ;
		}else{
			GetComponent<Rigidbody>().AddForce(dir*5f,ForceMode.Impulse) ;
		}

		if(Vector3.Distance(GetComponent<Rigidbody>().transform.position, target) <= .2f){
			GetComponent<Rigidbody>().MovePosition(target) ;
			snapToFloor() ;
			isFalling = false ;
			isGoingForward = false ;
			GetComponent<Rigidbody>().velocity = new Vector3(0f,0f,0f) ;
			return true ;
		}
		return false ;
	}

	bool turn(int direction){
		if(!isTurning){
			if (direction == 1) {
				//turn left
				eulerAngleTarget = Quaternion.Euler(GetComponent<Rigidbody>().rotation.eulerAngles + new Vector3(0f,-90f,0f)).eulerAngles ;
			}else{
				//turn right
				eulerAngleTarget = Quaternion.Euler(GetComponent<Rigidbody>().rotation.eulerAngles + new Vector3(0f,90f,0f)).eulerAngles ;
			}

			isTurning = true ;
		}
		
		float step = speedRotation * Time.deltaTime *100f ;
		Quaternion deltaRotation = Quaternion.Euler(eulerAngleTarget * step);

		GetComponent<Rigidbody>().MoveRotation(Quaternion.RotateTowards(GetComponent<Rigidbody>().rotation, Quaternion.Euler(eulerAngleTarget), step)) ;
		if(Vector3.Distance(GetComponent<Rigidbody>().rotation.eulerAngles, eulerAngleTarget) <= 0.2f){
			GetComponent<Rigidbody>().MoveRotation(Quaternion.Euler(eulerAngleTarget)) ;
			isTurning = false ;
			return true ;
		}
		return false ; 
	}

	public bool jump(bool allowFall = false){


		//if(canIGo(Vector3.Normalize(transform.forward+transform.up),1.1f)){
		if(!isJumping){
			if(willFall() && !allowFall)
				return true;
			targetJump = GetComponent<Rigidbody>().transform.position + (transform.forward*1.0f + transform.up) ;
			isJumping = true;
			GetComponent<Rigidbody>().AddForce((transform.up)*110f,ForceMode.Impulse) ;
            GetComponent<AudioSource>().PlayOneShot(jumpSounds[Random.Range(0, jumpSounds.Length)]);
		}
		
		
		if(isJumping){		
			/*if(targetJump.y - GetComponent<Rigidbody>().transform.position.y >= 1.0f){
				//GetComponent<Rigidbody>().AddForce((transform.forward)*0.5f,ForceMode.Impulse) ;
			}else if(targetJump.y - GetComponent<Rigidbody>().transform.position.y >= -0.5f){
				GetComponent<Rigidbody>().AddForce((transform.forward)*1.5f,ForceMode.Impulse) ;
				isFalling = true ;
			}else{
				GetComponent<Rigidbody>().velocity = new Vector3(0,GetComponent<Rigidbody>().velocity.y,0) ;
			}*/

			if(transform.position.y-targetJump.y >= 0.0)
				GetComponent<Rigidbody>().AddForce((transform.forward)*4f,ForceMode.Impulse) ;
			else
				GetComponent<Rigidbody>().velocity = new Vector3(0,GetComponent<Rigidbody>().velocity.y,0) ;

			Vector3 vitHor = new Vector3(GetComponent<Rigidbody>().velocity.x,0,GetComponent<Rigidbody>().velocity.z);
			//Debug.Log (vitHor.magnitude);
			if(vitHor.magnitude > 1.0){
				vitHor = vitHor.normalized * 1.0f;
				GetComponent<Rigidbody>().velocity = new Vector3(vitHor.x,GetComponent<Rigidbody>().velocity.y,vitHor.z);
			}
		}

		RaycastHit hit;
		Physics.Raycast(GetComponent<Rigidbody>().transform.position, -Vector3.up, out hit) ;
		if (GetComponent<Rigidbody> ().velocity.y < -0.1f && hit.distance <= 1f) {
			GetComponent<Rigidbody> ().MovePosition (new Vector3 (targetJump.x, GetComponent<Rigidbody> ().transform.position.y, targetJump.z));
			snapToFloor ();
			isJumping = false;
			isFalling = false;
			GetComponent<Rigidbody> ().velocity = new Vector3 (0f, 0f, 0f);
			return true;
		} 

		//Bloque
		if(GetComponent<Rigidbody> ().velocity.magnitude < 1.0)
		{
			timeBlockedJump += Time.deltaTime;
		}
		else
			timeBlockedJump = 0.0f;

		if(timeBlockedJump > timeBlockedJumpMax)
		{
			timeBlockedJump = 0.0f;
			GetComponent<Rigidbody>().AddForce(Random.onUnitSphere*100f,ForceMode.Impulse) ;
		}

		return false ;
	}

	bool createBlock(bool under = false){
		float distance = 1 ;

		Vector3 newPos = GetComponent<Rigidbody>().position + transform.forward*distance ;
		newPos.x = Mathf.Round(newPos.x) ;
		newPos.y =	Mathf.Floor(newPos.y)+0.5f ;
		if (under)
			newPos.y -= 1.0f;
		newPos.z = Mathf.Round(newPos.z) ;


		if(!isCube(newPos, 0.3f)){
			GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
			cube.transform.position = newPos ;
			cube.transform.parent = parentCube.transform ;
			cube.GetComponent<MeshRenderer>().material = cubeMaterial;
			cube.GetComponent<MeshRenderer>().material.color = myBlaze ;
            GetComponent<AudioSource>().PlayOneShot(putCubeSounds[Random.Range(0, putCubeSounds.Length)]);
        }
        else{
            GetComponent<AudioSource>().PlayOneShot(errorSound);
        }
		return true ;
	}
	
	void snapToFloor(){
		// Save current object layer
		int oldLayer = gameObject.layer;
		
		//Change object layer to a layer it will be alone
		//gameObject.layer = LayerMask.NameToLayer("Ghost");
		 
		int layerToIgnore = 1 << gameObject.layer;
		layerToIgnore = ~layerToIgnore;

		RaycastHit hit;
		//if (Physics.Raycast(transform.position, -Vector3.up, out hit,1.0f,layerToIgnore)){
        if (Physics.Raycast(transform.position, -Vector3.up, out hit)){
			if(Vector3.Distance(transform.position,hit.point+new Vector3(0.0f,distToFloor,0.0f))>=0.2f){
				//Debug.Log ("snap :"+hit.collider.gameObject.name) ;
				transform.position = new Vector3(Mathf.Round(hit.point.x), Mathf.Round(hit.point.y), Mathf.Round(hit.point.z))+new Vector3(0.0f,distToFloor,0.0f) ;
				GetComponent<Rigidbody>().velocity = new Vector3(0f,0f,0f) ;
                GetComponent<Rigidbody>().angularVelocity = new Vector3(0f, 0f, 0f);
			}
		}
		float newY = Mathf.Round(transform.rotation.eulerAngles.y / 90.0f) * 90.0f ;
		transform.rotation = Quaternion.Euler(0.0f,newY,0.0f) ;
		//gameObject.layer = oldLayer;
	}
}