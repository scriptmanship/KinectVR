using UnityEngine;
using System.Collections;

public class test : MonoBehaviour {

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		myVoid ();
	}

	void myVoid (){
		if (Input.GetButton ("Jump")){
		Vector3 tempLocation = transform.position;
		tempLocation.y += 1f*Time.deltaTime;
		transform.position = tempLocation;
		}
	}
}
