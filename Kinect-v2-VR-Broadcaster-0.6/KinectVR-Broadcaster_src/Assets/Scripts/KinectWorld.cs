using UnityEngine;
using System.Collections;

public class KinectWorld : MonoBehaviour {

	private static KinectWorld singleton;
	public static KinectWorld s {get {return singleton;}}
	protected void Awake(){
		singleton = this;
	}	


	public Vector3 startPos;
	public Transform gearCam;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
