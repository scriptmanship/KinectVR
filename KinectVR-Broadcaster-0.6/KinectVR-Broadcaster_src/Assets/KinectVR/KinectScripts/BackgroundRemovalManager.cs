using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;


/// <summary>
/// Background removal manager is the component that deals with Kinect background removal.
/// </summary>
public class BackgroundRemovalManager : MonoBehaviour 
{
	[Tooltip("Index of the player, tracked by this component. -1 means all players, 0 - the 1st player only, 1 - the 2nd player only, etc.")]
	public int playerIndex = -1;
	
	[Tooltip("Camera used to display the foreground texture on the screen. Leave empty, if on-screen display of the foreground texture is not required.")]
	public Camera foregroundCamera;

	[Tooltip("Whether the hi-res (color camera resolution) is preferred for the foreground image. Otherwise the depth camera resolution will be used.")]
	public bool colorCameraResolution = true;
	
	[Tooltip("Color used to paint pixels, where the foreground color data is not available.")]
	public Color32 defaultColor = new Color32(64, 64, 64, 255);
	
	[Tooltip("GUI-Text to display the BR-Manager debug messages.")]
	public GUIText debugText;

	// buffer for the raw foreground image
	private byte[] foregroundImage;
	
	// the foreground texture
	private Texture2D foregroundTex;
	
	// rectangle taken by the foreground texture (in pixels)
	private Rect foregroundRect;
	
	// primary sensor data structure
	private KinectInterop.SensorData sensorData = null;
	
	// Bool to keep track whether Kinect and BR library have been initialized
	private bool isBrInited = false;
	
	// The single instance of BackgroundRemovalManager
	private static BackgroundRemovalManager instance;
	
	
	/// <summary>
	/// Gets the single BackgroundRemovalManager instance.
	/// </summary>
	/// <value>The BackgroundRemovalManager instance.</value>
    public static BackgroundRemovalManager Instance
    {
        get
        {
            return instance;
        }
    }
	
	/// <summary>
	/// Determines whether the BackgroundRemovalManager was successfully initialized.
	/// </summary>
	/// <returns><c>true</c> if the BackgroundRemovalManager was successfully initialized; otherwise, <c>false</c>.</returns>
	public bool IsBackgroundRemovalInitialized()
	{
		return isBrInited;
	}
	
//	// returns the raw foreground image
//	public byte[] GetForegroundImage()
//	{
//		return foregroundImage;
//	}
	
	/// <summary>
	/// Gets the foreground image texture.
	/// </summary>
	/// <returns>The foreground image texture.</returns>
	public Texture GetForegroundTex()
	{ 
		bool bHiResSupported = sensorData != null && sensorData.sensorInterface != null ?
			sensorData.sensorInterface.IsBRHiResSupported() : false;
		bool bKinect1Int = sensorData != null && sensorData.sensorInterface != null ?
			(sensorData.sensorInterface.GetSensorPlatform() == KinectInterop.DepthSensorPlatform.KinectSDKv1) : false;
		
		if(sensorData != null && bHiResSupported && !bKinect1Int && sensorData.color2DepthTexture)
		{
			return sensorData.color2DepthTexture;
		}
		else if(sensorData != null && !bKinect1Int && sensorData.depth2ColorTexture)
		{
			return sensorData.depth2ColorTexture;
		}
		
		return foregroundTex;
	}

	/// <summary>
	/// Gets the alpha body texture.
	/// </summary>
	/// <returns>The alpha body texture.</returns>
	public Texture GetAlphaBodyTex()
	{
		if(sensorData != null)
		{
			return sensorData.alphaBodyTexture;
		}

		return null;
	}
	
	//----------------------------------- end of public functions --------------------------------------//
	
