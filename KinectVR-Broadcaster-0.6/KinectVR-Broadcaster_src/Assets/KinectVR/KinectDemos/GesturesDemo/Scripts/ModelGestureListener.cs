using UnityEngine;
using System.Collections;
using System;
//using Windows.Kinect;

public class ModelGestureListener : MonoBehaviour, KinectGestures.GestureListenerInterface
{
	[Tooltip("GUI-Text to display gesture-listener messages and gesture information.")]
	public GUIText gestureInfo;

	// singleton instance of the class
	private static ModelGestureListener instance = null;
	
	// internal variables to track if progress message has been displayed
	private bool progressDisplayed;
	private float progressGestureTime;

	// whether the needed gesture has been detected or not
	private bool zoomOut;
	private bool zoomIn;
	private float zoomFactor = 1f;

	private bool wheel;
	private float wheelAngle = 0f;

	private bool raiseHand = false;


	/// <summary>
	/// Gets the singleton ModelGestureListener instance.
	/// </summary>
	/// <value>The ModelGestureListener instance.</value>
	public static ModelGestureListener Instance
	{
		get
		{
			return instance;
		}
	}
	
	/// <summary>
	/// Determines whether the user is zooming out.
	/// </summary>
	/// <returns><c>true</c> if the user is zooming out; otherwise, <c>false</c>.</returns>
	public bool IsZoomingOut()
	{
		return zoomOut;
	}

	/// <summary>
	/// Determines whether the user is zooming in.
	/// </summary>
	/// <returns><c>true</c> if the user is zooming in; otherwise, <c>false</c>.</returns>
	public bool IsZoomingIn()
	{
		return zoomIn;
	}

	/// <summary>
	/// Gets the zoom factor.
	/// </summary>
	/// <returns>The zoom factor.</returns>
	public float GetZoomFactor()
	{
		return zoomFactor;
	}

	/// <summary>
	/// Determines whether the user is turning wheel.
	/// </summary>
	/// <returns><c>true</c> if the user is turning wheel; otherwise, <c>false</c>.</returns>
	public bool IsTurningWheel()
	{
		return wheel;
	}

	/// <summary>
	/// Gets the wheel angle.
	/// </summary>
	/// <returns>The wheel angle.</returns>
	public float GetWheelAngle()
	{
		return wheelAngle;
	}

	/// <summary>
	/// Determines whether the user has raised his left or right hand.
	/// </summary>
	/// <returns><c>true</c> if the user has raised his left or right hand; otherwise, <c>false</c>.</returns>
	public bool IsRaiseHand()
	{
		if(raiseHand)
		{
			raiseHand = false;
			return true;
		}
		
		return false;
	}
	

	/// <summary>
	/// Invoked when a new user is detected. Here you can start gesture tracking by invoking KinectManager.DetectGesture()-function.
	/// </summary>
	/// <param name="userId">User ID</param>
	/// <param name="userIndex">User index</param>
	public void UserDetected(long userId, int userIndex)
	{
		// the gestures are allowed for the primary user only
		KinectManager manager = KinectManager.Instance;
		if(!manager || (userId != manager.GetPrimaryUserID()))
			return;
		
		// detect these user specific gestures
		manager.DetectGesture(userId, KinectGestures.Gestures.ZoomOut);
		manager.DetectGesture(userId, KinectGestures.Gestures.ZoomIn);
		manager.DetectGesture(userId, KinectGestures.Gestures.Wheel);

		manager.DetectGesture(userId, KinectGestures.Gestures.RaiseLeftHand);
		manager.DetectGesture(userId, KinectGestures.Gestures.RaiseRightHand);
		
		if(gestureInfo != null)
		{
			gestureInfo.GetComponent<GUIText>().text = "Zoom-in, zoom-out or wheel to rotate the model. Raise hand to reset it.";
		}
	}

	/// <summary>
	/// Invoked when a user gets lost. All tracked gestures for this user are cleared automatically.
	/// </summary>
	/// <param name="userId">User ID</param>
	/// <param name="userIndex">User index</param>
	public void UserLost(long userId, int userIndex)
	{
		// the gestures are allowed for the primary user only
		KinectManager manager = KinectManager.Instance;
		if(!manager || (userId != manager.GetPrimaryUserID()))
			return;
		
		if(gestureInfo != null)
		{
			gestureInfo.GetComponent<GUIText>().text = string.Empty;
		}
	}

