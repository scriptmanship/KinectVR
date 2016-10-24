using UnityEngine;
using System.Collections;

public class SetBackgroundRemovalImage : MonoBehaviour 
{
	[Tooltip("GUI-texture used to display the color camera feed on the scene background.")]
	public GUITexture backgroundImage;

	[Tooltip("Camera that will be set-up to display 3D-models in the Kinect FOV.")]
	public Camera foregroundCamera;

	[Tooltip("Use this setting to minimize the offset between the image and the model overlay.")]
	[Range(-0.1f, 0.1f)]
	public float adjustedCameraOffset = 0f;
	
	
	// variable to track the current camera offset
	private float currentCameraOffset = 0f;
	// initial camera position
	private Vector3 initialCameraPos = Vector3.zero;
	

	void Start()
	{
		KinectManager manager = KinectManager.Instance;
		
		if(manager && manager.IsInitialized())
		{
			KinectInterop.SensorData sensorData = manager.GetSensorData();

			if(foregroundCamera != null && sensorData != null && sensorData.sensorInterface != null)
			{
				foregroundCamera.fieldOfView = sensorData.colorCameraFOV;

				initialCameraPos = foregroundCamera.transform.position;
				Vector3 fgCameraPos = initialCameraPos;
				
				fgCameraPos.x += sensorData.faceOverlayOffset + adjustedCameraOffset;
				foregroundCamera.transform.position = fgCameraPos;
				currentCameraOffset = adjustedCameraOffset;
			}
		}
	}

	void Update()
	{
		BackgroundRemovalManager backManager = BackgroundRemovalManager.Instance;
		if(backManager && backManager.IsBackgroundRemovalInitialized())
		{
			Texture foregroundTex = backManager.GetForegroundTex();

			if(backgroundImage && (backgroundImage.texture == null) && foregroundTex)
			{
				backgroundImage.texture = foregroundTex;
			}
		}

		KinectManager manager = KinectManager.Instance;
		if(manager && manager.IsInitialized())
		{
			if(currentCameraOffset != adjustedCameraOffset)
			{
				// update the camera automatically, according to the current sensor height and angle
				KinectInterop.SensorData sensorData = manager.GetSensorData();
				
				if(foregroundCamera != null && sensorData != null)
				{
					Vector3 fgCameraPos = initialCameraPos;
					fgCameraPos.x += sensorData.faceOverlayOffset + adjustedCameraOffset;
					foregroundCamera.transform.position = fgCameraPos;
					currentCameraOffset = adjustedCameraOffset;
				}
			}
		}
	}

}
