using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Rigidbody))]
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
	protected Vector3 playerPos ;

    public bool isGoingForward { get; private set; }
	public bool isTurning  {get; private set; }
	public bool isJumping  {get; private set; }
	public bool isFalling  {get; private set; }
	public bool wait {get; private set; }
	
    public AudioSource audio;
    public AudioClip[] whatDoWeDo ;
    public AudioClip[] putCube ;

	protected bool inPlayerVicinity = false ;
	public bool isSlave = false ;
	
	public bool carried { get; set; }
	protected bool wasCarried = false ;
	
	protected GameObject parentCube ;
	
	protected Color myBlaze ;
	
	// Use this for initialization
	void Start () {
        jobToDo = true;
        wait = true;
        Invoke("endWait", 3.0f);

        if (code == null) //si pas deja set par un instantiate
            code = Genetics.makeGeneticCode();

        distToFloor = rigidbody.collider.bounds.size.y / 2f;
        player = GameObject.Find("Player").transform;
		carried = false ;
		parentCube = GameObject.Find("cubes") ;
		if(isSlave){
			myBlaze = new Color(Random.Range(0f,0.5f),Random.value,Random.Range(0.6f,1f),0.5f) ;
		}else{
			myBlaze = new Color(Random.Range(0.6f,1.0f),Random.value,Random.Range(0.0f,0.5f),0.5f) ;
		}

        transform.GetChild(0).GetChild(1).renderer.material.color = myBlaze;
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

		/*playerPos = player.transform.position ;
		if(Vector3.Distance(playerPos, transform.position) <= 2.0f){
			inPlayerVicinity = true ;
		}else{
			inPlayerVicinity = false ;
		}*/
	
		
		if(jobToDo){
			doYourJob();
		}else{
			if(transform.position.y > distToFloor || isJumping){
				if(jump()){
					turn (1) ;
				}
            }
            else{
                //audio.PlayOneShot(clipStorage[Random.Range(0,clipStorage.Length)]);
            }
				
			/*if(isSlave && !isGoingForward && !inPlayerVicinity){
				Debug.Log("going to my master") ;
				goForward(5.0f, playerPos) ;
			}*/
				
		}
		
		
		RaycastHit hit;
		Physics.Raycast(rigidbody.transform.position, -Vector3.up, out hit) ;
		if(!isFalling && hit.distance >= 1f){
			isFalling = true ;
		}else if(isFalling && rigidbody.velocity.y < 0.1f && hit.distance <= 1f){
			snapToFloor() ;
			isFalling = false ;
		}else if(isFalling){
			rigidbody.AddForce(-Vector3.up*2f,ForceMode.Impulse) ;
		}
	}

	public void startJob(){
		stepAction = 0 ;
		repeatAction = 0 ;
		jobToDo = true ;
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
        if(Physics.Raycast(rigidbody.transform.position, direction,out hit, distance)){
			return false ;
		}
		return true ;
	}
	
	bool goForward(float moveDistance = 5.0f, Vector3 targetToGo = default(Vector3)){
		if(!isGoingForward){
			if(targetToGo.magnitude > 0.1f){
				target = Vector3.Normalize(target-rigidbody.position)*5.0f ;
				Debug.Log ("going to "+target) ;
			}else{
				target = transform.forward*moveDistance + rigidbody.position ;
				target.x = Mathf.Round(target.x) ;
				target.y = Mathf.Round(target.y) ;
				target.z = Mathf.Round(target.z) ;
			}
			isGoingForward = true ;
		}
		target.y = rigidbody.transform.position.y ;
		Vector3 dir = Vector3.Normalize(target-rigidbody.position) ;
		if(!canIGo(dir, moveDistance+0.1f)){
			isGoingForward = false ;
			return true ;
		}else{
			rigidbody.AddForce(dir*5f,ForceMode.Impulse) ;
		}

		if(Vector3.Distance(rigidbody.transform.position, target) <= .2f){
			rigidbody.MovePosition(target) ;
			snapToFloor() ;
			isFalling = false ;
			isGoingForward = false ;
			rigidbody.velocity = new Vector3(0f,0f,0f) ;
			return true ;
		}
		return false ;
	}

	bool turn(int direction){
		if(!isTurning){
			if (direction == 1) {
				//turn left
				eulerAngleTarget = Quaternion.Euler(rigidbody.rotation.eulerAngles + new Vector3(0f,-90f,0f)).eulerAngles ;
			}else{
				//turn right
				eulerAngleTarget = Quaternion.Euler(rigidbody.rotation.eulerAngles + new Vector3(0f,90f,0f)).eulerAngles ;
			}

			isTurning = true ;
		}
		
		float step = speedRotation * Time.deltaTime *100f ;
		Quaternion deltaRotation = Quaternion.Euler(eulerAngleTarget * step);

		rigidbody.MoveRotation(Quaternion.RotateTowards(rigidbody.rotation, Quaternion.Euler(eulerAngleTarget), step)) ;
		if(Vector3.Distance(rigidbody.rotation.eulerAngles, eulerAngleTarget) <= 0.2f){
			rigidbody.MoveRotation(Quaternion.Euler(eulerAngleTarget)) ;
			isTurning = false ;
			return true ;
		}
		return false ;
	}

	public bool jump(){
		//if(canIGo(Vector3.Normalize(transform.forward+transform.up),1.1f)){
			if(!isJumping){
				targetJump = rigidbody.transform.position + (transform.forward*1.0f + transform.up) ;
				isJumping = true ;;
				rigidbody.AddForce((transform.up)*110f,ForceMode.Impulse) ;
			}
		
		
		if(isJumping){		
			if(targetJump.y - rigidbody.transform.position.y >= 1.0f){
				rigidbody.AddForce((transform.forward)*0.5f,ForceMode.Impulse) ;
			}else if(targetJump.y - rigidbody.transform.position.y >= -1.0f){
				rigidbody.AddForce((transform.forward)*1.2f,ForceMode.Impulse) ;
				isFalling = true ;
			}
		}
		RaycastHit hit;
		Physics.Raycast(rigidbody.transform.position, -Vector3.up, out hit) ;
		if(isFalling && rigidbody.velocity.y <= 0.1f && hit.distance <= 1f){
			rigidbody.MovePosition(new Vector3(targetJump.x, rigidbody.transform.position.y, targetJump.z)) ;
			snapToFloor() ;
			isJumping = false ;
			isFalling = false ;
			rigidbody.velocity = new Vector3(0f,0f,0f) ;
			return true ;
		}
		return false ;
	}

	bool createBlock(){
		float distance = 1 ;
		if(canIGo(rigidbody.transform.forward, distance)){
			GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
			Vector3 newPos = rigidbody.position + transform.forward*distance ;
			newPos.x = Mathf.Round(newPos.x) ;
			newPos.y = Mathf.Round((newPos.y+0.5f-distToFloor))+0.5f ;
			newPos.z = Mathf.Round(newPos.z) ;
			cube.transform.position = newPos ;
			cube.transform.parent = parentCube.transform ;
			cube.renderer.material.color = myBlaze ;
		}
		return true ;
	}
	
	void snapToFloor(){
		// Save current object layer
		int oldLayer = gameObject.layer;
		
		//Change object layer to a layer it will be alone
		gameObject.layer = LayerMask.NameToLayer("Ghost");
		
		int layerToIgnore = 1 << gameObject.layer;
		layerToIgnore = ~layerToIgnore;

		RaycastHit hit;
		if (Physics.Raycast(transform.position, -Vector3.up, out hit,1.0f,layerToIgnore)){
			if(Vector3.Distance(transform.position,hit.point+new Vector3(0.0f,distToFloor,0.0f))>=0.2f){
				//Debug.Log ("snap :"+hit.collider.gameObject.name) ;
				transform.position = new Vector3(Mathf.Round(hit.point.x), Mathf.Round(hit.point.y), Mathf.Round(hit.point.z))+new Vector3(0.0f,distToFloor,0.0f) ;
				rigidbody.velocity = new Vector3(0f,0f,0f) ;
                rigidbody.angularVelocity = new Vector3(0f, 0f, 0f);
			}
		}
		float newY = Mathf.Round(transform.rotation.eulerAngles.y / 90.0f) * 90.0f ;
		transform.rotation = Quaternion.Euler(0.0f,newY,0.0f) ;
		gameObject.layer = oldLayer;
	}
}