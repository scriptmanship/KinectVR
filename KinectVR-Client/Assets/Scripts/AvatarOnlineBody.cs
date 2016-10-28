using UnityEngine;
using System.Collections;

public class AvatarOnlineBody : MonoBehaviour {

	//Mixamo rig naming convention, YbotWithHands character leave blank, otherwise default should be namePrefix = "mixamorig:";
	private string namePrefix = "";


	public Transform avatar;
	public OnlineBody body;
	public Vector3 offset;

	public Transform root;

	public Transform spine;
	public Transform spineMid;
	public Transform head;
	public Transform shoulderLeft;
	public Transform shoulderRight;
	public Transform elbowLeft;
	public Transform elbowRight;
	public Transform handLeft;
	public Transform handRight;
	public Transform hipLeft;
	public Transform hipRight;
	public Transform kneeLeft;
	public Transform kneeRight;

	public HandCloseScript leftHand;
	public HandCloseScript rightHand;

	private float scale = 2.718f;
	// Use this for initialization
	void Start () {
		avatar = (Transform)Instantiate (ObjectManager.s.avatarPrefab, transform.position, transform.rotation) as Transform;
		Renderer r = avatar.GetChild(1).GetComponent<Renderer> ();
		r.material.color = new Color (Random.Range (0f, 1f), Random.Range (0f, 1f), Random.Range (0f, 1f));
		avatar.localScale = new Vector3 (scale, scale, scale);
		avatar.Rotate (new Vector3 (0, 180f, 0));

		foreach (Transform t in avatar) {



			if (t.name == namePrefix + "Hips") {
				root = t;
				foreach (Transform t2 in t) {
					if (t2.name == namePrefix + "LeftUpLeg") {
						hipLeft = t2;
						kneeLeft = hipLeft.GetChild (0);
					}

					if (t2.name == namePrefix + "RightUpLeg") {
						hipRight = t2;
						kneeRight = hipRight.GetChild (0);
					}


					if (t2.name == namePrefix + "Spine") {
						
						spine = t2;
						spineMid = t2.GetChild (0);
						foreach (Transform t3a in spineMid.GetChild(0)) {
							if (t3a.name == namePrefix + "LeftShoulder") {
								shoulderLeft = t3a.GetChild(0);
								elbowLeft = shoulderLeft.GetChild (0);
								handLeft = elbowLeft.GetChild (0);
								leftHand = handLeft.gameObject.GetComponent<HandCloseScript> ();
							}
							if (t3a.name == namePrefix + "RightShoulder") {

								shoulderRight = t3a.GetChild(0);
								elbowRight = shoulderRight.GetChild (0);
								handRight = elbowRight.GetChild (0);
								rightHand = handRight.gameObject.GetComponent<HandCloseScript> ();
							}
						}

						foreach (Transform t3b in spineMid.GetChild(0)) {
							if (t3b.name == namePrefix + "Neck") {
								head = t3b.GetChild (0);
								break;
							}
						}

						break;
					}
				}
				break;
			}
		}

	}

