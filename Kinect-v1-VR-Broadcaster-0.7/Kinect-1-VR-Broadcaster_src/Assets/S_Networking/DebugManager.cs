using UnityEngine;
using System.Collections;

public class DebugManager : MonoBehaviour {
	private static DebugManager singleton;
	public static DebugManager s {get {return singleton;}}
	protected void Awake(){
		singleton = this;
	}
	public GUIText overlay;

	void Start () {
		overlay = GameObject.Find ("Debug").GetComponent<GUIText>();
		overlay.enabled = false;
	}
	

	void Update () {

		if (Input.GetKeyDown ("`")){
			if (overlay.enabled == true)
				overlay.enabled = false;
			else
				overlay.enabled = true;
		}
	
	}
}
