using UnityEngine;
using System.Collections;

public class CameraSystem : MonoBehaviour {

    public Camera playerCamera;
    public Camera viewCamera1; //top
    public Camera viewCamera2; //base
    public Camera viewCamera3; //top lookat player
    public Camera viewCamera4; //top turn around

    public Transform goodMigeon;

    private bool cam4pause = false;

	// Use this for initialization
	void Start () {
        viewCamera1.enabled = false;
        viewCamera2.enabled = false;
        viewCamera3.enabled = false;
        viewCamera4.enabled = false;
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown("0")){
            viewCamera1.enabled = false;
            viewCamera2.enabled = false;
            viewCamera3.enabled = false;
            viewCamera4.enabled = false;
            playerCamera.enabled = true;
        }
        if (Input.GetKeyDown("1"))
        {
            viewCamera1.enabled = true;
            viewCamera2.enabled = false;
            viewCamera3.enabled = false;
            viewCamera4.enabled = false;
            playerCamera.enabled = false;
        }
        if (Input.GetKeyDown("2"))
        {
            viewCamera1.enabled = false;
            viewCamera2.enabled = true;
            viewCamera3.enabled = false;
            viewCamera4.enabled = false;
            playerCamera.enabled = false;
        }
        if (Input.GetKeyDown("3"))
        {
            viewCamera1.enabled = false;
            viewCamera2.enabled = false;
            viewCamera3.enabled = true;
            viewCamera4.enabled = false;
            playerCamera.enabled = false;
        }
        if (Input.GetKeyDown("4"))
        {
            viewCamera1.enabled = false;
            viewCamera2.enabled = false;
            viewCamera3.enabled = false;
            viewCamera4.enabled = true;
            playerCamera.enabled = false;
        }

        if (Input.GetKeyDown("5"))
        {
            cam4pause = !cam4pause;
        }

            viewCamera2.transform.LookAt(goodMigeon);
        viewCamera3.transform.LookAt(playerCamera.transform);
        if (!cam4pause) { 
            Vector3 euler = viewCamera4.transform.eulerAngles;
            euler.y = (euler.y + 0.3f) % 360;
            viewCamera4.transform.eulerAngles = euler;
        }
        //viewCamera4.transform.Rotate(euler);
    }
}