	void newBody(){
		Destroy (avatar.gameObject);
		avatar = (Transform)Instantiate (ObjectManager.s.avatarPrefab, transform.position, transform.rotation) as Transform;
		Renderer r = avatar.GetChild(1).GetComponent<Renderer> ();
		r.material.color = new Color (Random.Range (0f, 1f), Random.Range (0f, 1f), Random.Range (0f, 1f));
		avatar.localScale = new Vector3 (scale, scale, scale);
		avatar.Rotate (new Vector3 (0, 180f, 0));

		foreach (Transform t in avatar) {



			if (t.name == namePrefix + "Hips") {
				root = t;
				foreach (Transform t2 in t) {
					if (t2.name == namePrefix + "LeftUpLeg") {
						hipLeft = t2;
						kneeLeft = hipLeft.GetChild (0);
					}

					if (t2.name == namePrefix + "RightUpLeg") {
						hipRight = t2;
						kneeRight = hipRight.GetChild (0);
					}


					if (t2.name == namePrefix + "Spine") {

						spine = t2;
						spineMid = t2.GetChild (0);
						foreach (Transform t3a in spineMid.GetChild(0)) {
							if (t3a.name == namePrefix + "LeftShoulder") {
								shoulderLeft = t3a.GetChild(0);
								elbowLeft = shoulderLeft.GetChild (0);
								handLeft = elbowLeft.GetChild (0);
								leftHand = handLeft.gameObject.GetComponent<HandCloseScript> ();
							}
							if (t3a.name == namePrefix + "RightShoulder") {

								shoulderRight = t3a.GetChild(0);
								elbowRight = shoulderRight.GetChild (0);
								handRight = elbowRight.GetChild (0);
								rightHand = handRight.gameObject.GetComponent<HandCloseScript> ();
							}
						}

						foreach (Transform t3b in spineMid.GetChild(0)) {
							if (t3b.name == namePrefix + "Neck") {
								head = t3b.GetChild (0);
								break;
							}
						}

						break;
					}
				}
				break;
			}
		}

	}
	
