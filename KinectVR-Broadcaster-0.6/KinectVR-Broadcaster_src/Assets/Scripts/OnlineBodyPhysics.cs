using UnityEngine;
using System.Collections;

public class OnlineBodyPhysics : MonoBehaviour {
	Transform ankleRight;
	Transform ankleLeft;
	Transform kneeRight;
	Transform kneeLeft;
	private float jumpRayLength = 1.5f;
	private float rayLength = 1.5f;
	private float height = 3f;
	public bool groundedLeft = false;
	public bool groundedRight = false;

	public Hoverboard hoverboard;

	public OnlineBodyHeight heightSphere;


	private float walkSpeed = 6f;

	private float gravityDefaultSpeed = 10f;
	private float maxFallSpeed = 100f;
	private float gravity = 10f;
	private float gravityAcceleration = 9f;

	private float ankleLeftY = 0f;
	private float ankleRightY = 0f;


	// Use this for initialization
	void Start () {
		foreach (Transform t in transform) {
			if (t.name == "AnkleRight"){
				ankleLeft = t;
			}
			if (t.name == "AnkleLeft"){
				ankleRight = t;
			}
			if (t.name == "KneeRight"){
				kneeLeft = t;
			}
			if (t.name == "KneeLeft"){
				kneeRight = t;
			}



			if (ankleLeft && ankleRight && kneeLeft && kneeRight)
				break;



			//Transform hov = (Transform) Instantiate (ObjectManager.s.hoverboard, new Vector3 (0, 0, 0), Quaternion.identity) as Transform;
			//hov.transform.parent = transform;
			//hoverboard = hov.GetComponent<Hoverboard>();
			//hoverboard.body = this;
		}


	}




	private bool inAir;
	// Update is called once per frame
	void Update () {

		Debug.Log (CheckJump ());


		if (CheckJump () && !inAir) {
			inAir = true;

		}
		//Hoverboard ();
		//Walk ();
		ApplyGravity ();
	
	}




	private bool grounding;


	float rightLegTime;
	float leftLegTime;
	float idleLegTime;
	public bool stride = false;

	void HoverBoard(){
		if (CheckJump () && !inAir && hoverboard != null) {
			if (!hoverboard.on){
				hoverboard.Play();
			}else{
				hoverboard.Stop();
			}
			inAir = true;
		}
		
		if (groundedLeft && groundedRight) {
			inAir = false;
		}
		
		if (hoverboard != null)
			hoverboard.transform.position = transform.position;

		if (groundedLeft && groundedRight) {

		}

	}

	void ApplyGravity(){
		
		
		Vector3 newpos = transform.position;
		newpos.y = heightSphere.transform.position.y+height;
		transform.position = newpos;

	}

	bool CheckJump(){
		bool jumped = false;
		RaycastHit hit;
		
		Ray rayAnkleLeft = new Ray (ankleLeft.position - transform.up * ankleLeft.localScale.x / 2, -transform.up);
		if (Physics.Raycast (rayAnkleLeft, out hit, jumpRayLength)) {
			groundedLeft = true;
		
			
		} else {
			groundedLeft = false;
		}
		RaycastHit hitt;
		
		Ray rayAnkleRight = new Ray (ankleRight.position - transform.up * ankleRight.localScale.x / 2, -transform.up);
		if (Physics.Raycast (rayAnkleRight, out hitt, jumpRayLength)) {
			groundedRight = true;

			
		} else {
			groundedRight = false;
		}

		if (!groundedLeft && !groundedRight) {
			jumped = true;
		} else {
			jumped = false;
		}
		return jumped;
	}





	void Walk(){
		CheckWalking ();
		//Debug.Log (stride);
		//Debug.Log (groundedLeft + " | " + groundedRight);
		
		RaycastHit hit;
		
		Ray rayAnkleLeft = new Ray (ankleLeft.position - transform.up * ankleLeft.localScale.x / 2, -transform.up);
		if (Physics.Raycast (rayAnkleLeft, out hit, rayLength)) {
			groundedLeft = true;
			ankleLeftY = hit.point.y;
			
		} else {
			groundedLeft = false;
		}
		RaycastHit hitt;
		
		Ray rayAnkleRight = new Ray (ankleRight.position - transform.up * ankleRight.localScale.x / 2, -transform.up);
		if (Physics.Raycast (rayAnkleRight, out hitt, rayLength)) {
			groundedRight = true;
			ankleRightY = hitt.point.y;
			
		} else {
			groundedRight = false;
		}
		
		
		//Digital Movement

		Vector3 movePos = transform.position;
		if (stride)
			movePos += Camera.main.transform.forward*walkSpeed*Time.deltaTime;

		if (!groundedLeft && !groundedRight) {
			grounding = false;
			Vector3 newpos = transform.position;
			newpos.y -= gravity * Time.deltaTime;
			if (gravity > maxFallSpeed){
				gravity = maxFallSpeed;
			}else{
				gravity += gravityAcceleration*Time.deltaTime;
			}
			transform.position = newpos;
		} else if (groundedLeft && groundedRight) {
			transform.position = new Vector3(movePos.x,transform.position.y,movePos.z);
			if (grounding == false){
			gravity = gravityDefaultSpeed;
				transform.position = new Vector3 (movePos.x, (ankleLeftY + ankleRightY) / 2f + height, movePos.z);
				grounding = true;
			}

		} else if (groundedLeft) {
			transform.position = new Vector3(movePos.x,transform.position.y,movePos.z);
			if (grounding == false){
			gravity = gravityDefaultSpeed;
				transform.position = new Vector3 (movePos.x, ankleLeftY  + height, movePos.z);
				grounding = true;
			}

		} else if (groundedRight) {
			transform.position = new Vector3(movePos.x,transform.position.y,movePos.z);
			if (grounding == false){
			gravity = gravityDefaultSpeed;
				transform.position = new Vector3 (movePos.x, ankleRightY + height, movePos.z);
				grounding = true;
			}

		}


	}

	void CheckWalking(){
		float LeftLegY = ankleLeft.position.y;
		float RightLegY = ankleRight.position.y;

		
		float diffmax = 0.15f;
		float diffmin = -0.15f;
		float legtimemax = 0.75f;
		float idlelegtimemax = 0.45f;
		
		float diff = LeftLegY - RightLegY;
		if (diff > diffmax) {
			if (rightLegTime > 0.0f && rightLegTime < legtimemax) {
				stride = true;
			}
			rightLegTime = 0f;
			idleLegTime = 0f;
			leftLegTime += Time.deltaTime;
			if (leftLegTime > legtimemax)
				stride = false;
			//Debug.Log ("Left Leg Up");
		} else if (diff < -diffmin) {
			if (leftLegTime > 0.0f && leftLegTime < legtimemax) {
				stride = true;
			}
			leftLegTime = 0f;
			idleLegTime = 0f;
			rightLegTime += Time.deltaTime;
			if (rightLegTime > legtimemax)
				stride = false;
			//Debug.Log ("Right Leg Up");
		} else if (diff >= -diffmin && diff <= diffmax) {
			
			idleLegTime += Time.deltaTime;
			if (idleLegTime > idlelegtimemax) {
				stride = false;
				leftLegTime = 0f;
				rightLegTime = 0f;
			}
		}
		

	}

}
