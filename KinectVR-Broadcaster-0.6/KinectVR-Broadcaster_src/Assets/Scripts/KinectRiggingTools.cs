using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class KinectRiggingTools : MonoBehaviour {

	private static KinectRiggingTools singleton;
	public static KinectRiggingTools s {get {return singleton;}}
	protected void Awake(){
		singleton = this;
	}	

	public IDictionary<string, Quaternion> kinectRotations;
	public void Start(){
		//AssignKinectObjects (72057594037928347);

		/*
		string data;
		data = GetKinectQuaternions (72057594037928347);
		System.IO.File.WriteAllText("C:/Users/Filip/Desktop/KinectQuaternions.txt", data);
		*/

		kinectRotations = new Dictionary<string, Quaternion>();
		kinectRotations["SpineBase"] = new Quaternion(-0.0002299043f,0.9985657f,0.02588109f,0.04686938f);
		kinectRotations["SpineMid"] = new Quaternion(-0.0002568785f,0.998614f,0.02588083f,0.04582861f);
		kinectRotations["Neck"] = new Quaternion(-0.0001554212f,0.9989583f,-0.008790282f,0.0447775f);
		kinectRotations["Head"] = new Quaternion(0f,0f,0f,1f);
		kinectRotations["ShoulderLeft"] = new Quaternion(0.7756962f,-0.6293045f,0.02866264f,-0.03807498f);
		kinectRotations["ElbowLeft"] = new Quaternion(-0.5538946f,0.2882095f,0.6947065f,0.3570979f);
		kinectRotations["WristLeft"] = new Quaternion(0.7852241f,-0.5179361f,0.3389075f,-0.01752771f);
		kinectRotations["HandLeft"] = new Quaternion(0.8011165f,-0.509155f,0.3129924f,0.03176848f);
		kinectRotations["ShoulderRight"] = new Quaternion(0.7849638f,0.6178735f,0.0411999f,0.01915159f);
		kinectRotations["ElbowRight"] = new Quaternion(0.6009046f,0.3362449f,0.6545691f,-0.3120776f);
		kinectRotations["WristRight"] = new Quaternion(0.2159232f,-0.1043377f,0.8437569f,-0.4801719f);
		kinectRotations["HandRight"] = new Quaternion(0.2455297f,-0.1315757f,0.86044f,-0.4266687f);
		kinectRotations["HipLeft"] = new Quaternion(0.6970571f,-0.6687651f,0.1690861f,-0.1956389f);
		kinectRotations["KneeLeft"] = new Quaternion(0.7388211f,-0.07904721f,-0.6687124f,-0.02680912f);
		kinectRotations["AnkleLeft"] = new Quaternion(0.7275218f,-0.009648697f,-0.6786849f,0.1000288f);
		kinectRotations["FootLeft"] = new Quaternion(0f,0f,0f,1f);
		kinectRotations["HipRight"] = new Quaternion(0.6951588f,0.6965352f,-0.107333f,-0.1416779f);
		kinectRotations["KneeRight"] = new Quaternion(0.6729166f,0.05451683f,0.7373968f,-0.02137875f);
		kinectRotations["AnkleRight"] = new Quaternion(0.6892793f,0.03395439f,0.7169288f,0.09876455f);
		kinectRotations["FootRight"] = new Quaternion(0f,0f,0f,1f);
		kinectRotations["SpineShoulder"] = new Quaternion(-0.0001837048f,0.9989376f,0.01083513f,0.04479027f);
		kinectRotations["HandTipLeft"] = new Quaternion(0f,0f,0f,1f);
		kinectRotations["ThumbLeft"] = new Quaternion(0f,0f,0f,1f);
		kinectRotations["HandTipRight"] = new Quaternion(0f,0f,0f,1f);
		kinectRotations["ThumbRight"] = new Quaternion(0f,0f,0f,1f);

	}



	public string GetKinectQuaternions (ulong body) {
		GameObject kinectbody = GameObject.Find ("Body_" + body.ToString ());
		List<GameObject>  bodyparts_list = kinectbody.GetChildren ();
		GameObject[] bodyparts = bodyparts_list.ToArray ();
		string data = "";
		foreach (GameObject g in bodyparts) {
			//Debug.Log(g.name + "," + g.transform.rotation.x + "," + g.transform.rotation.y + "," + g.transform.rotation.z + "," + g.transform.rotation.w + ",");
			data += "kinectRotations['"+g.name+"'] = new Quaternion("+g.transform.rotation.x + "f," + g.transform.rotation.y + "f," + g.transform.rotation.z + "f," + g.transform.rotation.w + "f);";
		}
		return data;
	}

	public IDictionary<string, GameObject> kinectParts;
	public void AssignKinectObjects (ulong body) {
		kinectParts = new Dictionary<string, GameObject>();
		GameObject kinectbody = GameObject.Find ("Body_" + body.ToString ());
		List<GameObject>  bodyparts_list = kinectbody.GetChildren ();
		GameObject[] bodyparts = bodyparts_list.ToArray ();
		foreach (GameObject g in bodyparts) {
			kinectParts[g.name] = g;
			}

	}
}