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
	public OnlineBody onlineBody;

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



		}


	}




	private bool inAir;
	// Update is called once per frame
	void Update () {

		if (onlineBody.partsDic ["AnkleLeft"].infered == 0 && onlineBody.partsDic ["AnkleRight"].infered == 0 && onlineBody.partsDic ["KneeLeft"].infered == 0 && onlineBody.partsDic ["KneeRight"].infered == 0 ) {
			//Walk ();
		}
		ApplyGravity ();
	
	}




	private bool grounding;


	float rightLegTime;
	float leftLegTime;
	float idleLegTime;
	public bool stride = false;



	void ApplyGravity(){
		
		
		Vector3 newpos = transform.position;
		newpos.y = heightSphere.transform.position.y+height;
		transform.position = newpos;

	}

	

	void Walk(){
		CheckWalking ();

		
		
		//Digital Movement

		Vector3 movePos = transform.position;
		if (stride) {
			movePos += Camera.main.transform.forward * walkSpeed * Time.deltaTime;
		}
		transform.position = new Vector3 (movePos.x, transform.position.y, movePos.z);


	}

	void CheckWalking(){
		float LeftLegY = ankleLeft.position.y;
		float RightLegY = ankleRight.position.y;

		
		float diffmax = 0.45f;
		float diffmin = -0.45f;
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