	void Start() 
	{
		try 
		{
			// get sensor data
			KinectManager kinectManager = KinectManager.Instance;
			if(kinectManager && kinectManager.IsInitialized())
			{
				sensorData = kinectManager.GetSensorData();
			}
			
			if(sensorData == null || sensorData.sensorInterface == null)
			{
				throw new Exception("Background removal cannot be started, because KinectManager is missing or not initialized.");
			}
			
			// ensure the needed dlls are in place and speech recognition is available for this interface
			bool bNeedRestart = false;
			bool bSuccess = sensorData.sensorInterface.IsBackgroundRemovalAvailable(ref bNeedRestart);

			if(bSuccess)
			{
				if(bNeedRestart)
				{
					KinectInterop.RestartLevel(gameObject, "BR");
					return;
				}
			}
			else
			{
				string sInterfaceName = sensorData.sensorInterface.GetType().Name;
				throw new Exception(sInterfaceName + ": Background removal is not supported!");
			}
			
			// Initialize the background removal
			bSuccess = sensorData.sensorInterface.InitBackgroundRemoval(sensorData, colorCameraResolution);

			if (!bSuccess)
	        {
				throw new Exception("Background removal could not be initialized.");
	        }

			// create the foreground image and alpha-image
			int imageLength = sensorData.sensorInterface.GetForegroundFrameLength(sensorData, colorCameraResolution);
			foregroundImage = new byte[imageLength];

			// get the needed rectangle
			Rect neededFgRect = sensorData.sensorInterface.GetForegroundFrameRect(sensorData, colorCameraResolution);

			// create the foreground texture
			foregroundTex = new Texture2D((int)neededFgRect.width, (int)neededFgRect.height, TextureFormat.RGBA32, false);

			// calculate the foreground rectangle
			if(foregroundCamera != null)
			{
				Rect cameraRect = foregroundCamera.pixelRect;
				float rectHeight = cameraRect.height;
				float rectWidth = cameraRect.width;
				
				if(rectWidth > rectHeight)
					rectWidth = Mathf.Round(rectHeight * neededFgRect.width / neededFgRect.height);
				else
					rectHeight = Mathf.Round(rectWidth * neededFgRect.height / neededFgRect.width);
				
				foregroundRect = new Rect((cameraRect.width - rectWidth) / 2, cameraRect.height - (cameraRect.height - rectHeight) / 2, rectWidth, -rectHeight);
			}

			instance = this;
			isBrInited = true;
			
			//DontDestroyOnLoad(gameObject);
		} 
		catch(DllNotFoundException ex)
		{
			Debug.LogError(ex.ToString());
			if(debugText != null)
				debugText.GetComponent<GUIText>().text = "Please check the Kinect and BR-Library installations.";
		}
		catch (Exception ex) 
		{
			Debug.LogError(ex.ToString());
			if(debugText != null)
				debugText.GetComponent<GUIText>().text = ex.Message;
		}
	}

	void OnDestroy()
	{
		if(isBrInited && sensorData != null && sensorData.sensorInterface != null)
		{
			// finish background removal
			sensorData.sensorInterface.FinishBackgroundRemoval(sensorData);
		}
		
		isBrInited = false;
		instance = null;
	}
	
	void Update () 
	{
		if(isBrInited)
		{
			// select one player or all players
			if(playerIndex != -1)
			{
				KinectManager kinectManager = KinectManager.Instance;
				long userID = 0;

				if(kinectManager && kinectManager.IsInitialized())
				{
					userID = kinectManager.GetUserIdByIndex(playerIndex);

					if(userID != 0)
					{
						sensorData.selectedBodyIndex = (byte)kinectManager.GetBodyIndexByUserId(userID);
					}
				}

				if(userID == 0)
				{
					// don't display anything - set fictive index
					sensorData.selectedBodyIndex = 222;
				}
			}
			else
			{
				// show all players
				sensorData.selectedBodyIndex = 255;
			}

			// update the background removal
			bool bSuccess = sensorData.sensorInterface.UpdateBackgroundRemoval(sensorData, colorCameraResolution, defaultColor);
			
			if(bSuccess)
			{
				KinectManager kinectManager = KinectManager.Instance;
				if(kinectManager && kinectManager.IsInitialized())
				{
					bool bLimitedUsers = kinectManager.IsTrackedUsersLimited();
					List<int> alTrackedIndexes = kinectManager.GetTrackedBodyIndices();
					bSuccess = sensorData.sensorInterface.PollForegroundFrame(sensorData, colorCameraResolution, defaultColor, bLimitedUsers, alTrackedIndexes, ref foregroundImage);

					if(bSuccess)
					{
						foregroundTex.LoadRawTextureData(foregroundImage);
						foregroundTex.Apply();
					}
				}
			}
		}
	}
	
	void OnGUI()
	{
		if(isBrInited && foregroundCamera)
		{
			// get the foreground rectangle (use the portrait background, if available)
			PortraitBackground portraitBack = PortraitBackground.Instance;
			if(portraitBack && portraitBack.enabled)
			{
				foregroundRect = portraitBack.GetBackgroundRect();

				foregroundRect.y += foregroundRect.height;  // invert y
				foregroundRect.height = -foregroundRect.height;
			}

			// update the foreground texture
			bool bHiResSupported = sensorData != null && sensorData.sensorInterface != null ?
				sensorData.sensorInterface.IsBRHiResSupported() : false;
			bool bKinect1Int = sensorData != null && sensorData.sensorInterface != null ?
				(sensorData.sensorInterface.GetSensorPlatform() == KinectInterop.DepthSensorPlatform.KinectSDKv1) : false;

			if(sensorData != null && bHiResSupported && !bKinect1Int && sensorData.color2DepthTexture)
			{
				//GUI.DrawTexture(foregroundRect, sensorData.alphaBodyTexture);
				GUI.DrawTexture(foregroundRect, sensorData.color2DepthTexture);
			}
			else if(sensorData != null && !bKinect1Int && sensorData.depth2ColorTexture)
			{
				//GUI.DrawTexture(foregroundRect, sensorData.alphaBodyTexture);
				GUI.DrawTexture(foregroundRect, sensorData.depth2ColorTexture);
			}
			else if(foregroundTex)
			{
				//GUI.DrawTexture(foregroundRect, sensorData.alphaBodyTexture);
				GUI.DrawTexture(foregroundRect, foregroundTex);
			}
		}
	}


}
