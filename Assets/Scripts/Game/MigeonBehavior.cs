using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Rigidbody))]
public class MigeonBehavior : MonoBehaviour {

	private Transform player;

	protected int[] actionsList ;
	protected int maxActions ;
	protected int stepAction = 1 ;
	protected int repeatAction = 0 ;
	protected float distToFloor = 0.5f ;

	protected float moveDistance = 5.0f ;
	protected float speed = 1f ;
	protected float speedRotation = 0.5f ;
	protected bool jobToDo = true ;
	public Vector3 target ;
	public Vector3 eulerAngleTarget ;
	
	protected bool isGoingForward = false ;
	protected bool isTurning = false ;
	
	public bool carried { get; set; }
	protected bool wasCarried = false ;
	
	// Use this for initialization
	void Start () {
		player = GameObject.Find("Player").transform;

		carried = false ;
		//maxActions = Random.Range (5, 10);
		maxActions = 5 ;
		actionsList = new int[maxActions+1];
		actionsList = getBaseActions(Random.Range (1, 4)) ;
		//actionsList[0] = Random.Range (2, 5);
		//actionsList[0] = 3 ;
		/*for (int i = 1; i <= maxActions; i++) {
			actionsList[i] = Random.Range(0,5) ;
			//actionsList[i] = 1 ;
		}*/
	}
	
	int[] getBaseActions(int idBase){
		int[] baseActions = new int[6] ;
		switch(idBase){
		case 1 :
			baseActions[0] = 10 ;
			baseActions[1] = 0 ;
			baseActions[2] = 1 ;
			baseActions[3] = 4 ;
			baseActions[4] = 1 ;
			baseActions[5] = 0 ;
		break ;
		
		case 2 :
			baseActions[0] = 10 ;
			baseActions[1] = 2 ;
			baseActions[2] = 0 ;
			baseActions[3] = 0 ;
			baseActions[4] = 4 ;
			baseActions[5] = 2 ;
		break ;
		
		case 3 :
			baseActions[0] = 10 ;
			baseActions[1] = 4 ;
			baseActions[2] = 2 ;
			baseActions[3] = 0 ;
			baseActions[4] = 4 ;
			baseActions[5] = 1 ;
		break;
		}
		return baseActions ;
	}
	
	void Update(){
		if(carried){
			jobToDo = false ;
			isGoingForward = false ;
			isTurning = false ;
		}
	}

	// Update is called once per frame
	void FixedUpdate() {
		

		if (carried)
		{
			rigidbody.Sleep();
			transform.position = player.position + transform.forward * 2f;
			transform.forward = Camera.main.transform.forward;
			wasCarried = true ;
		}
		else if(wasCarried == true && carried == false)
		{
			snapToFloor() ;
			rigidbody.WakeUp();
			wasCarried = false ;
			startJob () ;
		}else if(jobToDo){
			doYourJob() ;
		}
	}

	void startJob(){
		stepAction = 1 ;
		repeatAction = 0 ;
		jobToDo = true ;
	}

	void doYourJob(){
		switch(actionsList[stepAction]){
			case 0 :
				goForward() ;
			break;
			case 1 : case 2 :
				turn(actionsList [stepAction]) ;
				break;
			case 3 :
				//jump() ;
				//Debug.Log("jump") ;
				break;
			case 4 :
				createBlock() ;
				stepAction++ ;
				break;
		}
		if(stepAction > maxActions){
			stepAction = 1 ;
			repeatAction++ ;
			//Debug.Log("repeat") ;
			if(repeatAction >= actionsList[0]){
				//Debug.Log("endJob") ;
				jobToDo = false ;
			}
		}
	}
	
	bool canIGoForward(Vector3 direction, float distance){
		if(Physics.Raycast(rigidbody.transform.position, direction, distance)){
			return false ;
		}
		return true ;
	}
	
	void goForward(){
		if(!isGoingForward){
			target = transform.forward*moveDistance + rigidbody.position ;
			target.x = Mathf.Round(target.x) ;
			target.y = Mathf.Round(target.y) ;
			target.z = Mathf.Round(target.z) ;
			//Debug.Log("move "+target) ;
			isGoingForward = true ;
		}
		Vector3 dir = Vector3.Normalize(target-rigidbody.position) ;
		if(!canIGoForward(dir, moveDistance+1.0f)){
			isGoingForward = false ;
			stepAction++ ;
			//Debug.Log ("cant move, skip") ;
			return ;
		}
		Vector3 step = dir*moveDistance*speed ;
		// Move our position a step closer to the target.
		rigidbody.MovePosition(rigidbody.transform.position + (step*Time.deltaTime));
		if(Vector3.Distance(rigidbody.transform.position, target) <= .5f){
			isGoingForward = false ;
			stepAction++ ;
		}
	}

	void turn(int direction){
		if(!isTurning){
			if (direction == 1) {
				//turn left
				eulerAngleTarget = Quaternion.Euler(rigidbody.rotation.eulerAngles + new Vector3(0f,-90f,0f)).eulerAngles ;
				//Debug.Log("turn left "+eulerAngleTarget) ;
			}else{
				//turn right
				eulerAngleTarget = Quaternion.Euler(rigidbody.rotation.eulerAngles + new Vector3(0f,90f,0f)).eulerAngles ;
				//Debug.Log("turn right "+eulerAngleTarget) ;
			}

			isTurning = true ;
			
		}
		
			float step = speedRotation * Time.deltaTime *100f ;
			Quaternion deltaRotation = Quaternion.Euler(eulerAngleTarget * step);
			//rigidbody.MoveRotation(rigidbody.rotation * deltaRotation) ;
			rigidbody.MoveRotation(Quaternion.RotateTowards(rigidbody.rotation, Quaternion.Euler(eulerAngleTarget), step)) ;
		if(Vector3.Distance(rigidbody.rotation.eulerAngles, eulerAngleTarget) <= 0.2f){
			rigidbody.MoveRotation(Quaternion.Euler(eulerAngleTarget)) ;
			isTurning = false ;
			stepAction++ ;
		}
	}

	void jump(){
		
	}

	void createBlock(){
		//Debug.Log("create block") ;
		float distance = 2 ;
		if(canIGoForward(transform.forward, distance+0.5f)){
			GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
			cube.transform.position = transform.forward*distance + rigidbody.position ;
		}else{
			//peut pas poser
		}
	}
	
	void snapToFloor(){
		RaycastHit hit;
		if (Physics.Raycast(transform.position, -Vector3.up, out hit)){
			
			transform.position = hit.point+new Vector3(0.0f,distToFloor,0.0f) ;
			float newY = Mathf.Round(transform.rotation.eulerAngles.y / 45.0f) * 45.0f ;
			Debug.Log (transform.rotation.eulerAngles.y+" "+newY) ;
			transform.rotation = Quaternion.Euler(0.0f,newY,0.0f) ;
			//transform.position = transform.position ;
		}
	}
	
}