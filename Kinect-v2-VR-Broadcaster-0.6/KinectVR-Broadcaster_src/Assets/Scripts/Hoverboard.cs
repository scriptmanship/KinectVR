using UnityEngine;
using System.Collections;

public class Hoverboard : MonoBehaviour {

	public OnlineBodyPhysics body;
	public bool on;
	public bool animating;
	private Vector3 startScale;


	// Use this for initialization
	void Start () {
		startScale = transform.localScale;
		if (!on)
			transform.localScale = new Vector3 (0f, 0f, 0f);
	}
	
	// Update is called once per frame
	void Update () {
		if (animating)
		Animate ();

		transform.position = body.heightSphere.transform.position;
	}

	public void Play(){
		if (!animating) {
			animating = true;
			on = true;
		}


	}
	public void Stop(){
		if (!animating) {
			animating = true;
			on = false;
		}
	}

	void Animate(){
		Vector3 newScale = transform.localScale;
		if (!on) {

			if (newScale.x < 1f){
				newScale += new Vector3(1f,1f,1f)*Time.deltaTime;
			}else{
				newScale = new Vector3(1f,1f,1f);
				on = true;
				animating = false;
			}

		}

		if (on) {

			if (newScale.x > 0f){
				newScale -= new Vector3(1f,1f,1f)*Time.deltaTime;
			}else{
				newScale = new Vector3(0f,0f,0f);
				on = false;
				animating = false;
			}

			
		}
		transform.localScale = newScale;
	}
	
}
