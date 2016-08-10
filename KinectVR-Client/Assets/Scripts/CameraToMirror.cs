using UnityEngine;
using System.Collections;

public class CameraToMirror : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Camera camera = gameObject.GetComponent<Camera> ();
		camera.projectionMatrix = camera.projectionMatrix * Matrix4x4.Scale(new Vector3(-1, -1, 1));
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
