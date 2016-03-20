using UnityEngine;
using System.Collections;

public class OverlayController : MonoBehaviour 
{
	[Tooltip("GUI-texture used to display the color camera feed on the scene background.")]
	public GUITexture backgroundImage;

	[Tooltip("Camera that will be set-up to display the background image in the Kinect FOV.")]
	public Camera backgroundCamera;

	[Tooltip("Camera that will be set-up to display 3D-models in the Kinect FOV.")]
	public Camera foregroundCamera;

	[Tooltip("Use this setting to minimize the offset between the image and the model overlay.")]
	[Range(-0.1f, 0.1f)]
	public float adjustedCameraOffset = 0f;

	[Tooltip("GUI-Text used to display the overlay controller messages.")]
	public GUIText debugText;


	// variable to track the current camera offset
	private float currentCameraOffset = 0f;


	void Start () 
	{
		KinectManager manager = KinectManager.Instance;
		
		if(manager && manager.IsInitialized())
		{
			KinectInterop.SensorData sensorData = manager.GetSensorData();

			if(foregroundCamera != null && sensorData != null && sensorData.sensorInterface != null)
			{
				foregroundCamera.transform.position = new Vector3(sensorData.depthCameraOffset + adjustedCameraOffset, 
				                                                  manager.sensorHeight, 0f);
				foregroundCamera.transform.rotation = Quaternion.Euler(-manager.sensorAngle, 0f, 0f);
				currentCameraOffset = adjustedCameraOffset;

				foregroundCamera.fieldOfView = sensorData.colorCameraFOV;
			}

			if(backgroundCamera != null && sensorData != null && sensorData.sensorInterface != null)
			{
				backgroundCamera.transform.position = new Vector3(0f, manager.sensorHeight, 0f);
				backgroundCamera.transform.rotation = Quaternion.Euler(-manager.sensorAngle, 0f, 0f);
			}

			if(debugText != null)
			{
				debugText.GetComponent<GUIText>().text = "Please stand in T-pose for calibration.";
			}
		}
		else
		{
			string sMessage = "KinectManager is missing or not initialized";
			Debug.LogError(sMessage);

			if(debugText != null)
			{
				debugText.GetComponent<GUIText>().text = sMessage;
			}
		}
	}

	void Update () 
	{
		KinectManager manager = KinectManager.Instance;
		
		if(manager && manager.IsInitialized())
		{
			KinectInterop.SensorData sensorData = manager.GetSensorData();
			
			if(manager.autoHeightAngle == KinectManager.AutoHeightAngle.AutoUpdate || 
			   manager.autoHeightAngle == KinectManager.AutoHeightAngle.AutoUpdateAndShowInfo ||
			   currentCameraOffset != adjustedCameraOffset)
			{
				// update the cameras automatically, according to the current sensor height and angle
				if(foregroundCamera != null && sensorData != null)
				{
					foregroundCamera.transform.position = new Vector3(sensorData.depthCameraOffset + adjustedCameraOffset, 
					                                                  manager.sensorHeight, 0f);
					foregroundCamera.transform.rotation = Quaternion.Euler(-manager.sensorAngle, 0f, 0f);
					currentCameraOffset = adjustedCameraOffset;
				}
				
				if(backgroundCamera != null && sensorData != null)
				{
					backgroundCamera.transform.position = new Vector3(0f, manager.sensorHeight, 0f);
					backgroundCamera.transform.rotation = Quaternion.Euler(-manager.sensorAngle, 0f, 0f);
				}
				
			}
			
			if(backgroundImage)
			{
				if(backgroundImage.texture == null)
				{
					backgroundImage.texture = manager.GetUsersClrTex();
				}
			}

			MonoBehaviour[] monoScripts = FindObjectsOfType(typeof(MonoBehaviour)) as MonoBehaviour[];

			foreach(MonoBehaviour monoScript in monoScripts)
			{
				if(typeof(AvatarScaler).IsAssignableFrom(monoScript.GetType()) &&
				   monoScript.enabled)
				{
					AvatarScaler scaler = (AvatarScaler)monoScript;

					int userIndex = scaler.playerIndex;
					long userId = manager.GetUserIdByIndex(userIndex);

					if(userId != scaler.currentUserId)
					{
						scaler.currentUserId = userId;
					
						if(userId != 0)
						{
							scaler.GetUserBodySize(true, true, true);
							scaler.FixJointsBeforeScale();
							scaler.ScaleAvatar(0f);
						}
					}
				}
			}

			if(!manager.IsUserDetected())
			{
				if(debugText != null)
				{
					debugText.GetComponent<GUIText>().text = "Please stand in T-pose for calibration.";
				}
			}

		}

	}

}
