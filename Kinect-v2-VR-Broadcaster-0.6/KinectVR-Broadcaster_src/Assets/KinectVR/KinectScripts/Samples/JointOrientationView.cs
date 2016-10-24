using UnityEngine;
using System.Collections;
//using Windows.Kinect;


public class JointOrientationView : MonoBehaviour 
{
	[Tooltip("The Kinect joint we want to track.")]
	public KinectInterop.JointType trackedJoint = KinectInterop.JointType.SpineBase;

	[Tooltip("Whether the joint view is mirrored or not.")]
	public bool mirroredView = false;

	[Tooltip("Smooth factor used for the joint orientation smoothing.")]
	public float smoothFactor = 5f;

	[Tooltip("GUI-Text to display the current joint rotation.")]
	public GUIText debugText;
	
	private Quaternion initialRotation = Quaternion.identity;

	
	void Start()
	{
		initialRotation = transform.rotation;
		//transform.rotation = Quaternion.identity;
	}
	
	void Update () 
	{
		KinectManager manager = KinectManager.Instance;
		
		if(manager && manager.IsInitialized())
		{
			int iJointIndex = (int)trackedJoint;

			if(manager.IsUserDetected())
			{
				long userId = manager.GetPrimaryUserID();
				
				if(manager.IsJointTracked(userId, iJointIndex))
				{
					Quaternion qRotObject = manager.GetJointOrientation(userId, iJointIndex, !mirroredView);
					qRotObject = initialRotation * qRotObject;
					
					if(debugText)
					{
						Vector3 vRotAngles = qRotObject.eulerAngles;
						debugText.GetComponent<GUIText>().text = string.Format("{0} - R({1:000}, {2:000}, {3:000})", trackedJoint, 
						                                       vRotAngles.x, vRotAngles.y, vRotAngles.z);
					}

					if(smoothFactor != 0f)
						transform.rotation = Quaternion.Slerp(transform.rotation, qRotObject, smoothFactor * Time.deltaTime);
					else
						transform.rotation = qRotObject;
				}
				
			}
			
		}
	}
}