	// Update is called once per frame
	void Update () {
		if (body.partsDic ["SpineBase"] != null && avatar != null) {
			avatar.position = body.partsDic ["SpineBase"].go.transform.position;
			avatar.position += new Vector3 (0, -3f, 0);
			if (spine != null) {



				Vector3 relativePos = body.partsDic ["SpineMid"].go.transform.position - body.partsDic ["SpineBase"].go.transform.position;
				Quaternion rotation = Quaternion.LookRotation(relativePos.normalized, Vector3.forward);

				spine.rotation = rotation * Quaternion.Euler (90f, 0f, 0f);

				relativePos = body.partsDic ["SpineShoulder"].go.transform.position - body.partsDic ["SpineMid"].go.transform.position;
				rotation = Quaternion.LookRotation(relativePos.normalized, Vector3.forward);
				spineMid.rotation = rotation * Quaternion.Euler (90f, 0f, 0f);
				//Disable for Kinect v1
				/*
				relativePos = body.partsDic ["Head"].go.transform.position - body.partsDic ["Neck"].go.transform.position;
				rotation = Quaternion.LookRotation(relativePos.normalized, Vector3.forward);
				head.rotation = rotation * Quaternion.Euler (90f, 0f, 0f);
				*/

				relativePos = body.partsDic ["ShoulderLeft"].go.transform.position - body.partsDic ["ElbowLeft"].go.transform.position;
				rotation = Quaternion.LookRotation(relativePos.normalized, Vector3.forward);
				shoulderLeft.rotation = rotation * Quaternion.Euler (270f, 0f, 0f);

				relativePos = body.partsDic ["ShoulderRight"].go.transform.position - body.partsDic ["ElbowRight"].go.transform.position;
				rotation = Quaternion.LookRotation(relativePos.normalized, Vector3.forward);
				shoulderRight.rotation = rotation * Quaternion.Euler (270f, 0f, 0f);

				relativePos = body.partsDic ["ElbowLeft"].go.transform.position - body.partsDic ["WristLeft"].go.transform.position;
				rotation = Quaternion.LookRotation(relativePos.normalized, Vector3.forward);
				elbowLeft.rotation = rotation * Quaternion.Euler (270f, 0f, 0f);


				relativePos = body.partsDic ["ElbowRight"].go.transform.position - body.partsDic ["WristRight"].go.transform.position;
				rotation = Quaternion.LookRotation(relativePos.normalized, Vector3.forward);
				elbowRight.rotation = rotation * Quaternion.Euler (270f, 0f, 0f);
				//Disable for Kinect v1
				/*
				relativePos = body.partsDic ["WristLeft"].go.transform.position - body.partsDic ["HandTipLeft"].go.transform.position;
				rotation = Quaternion.LookRotation(relativePos.normalized, Vector3.forward);
				handLeft.rotation = rotation * Quaternion.Euler (270f, 0f, 0f);

				relativePos = body.partsDic ["WristRight"].go.transform.position - body.partsDic ["HandTipRight"].go.transform.position;
				rotation = Quaternion.LookRotation(relativePos.normalized, Vector3.forward);
				handRight.rotation = rotation * Quaternion.Euler (270f, 0f, 0f);
				*/


				relativePos = body.partsDic ["HipLeft"].go.transform.position - body.partsDic ["KneeLeft"].go.transform.position;
				rotation = Quaternion.LookRotation(relativePos.normalized, -Vector3.forward);
				hipLeft.rotation = rotation * Quaternion.Euler (270f, 0f, 0f);

				relativePos = body.partsDic ["HipRight"].go.transform.position - body.partsDic ["KneeRight"].go.transform.position;
				rotation = Quaternion.LookRotation(relativePos.normalized, -Vector3.forward);
				hipRight.rotation = rotation * Quaternion.Euler (270f, 0f, 0f);


				relativePos = body.partsDic ["KneeLeft"].go.transform.position - body.partsDic ["AnkleLeft"].go.transform.position;
				rotation = Quaternion.LookRotation(relativePos.normalized, -Vector3.forward);
				kneeLeft.rotation = rotation * Quaternion.Euler (270f, 0f, 0f);

				relativePos = body.partsDic ["KneeRight"].go.transform.position - body.partsDic ["AnkleRight"].go.transform.position;
				rotation = Quaternion.LookRotation(relativePos.normalized, -Vector3.forward);
				kneeRight.rotation = rotation * Quaternion.Euler (270f, 0f, 0f);


				//Disable for Kinect v1
				/*
				if (rightHand != null && leftHand != null) {
					if (body.partsDic ["HandRight"].infered == 0 && body.partsDic ["HandTipRight"].infered == 0) {
						if (Vector3.Distance (body.partsDic ["HandTipRight"].go.transform.position, body.partsDic ["HandRight"].go.transform.position) < 0.19f) {
							rightHand.fingersBent = true;
							rightHand.thumbBent = true;
						} else {
							rightHand.fingersBent = false;
							rightHand.thumbBent = false;
						}
					}

					if (body.partsDic ["HandLeft"].infered == 0 && body.partsDic ["HandTipLeft"].infered == 0) {
						if (Vector3.Distance (body.partsDic ["HandTipLeft"].go.transform.position, body.partsDic ["HandLeft"].go.transform.position) < 0.19f) {
							leftHand.fingersBent = true;
							leftHand.thumbBent = true;
						} else {
							leftHand.fingersBent = false;
							leftHand.thumbBent = false;
						}
					}
				}

				*/



				if (relativePos.z < -0.15f) {
					//spine.rotation = rotation;
					//spine.rotation = rotation * Quaternion.Euler (90f, 0f, 0f);
				} else if (relativePos.z >= -0.15f && relativePos.z <= 0.15f) {
					
					//spine.rotation = ConvertOrientation (rotation, Quaternion.Euler (90f, 0f, 0f), new Vector3 (1f, -1f, 1f));
				}//spine.rotation = ConvertOrientation (rotation, Quaternion.Euler (270f, 0f, 0f), new Vector3 (1f, 1f, 1f));
				//spine.rotation = ConvertOrientation(body.partsDic ["SpineBase"].go.transform.rotation, Quaternion.Euler(0f,180f,0f), new Vector3 (1f, 1f, -1f));
			}
		}
	}

	Quaternion ConvertOrientation(Quaternion rot, Quaternion mult, Vector3 mirror){
		rot *= mult;
		Vector3 ang = rot.eulerAngles;
		ang.x *= mirror.x;
		ang.y *= mirror.y;
		ang.z *= mirror.z;
		rot.eulerAngles = ang;
		return rot;

	}

}
