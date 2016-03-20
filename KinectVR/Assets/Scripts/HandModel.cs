using UnityEngine;
using System.Collections;

public class HandModel : MonoBehaviour {

	public Transform handModel;
	public Transform thumbPoint;
	public Transform thumbTracking;
	public Animation anim;
	// Use this for initialization
	void Start () {

		handModel = Instantiate (ObjectManager.s.kinectHand, transform.position, transform.rotation) as Transform;
		anim = handModel.GetComponent<Animation> ();
		handModel.parent = transform;

		transform.localScale = Vector3.Scale (transform.localScale, new Vector3 (1f, 1f, -1f));
		thumbPoint = handModel.FindChild ("thumbPoint");
		if (transform.name == "WristRight") {
			handModel.localScale = Vector3.Scale (handModel.localScale, new Vector3 (-1f, 1f, 1f));
			thumbTracking = transform.parent.FindChild ("ThumbLeft");
		} else {
			thumbTracking = transform.parent.FindChild ("ThumbRight");
		}



	
	}

	public void Open (){
		anim.clip = anim.GetClip ("open");
		anim.Play ();
		Debug.Log ("Open");
		//anim.CrossFade ("open",0.5f);
	}

	public void Close(){
		anim.clip = anim.GetClip ("closed");
		anim.Play ();
		Debug.Log ("Close");
		//anim.CrossFade ("closed",0.5f);
	}

	// Update is called once per frame
	void Update () {
		if (thumbPoint) {
			thumbPoint.LookAt (thumbTracking.position);
			Vector3 eul = handModel.localRotation.eulerAngles;
			eul.z = thumbPoint.localRotation.eulerAngles.x;
			Quaternion rot = handModel.localRotation;
			rot.eulerAngles = eul;
			handModel.localRotation = rot;
		}
	
	}
}
