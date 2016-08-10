using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class MapToKinect : MonoBehaviour {

	public float offsetY;
	public ulong assignedid;

	public Transform SpineBase;
	public Transform SpineMid;
	public Transform Neck;
	public Transform Head;
	public Transform ShoulderLeft;
	public Transform ElbowLeft;
	public Transform WristLeft;
	public Transform HandLeft;
	public Transform ShoulderRight;
	public Transform ElbowRight;
	public Transform WristRight;
	public Transform HandRight;
	public Transform HipLeft;
	public Transform KneeLeft;
	public Transform AnkleLeft;
	public Transform FootLeft;
	public Transform HipRight;
	public Transform KneeRight;
	public Transform AnkleRight;
	public Transform FootRight;
	public Transform SpineShoulder;
	public Transform HandTipLeft;
	public Transform ThumbLeft;
	public Transform HandTipRight;
	public Transform ThumbRight;


	public Transform kSpineBase;
	private Transform kSpineMid;
	private Transform kNeck;
	private Transform kHead;
	private Transform kShoulderLeft;
	private Transform kElbowLeft;
	private Transform kWristLeft;
	private Transform kHandLeft;
	private Transform kShoulderRight;
	private Transform kElbowRight;
	private Transform kWristRight;
	private Transform kHandRight;
	private Transform kHipLeft;
	private Transform kKneeLeft;
	private Transform kAnkleLeft;
	private Transform kFootLeft;
	private Transform kHipRight;
	private Transform kKneeRight;
	private Transform kAnkleRight;
	private Transform kFootRight;
	private Transform kSpineShoulder;
	private Transform kHandTipLeft;
	private Transform kThumbLeft;
	private Transform kHandTipRight;
	private Transform kThumbRight;






	public string assignedname;

	public void AssignBody (string id){
		Debug.Log ("assigned body " + id);
		assignedname = id;
		//assignedid = id;
		string bodyprefix = "Online Body: ";

		kSpineBase = GameObject.Find (bodyprefix+id.ToString()+"/SpineBase").transform;
		kSpineMid = GameObject.Find (bodyprefix+id.ToString()+"/SpineMid").transform;
		kNeck = GameObject.Find (bodyprefix+id.ToString()+"/Neck").transform;
		kHead = GameObject.Find (bodyprefix+id.ToString()+"/Head").transform;
		kShoulderLeft = GameObject.Find (bodyprefix+id.ToString()+"/ShoulderLeft").transform;
		kElbowLeft = GameObject.Find (bodyprefix+id.ToString()+"/ElbowLeft").transform;
		kWristLeft = GameObject.Find (bodyprefix+id.ToString()+"/WristLeft").transform;
		kHandLeft = GameObject.Find (bodyprefix+id.ToString()+"/HandLeft").transform;
		kShoulderRight = GameObject.Find (bodyprefix+id.ToString()+"/ShoulderRight").transform;
		kElbowRight = GameObject.Find (bodyprefix+id.ToString()+"/ElbowRight").transform;
		kWristRight = GameObject.Find (bodyprefix+id.ToString()+"/WristRight").transform;
		kHandRight = GameObject.Find (bodyprefix+id.ToString()+"/HandRight").transform;
		kHipLeft = GameObject.Find (bodyprefix+id.ToString()+"/HipLeft").transform;
		kKneeLeft = GameObject.Find (bodyprefix+id.ToString()+"/KneeLeft").transform;
		kAnkleLeft = GameObject.Find (bodyprefix+id.ToString()+"/AnkleLeft").transform;
		kFootLeft = GameObject.Find (bodyprefix+id.ToString()+"/FootLeft").transform;
		kHipRight = GameObject.Find (bodyprefix+id.ToString()+"/HipRight").transform;
		kKneeRight = GameObject.Find (bodyprefix+id.ToString()+"/KneeRight").transform;
		kAnkleRight = GameObject.Find (bodyprefix+id.ToString()+"/AnkleRight").transform;
		kFootRight = GameObject.Find (bodyprefix+id.ToString()+"/FootRight").transform;
		kSpineShoulder = GameObject.Find (bodyprefix+id.ToString()+"/SpineShoulder").transform;
		kHandTipLeft = GameObject.Find (bodyprefix+id.ToString()+"/HandTipLeft").transform;
		kThumbLeft = GameObject.Find (bodyprefix+id.ToString()+"/ThumbLeft").transform;
		kHandTipRight = GameObject.Find (bodyprefix+id.ToString()+"/HandTipRight").transform;
		kThumbRight = GameObject.Find (bodyprefix+id.ToString()+"/ThumbRight").transform;


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


	public IDictionary<string, Quaternion> kinectRotations = new Dictionary<string, Quaternion>();



	Quaternion rot;

	Quaternion startingrot1;
	Quaternion startingrot2;
	Quaternion startingrot3;
	Quaternion offset1;
	Quaternion offset2;
	Quaternion offset3;
	Quaternion offset4;
	Quaternion offset5;
	Quaternion offset6;
	Quaternion offset7;
	Quaternion offset8;
	Quaternion offset9;
	Quaternion offset10;

	// Use this for initialization
	void Start () {
		/*

		startingrot1 = SpineMid.rotation;
		rot = KinectRiggingTools.s.kinectRotations ["SpineMid"];
		offset1 = startingrot1 * rot;

		startingrot2 = Neck.rotation;
		rot = KinectRiggingTools.s.kinectRotations ["Neck"];
		offset2 = startingrot2 * rot;

		startingrot3 = ShoulderLeft.rotation;
		rot = KinectRiggingTools.s.kinectRotations ["ShoulderLeft"];
		offset3 = startingrot3 * rot;


		offset4 = ShoulderRight.rotation*KinectRiggingTools.s.kinectRotations ["ShoulderRight"];


		offset5 = ElbowLeft.rotation * KinectRiggingTools.s.kinectRotations ["ElbowRight"];

		offset6 = ElbowRight.rotation * KinectRiggingTools.s.kinectRotations ["ElbowRight"];

		offset7 = KneeLeft.rotation * KinectRiggingTools.s.kinectRotations ["KneeLeft"];

		offset8 = KneeRight.rotation * KinectRiggingTools.s.kinectRotations ["KneeRight"];

		offset9 = HipLeft.rotation * KinectRiggingTools.s.kinectRotations ["HipLeft"];

		offset10 = HipRight.rotation * KinectRiggingTools.s.kinectRotations ["HipRight"];

		offset1.x = -offset1.x;
		offset1.y = offset1.y;
		offset1.z = -offset1.z;
		offset1.w = offset1.w;

		offset2.x = -offset2.x;
		offset2.y = offset2.y;
		offset2.z = -offset2.z;
		offset2.w = offset2.w;

		//shoulderleft
		offset3.x = offset3.x;
		offset3.y = offset3.y;
		offset3.z = offset3.z;
		offset3.w = -offset3.w;
		//shoulderright
		offset4.x = offset4.x;
		offset4.y = offset4.y;
		offset4.z = -offset4.z;
		offset4.w = offset4.w;

		//elbowleft
		offset5.x = -offset5.x;
		offset5.y = offset5.y;
		offset5.z = offset5.z;
		offset5.w = -offset5.w;

		//elbowright
		offset6.x = -offset6.x;
		offset6.y = offset6.y;
		offset6.z = offset6.z;
		offset6.w = -offset6.w;

		//kneeleft
		offset7.x = offset7.x;
		offset7.y = -offset7.y;
		offset7.z = offset7.z;
		offset7.w = offset7.w;

		//kneeright
		offset8.x = offset8.x;
		offset8.y = -offset8.y;
		offset8.z = offset8.z;
		offset8.w = offset8.w;

		//hipleft
		offset9.x = offset9.x;
		offset9.y = -offset9.y;
		offset9.z = offset9.z;
		offset9.w = -offset9.w;

		//hipright
		offset10.x = offset10.x;
		offset10.y = offset10.y;
		offset10.z = offset10.z;
		offset10.w = offset10.w;
*/
	}

	// Update is called once per frame
	void Update () {

			if (assignedname.Length > 1) {
			RefreshMapping ();
				}
	}



	public float interpolateSpeed = 10f;

	public void RefreshMapping () {


		if (assignedname != null) {
		
			Vector3 offsetpos = kSpineBase.position;
			offsetpos.y += offsetY;
			SpineBase.position = offsetpos;
			//Head.position = kHead.position;
			
			SpineMid.rotation = Quaternion.Slerp(SpineMid.rotation,ConvertOrientation(kSpineMid.rotation, Quaternion.Euler(0f,90f,90f), new Vector3 (1f, -1f, -1f)), Time.deltaTime*interpolateSpeed);
			Neck.rotation = Quaternion.Slerp(Neck.rotation, ConvertOrientation(kNeck.rotation, Quaternion.Euler(0f,90f,90f), new Vector3 (1f, -1f, -1f)), Time.deltaTime*interpolateSpeed);			

			KneeLeft.rotation = Quaternion.Slerp(KneeLeft.rotation, ConvertOrientation(kKneeLeft.rotation,Quaternion.Euler(0f,180f,90f), new Vector3 (1f, -1f, -1f)), Time.deltaTime*interpolateSpeed);
			KneeRight.rotation = Quaternion.Slerp(KneeRight.rotation, ConvertOrientation(kKneeRight.rotation,Quaternion.Euler(0f,0f,90f), new Vector3 (1f, -1f, -1f)), Time.deltaTime*interpolateSpeed);

			AnkleLeft.rotation = Quaternion.Slerp(AnkleLeft.rotation, ConvertOrientation(kAnkleLeft.rotation,Quaternion.Euler(0f,180f,90f), new Vector3 (1f, -1f, -1f)), Time.deltaTime*interpolateSpeed);
			AnkleRight.rotation = Quaternion.Slerp(AnkleRight.rotation, ConvertOrientation(kAnkleRight.rotation,Quaternion.Euler(0f,0f,90f), new Vector3 (1f, -1f, -1f)), Time.deltaTime*interpolateSpeed);

			ShoulderLeft.rotation = Quaternion.Slerp(ShoulderLeft.rotation, ConvertOrientation(kShoulderLeft.rotation,Quaternion.Euler(0f,0f,90f), new Vector3 (1f, -1f, -1f)), Time.deltaTime*interpolateSpeed);
			ShoulderRight.rotation = Quaternion.Slerp(ShoulderRight.rotation, ConvertOrientation(kShoulderRight.rotation,Quaternion.Euler(0f,180f,90f), new Vector3 (1f, -1f, -1f)), Time.deltaTime*interpolateSpeed);

			ElbowLeft.rotation = Quaternion.Slerp(ElbowLeft.rotation, ConvertOrientation(kElbowLeft.rotation,Quaternion.Euler(0,0,90f), new Vector3 (1f, -1f, -1f)), Time.deltaTime*interpolateSpeed);
			ElbowRight.rotation = Quaternion.Slerp(ElbowRight.rotation, ConvertOrientation(kElbowRight.rotation,Quaternion.Euler(0,180f,90f), new Vector3 (1f, -1f, -1f)), Time.deltaTime*interpolateSpeed);

			WristLeft.rotation = Quaternion.Slerp(WristLeft.rotation, ConvertOrientation(kWristLeft.rotation,Quaternion.Euler(0,180f,90f), new Vector3 (1f, -1f, -1f)), Time.deltaTime*interpolateSpeed);
			WristRight.rotation = Quaternion.Slerp(WristRight.rotation, ConvertOrientation(kWristRight.rotation,Quaternion.Euler(0,0f,90f), new Vector3 (1f, -1f, -1f)), Time.deltaTime*interpolateSpeed);



			/*
			SpineMid.rotation = ConvertOrientation (kSpineMid.rotation, Quaternion.Euler (180f, 180f, 270f), mirroroffset);
			
			KneeLeft.rotation = ConvertOrientation (kKneeLeft.rotation, Quaternion.Euler (1f, 270f, 90f), mirroroffset);
			KneeRight.rotation = ConvertOrientation (kKneeRight.rotation, Quaternion.Euler (1f, 90f, 90f), mirroroffset);
			
			AnkleLeft.rotation = ConvertOrientation (kAnkleLeft.rotation, Quaternion.Euler (1f, 1f, 90f), mirroroffset);
			AnkleRight.rotation = ConvertOrientation (kAnkleRight.rotation, Quaternion.Euler (1f, 1f, 90f), mirroroffset);
			
			
			ShoulderLeft.rotation = ConvertOrientation (kShoulderLeft.rotation, Quaternion.Euler (125f, 45f, 225f), mirroroffset);
			ShoulderRight.rotation = ConvertOrientation (kShoulderRight.rotation, Quaternion.Euler (45f, 90f, 90f), mirroroffset);
			
			
			ElbowLeft.rotation = ConvertOrientation (kElbowLeft.rotation, Quaternion.Euler (1f, 90f, 90f), mirroroffset);
			ElbowRight.rotation = ConvertOrientation (kElbowRight.rotation, Quaternion.Euler (1f, 90f, 270f), mirroroffset);
			
			WristLeft.rotation = ConvertOrientation (kWristLeft.rotation, Quaternion.Euler (90f, 270f, 135f), mirroroffset);
			WristRight.rotation = ConvertOrientation (kWristRight.rotation, Quaternion.Euler (1f, 90f, 270f), mirroroffset);

			Neck.rotation = ConvertOrientation (kNeck.rotation, Quaternion.Euler (0f, 90f, 90f), mirroroffset);
			*/


		}
	}
}