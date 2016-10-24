using UnityEngine;
using System.Collections;

public class ModelHatController : MonoBehaviour 
{
	[Tooltip("Index of the player, tracked by this component. 0 means the 1st player, 1 - the 2nd one, 2 - the 3rd one, etc.")]
	public int playerIndex = 0;
	
	[Tooltip("Vertical offset of the hat above the head position (in meters).")]
	public float verticalOffset = 0f;

	[Tooltip("Smooth factor used for hat-model movement and rotation.")]
	public float smoothFactor = 10f;

	private KinectManager kinectManager;
	private FacetrackingManager faceManager;
	private Quaternion initialRotation;
	


	void Start () 
	{
		initialRotation = transform.rotation;
	}
	
	void Update () 
	{
		// get the face-tracking manager instance
		if(faceManager == null)
		{
			kinectManager = KinectManager.Instance;
			faceManager = FacetrackingManager.Instance;
		}
		
		if(kinectManager && faceManager && faceManager.IsTrackingFace())
		{
			// get user-id by user-index
			long userId = kinectManager.GetUserIdByIndex(playerIndex);
			if(userId == 0)
				return;

			// head rotation
			Quaternion newRotation = initialRotation * faceManager.GetHeadRotation(userId, true);
			
			if(smoothFactor != 0f)
				transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, smoothFactor * Time.deltaTime);
			else
				transform.rotation = newRotation;

			// head position
			Vector3 newPosition = faceManager.GetHeadPosition(userId, true);

			if(verticalOffset != 0f)
			{
				Vector3 dirHead = new Vector3(0, verticalOffset, 0);
				dirHead = transform.InverseTransformDirection(dirHead);
				newPosition += dirHead;
			}
			
//			if(smoothFactor != 0f)
//				transform.position = Vector3.Lerp(transform.position, newPosition, smoothFactor * Time.deltaTime);
//			else
				transform.position = newPosition;
		}

	}
}
