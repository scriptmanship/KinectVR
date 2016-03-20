using UnityEngine;
using System.Collections;

public class LoadLevelWhenUserDetected : MonoBehaviour 
{
	[Tooltip("User pose that will trigger the loading of next level.")]
	public KinectGestures.Gestures expectedUserPose = KinectGestures.Gestures.None;
	
	[Tooltip("Next level number. No level is loaded, if the number is negative.")]
	public int nextLevel = -1;

	[Tooltip("Whether to check for initialized KinectManager or not.")]
	public bool validateKinectManager = true;

	[Tooltip("GUI-Text used to display the debug messages.")]
	public GUIText debugText;


	private bool levelLoaded = false;
	private KinectGestures.Gestures savedCalibrationPose;


	void Start()
	{
		KinectManager manager = KinectManager.Instance;
		
		if(validateKinectManager && debugText != null)
		{
			if(manager == null || !manager.IsInitialized())
			{
				debugText.GetComponent<GUIText>().text = "KinectManager is not initialized!";
				levelLoaded = true;
			}
		}

		if(manager != null && manager.IsInitialized())
		{
			savedCalibrationPose = manager.playerCalibrationPose;
			manager.playerCalibrationPose = expectedUserPose;
		}
	}

	
	void Update() 
	{
		if(!levelLoaded && nextLevel >= 0)
		{
			KinectManager manager = KinectManager.Instance;
			
			if(manager != null && manager.IsUserDetected())
			{
				manager.playerCalibrationPose = savedCalibrationPose;

				levelLoaded = true;
				Application.LoadLevel(nextLevel);
			}
		}
	}
	
}