	/// <summary>
	/// Invoked when a gesture is in progress.
	/// </summary>
	/// <param name="userId">User ID</param>
	/// <param name="userIndex">User index</param>
	/// <param name="gesture">Gesture type</param>
	/// <param name="progress">Gesture progress [0..1]</param>
	/// <param name="joint">Joint type</param>
	/// <param name="screenPos">Normalized viewport position</param>
	public void GestureInProgress(long userId, int userIndex, KinectGestures.Gestures gesture, 
	                              float progress, KinectInterop.JointType joint, Vector3 screenPos)
	{
		// the gestures are allowed for the primary user only
		KinectManager manager = KinectManager.Instance;
		if(!manager || (userId != manager.GetPrimaryUserID()))
			return;

		if(gesture == KinectGestures.Gestures.ZoomOut)
		{
			if(progress > 0.5f)
			{
				zoomOut = true;
				zoomFactor = screenPos.z;

				if(gestureInfo != null)
				{
					string sGestureText = string.Format ("{0} - {1:F0}%", gesture, screenPos.z * 100f);
					gestureInfo.GetComponent<GUIText>().text = sGestureText;
					
					progressDisplayed = true;
					progressGestureTime = Time.realtimeSinceStartup;
				}
			}
			else
			{
				zoomOut = false;
			}
		}
		else if(gesture == KinectGestures.Gestures.ZoomIn)
		{
			if(progress > 0.5f)
			{
				zoomIn = true;
				zoomFactor = screenPos.z;

				if(gestureInfo != null)
				{
					string sGestureText = string.Format ("{0} factor: {1:F0}%", gesture, screenPos.z * 100f);
					gestureInfo.GetComponent<GUIText>().text = sGestureText;
					
					progressDisplayed = true;
					progressGestureTime = Time.realtimeSinceStartup;
				}
			}
			else
			{
				zoomIn = false;
			}
		}
		else if(gesture == KinectGestures.Gestures.Wheel)
		{
			if(progress > 0.5f)
			{
				wheel = true;
				wheelAngle = screenPos.z;
				
				if(gestureInfo != null)
				{
					string sGestureText = string.Format ("Wheel angle: {0:F0} degrees", screenPos.z);
					gestureInfo.GetComponent<GUIText>().text = sGestureText;
					
					progressDisplayed = true;
					progressGestureTime = Time.realtimeSinceStartup;
				}
			}
			else
			{
				wheel = false;
			}
		}
	}

	/// <summary>
	/// Invoked if a gesture is completed.
	/// </summary>
	/// <returns>true</returns>
	/// <c>false</c>
	/// <param name="userId">User ID</param>
	/// <param name="userIndex">User index</param>
	/// <param name="gesture">Gesture type</param>
	/// <param name="joint">Joint type</param>
	/// <param name="screenPos">Normalized viewport position</param>
	public bool GestureCompleted (long userId, int userIndex, KinectGestures.Gestures gesture, 
	                              KinectInterop.JointType joint, Vector3 screenPos)
	{
		if(gesture == KinectGestures.Gestures.RaiseLeftHand)
			raiseHand = true;
		else if(gesture == KinectGestures.Gestures.RaiseRightHand)
			raiseHand = true;

		return true;
	}

	/// <summary>
	/// Invoked if a gesture is cancelled.
	/// </summary>
	/// <returns>true</returns>
	/// <c>false</c>
	/// <param name="userId">User ID</param>
	/// <param name="userIndex">User index</param>
	/// <param name="gesture">Gesture type</param>
	/// <param name="joint">Joint type</param>
	public bool GestureCancelled (long userId, int userIndex, KinectGestures.Gestures gesture, 
	                              KinectInterop.JointType joint)
	{
		// the gestures are allowed for the primary user only
		KinectManager manager = KinectManager.Instance;
		if(!manager || (userId != manager.GetPrimaryUserID()))
			return false;
		
		if(gesture == KinectGestures.Gestures.ZoomOut)
		{
			zoomOut = false;
		}
		else if(gesture == KinectGestures.Gestures.ZoomIn)
		{
			zoomIn = false;
		}
		else if(gesture == KinectGestures.Gestures.Wheel)
		{
			wheel = false;
		}
		
		if(gestureInfo != null && progressDisplayed)
		{
			progressDisplayed = false;
			gestureInfo.GetComponent<GUIText>().text = "Zoom-in, zoom-out or wheel to rotate the model. Raise hand to reset it.";;
		}

		return true;
	}

	
	void Awake()
	{
		instance = this;
	}

	void Update()
	{
		if(progressDisplayed && ((Time.realtimeSinceStartup - progressGestureTime) > 2f))
		{
			progressDisplayed = false;
			gestureInfo.GetComponent<GUIText>().text = string.Empty;

			Debug.Log("Forced progress to end.");
		}
	}

}
