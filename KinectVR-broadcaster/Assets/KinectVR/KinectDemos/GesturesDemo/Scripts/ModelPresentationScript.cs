using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ModelPresentationScript : MonoBehaviour 
{
	[Tooltip("Speed of spinning, when presentation slides change.")]
	public float spinSpeed = 10;

	// reference to the gesture listener
	private ModelGestureListener gestureListener;
	

	void Start() 
	{
		// hide mouse cursor
		Cursor.visible = false;
		
		// get the gestures listener
		gestureListener = ModelGestureListener.Instance;
	}
	
	void Update() 
	{
		// dont run Update() if there is no gesture listener
		if(!gestureListener)
			return;

		if(gestureListener.IsZoomingIn() || gestureListener.IsZoomingOut())
		{
			// zoom the model
			float zoomFactor = gestureListener.GetZoomFactor();

			Vector3 newLocalScale = new Vector3(zoomFactor, zoomFactor, zoomFactor);
			transform.localScale = Vector3.Lerp(transform.localScale, newLocalScale, spinSpeed * Time.deltaTime);
		}

		if(gestureListener.IsTurningWheel())
		{
			// rotate the model
			float turnAngle = gestureListener.GetWheelAngle();

			Vector3 newRotation = transform.rotation.eulerAngles;
			newRotation.y += turnAngle;
			transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(newRotation), spinSpeed * Time.deltaTime);
		}

		if(gestureListener.IsRaiseHand())
		{
			// reset the model
			Vector3 newLocalScale = Vector3.one;
			transform.localScale = newLocalScale;

			Vector3 newRotation = transform.rotation.eulerAngles;
			newRotation.y = 0f;
			transform.rotation = Quaternion.Euler(newRotation);
		}

	}
	
}
