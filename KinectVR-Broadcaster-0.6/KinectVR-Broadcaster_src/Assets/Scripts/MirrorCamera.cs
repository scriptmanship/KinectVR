using UnityEngine;
using System.Collections;

public class MirrorCamera : MonoBehaviour {
	public Camera cam;
	// Use this for initialization
	void OnPreCull () {
		cam.ResetWorldToCameraMatrix ();
		cam.ResetProjectionMatrix ();
		Matrix4x4 mat = cam.projectionMatrix;
		mat *= Matrix4x4.Scale(new Vector3(-1f, 1f, 1f));
		cam.projectionMatrix = mat;	
	}
	
	void OnPreRender () {
		GL.SetRevertBackfacing (true);
	}
	
	void OnPostRender () {
		GL.SetRevertBackfacing (false);
	}



	void Start () {
		//Screen.SetResolution (4320, 1080, false);
	}
	
	// Update is called once per frame
	void Update () {

		//Debug.Log (Screen.width + "," + Screen.height);
	
	}
}
