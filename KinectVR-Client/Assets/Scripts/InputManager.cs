using UnityEngine;
using System.Collections;

public class InputManager : MonoBehaviour {
	public bool isGear = true;

	private float mouseXstart;
	private float mouseXend;
	private float currentTime;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		currentTime += Time.deltaTime;

		if (TouchMouseDown ()) {
			currentTime = 0;
			mouseXstart = Input.mousePosition.x;


		}

		if (TouchMouseUp ()) {
			mouseXend = Input.mousePosition.x;

			float mousediff = mouseXend - mouseXstart;
			if (mousediff > 0 && currentTime < 0.5f) {
				PlayerManager.s.NextBody ();
			}

		}
	}

	bool TouchMouseDown (){


		bool result = false;
		if (isGear) {
			if (Input.GetMouseButtonDown(0)){
				result = true;
			}else{
				result = false;
			}

		} else {

			foreach (Touch t in Input.touches) {
				if (t.phase == TouchPhase.Began){
					result = true;

				}

			}


		}
		return result;

	}

	bool TouchMouseUp (){


		bool result = false;
		if (isGear) {
			if (Input.GetMouseButtonUp(0)){
				result = true;
			}else{
				result = false;
			}

		} else {

			foreach (Touch t in Input.touches) {
				if (t.phase == TouchPhase.Ended){
					result = true;

				}

			}


		}
		return result;

	}



}
