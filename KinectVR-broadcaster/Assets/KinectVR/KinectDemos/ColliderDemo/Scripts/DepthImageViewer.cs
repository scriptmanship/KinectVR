using UnityEngine;
using System.Collections;

public class DepthImageViewer : MonoBehaviour 
{
	[Tooltip("Index of the player, tracked by this component. 0 means the 1st player, 1 - the 2nd one, 2 - the 3rd one, etc.")]
	public int playerIndex = 0;
	
	[Tooltip("Camera that will be used to overlay the 3D-objects over the background.")]
	public Camera foregroundCamera;
	
	// the KinectManager instance
	private KinectManager manager;

	// the foreground texture
	private Texture2D foregroundTex;
	
	// rectangle taken by the foreground texture (in pixels)
	private Rect foregroundGuiRect;
	private Rect foregroundImgRect;

	// game objects to contain the joint colliders
	private GameObject[] jointColliders = null;
	private int numColliders = 0;

	private int depthImageWidth;
	private int depthImageHeight;
	

	void Start () 
	{
		manager = KinectManager.Instance;

		if(manager && manager.IsInitialized())
		{
			KinectInterop.SensorData sensorData = manager.GetSensorData();

			if(sensorData != null && sensorData.sensorInterface != null && foregroundCamera != null)
			{
				// get depth image size
				depthImageWidth = sensorData.depthImageWidth;
				depthImageHeight = sensorData.depthImageHeight;

				// calculate the foreground rectangles
				Rect cameraRect = foregroundCamera.pixelRect;
				float rectHeight = cameraRect.height;
				float rectWidth = cameraRect.width;
				
				if(rectWidth > rectHeight)
					rectWidth = rectHeight * depthImageWidth / depthImageHeight;
				else
					rectHeight = rectWidth * depthImageHeight / depthImageWidth;
				
				float foregroundOfsX = (cameraRect.width - rectWidth) / 2;
				float foregroundOfsY = (cameraRect.height - rectHeight) / 2;
				foregroundImgRect = new Rect(foregroundOfsX, foregroundOfsY, rectWidth, rectHeight);
				foregroundGuiRect = new Rect(foregroundOfsX, cameraRect.height - foregroundOfsY, rectWidth, -rectHeight);
				
				// create joint colliders
				numColliders = sensorData.jointCount;
				jointColliders = new GameObject[numColliders];
				
				for(int i = 0; i < numColliders; i++)
				{
					string sColObjectName = ((KinectInterop.JointType)i).ToString() + "Collider";
					jointColliders[i] = new GameObject(sColObjectName);
					jointColliders[i].transform.parent = transform;
					
					SphereCollider collider = jointColliders[i].AddComponent<SphereCollider>();
					collider.radius = 0.2f;
				}
			}
		}

	}
	
	void Update () 
	{
		// get the users texture
		if(manager && manager.IsInitialized())
		{
			foregroundTex = manager.GetUsersLblTex();
		}

		if(manager && manager.IsUserDetected() && foregroundCamera)
		{
			long userId = manager.GetUserIdByIndex(playerIndex);  // manager.GetPrimaryUserID();

			// update colliders
			for(int i = 0; i < numColliders; i++)
			{
				if(manager.IsJointTracked(userId, i))
				{
					Vector3 posCollider = manager.GetJointPosDepthOverlay(userId, i, foregroundCamera, foregroundImgRect);
					jointColliders[i].transform.position = posCollider;
				}
			}
		}

	}

	void OnGUI()
	{
		if(foregroundTex)
		{
			GUI.DrawTexture(foregroundGuiRect, foregroundTex);
		}
	}

}
