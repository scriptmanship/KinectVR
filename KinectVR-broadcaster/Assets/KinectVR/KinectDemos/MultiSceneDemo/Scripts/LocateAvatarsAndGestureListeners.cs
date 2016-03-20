using UnityEngine;
using System.Collections;

public class LocateAvatarsAndGestureListeners : MonoBehaviour 
{

	void Start () 
	{
		KinectManager manager = KinectManager.Instance;
		
		if(manager)
		{
			// remove all users, filters and avatar controllers
			manager.avatarControllers.Clear();
			manager.ClearKinectUsers();

			// get the mono scripts. avatar controllers and gesture listeners are among them
			MonoBehaviour[] monoScripts = FindObjectsOfType(typeof(MonoBehaviour)) as MonoBehaviour[];
			
			// locate the available avatar controllers
			foreach(MonoBehaviour monoScript in monoScripts)
			{
				if(typeof(AvatarController).IsAssignableFrom(monoScript.GetType()) &&
				   monoScript.enabled)
				{
					AvatarController avatar = (AvatarController)monoScript;
					manager.avatarControllers.Add(avatar);
				}
			}

			// locate Kinect gesture manager, if any
			manager.gestureManager = null;
			foreach(MonoBehaviour monoScript in monoScripts)
			{
				if(typeof(KinectGestures).IsAssignableFrom(monoScript.GetType()) && 
				   monoScript.enabled)
				{
					manager.gestureManager = (KinectGestures)monoScript;
					break;
				}
			}

			// locate the available gesture listeners
			manager.gestureListeners.Clear();

			foreach(MonoBehaviour monoScript in monoScripts)
			{
				if(typeof(KinectGestures.GestureListenerInterface).IsAssignableFrom(monoScript.GetType()) &&
				   monoScript.enabled)
				{
					//KinectGestures.GestureListenerInterface gl = (KinectGestures.GestureListenerInterface)monoScript;
					manager.gestureListeners.Add(monoScript);
				}
			}

		}
	}
	
}
