using UnityEngine;
using System.Collections;

public class SetFaceTexture : MonoBehaviour 
{
	[Tooltip("Face rectangle width in meters.")]
	public float faceWidth = 0.25f;
	[Tooltip("Face rectangle height in meters.")]
	public float faceHeight = 0.25f;

	[Tooltip("Game object used to display the face texture.")]
	public GameObject targetObject;


	private Renderer targetRenderer;
	private Rect faceRect;
	private Texture2D colorTex, faceTex;
	private KinectManager kinectManager;
	private FacetrackingManager faceManager;

	//private const int pixelAlignment = -2;  // must be negative power of 2


	/// <summary>
	/// Gets the tracked face texture.
	/// </summary>
	/// <returns>The face texture.</returns>
	public Texture GetFaceTex()
	{
		return faceTex;
	}


	void Start () 
	{
		kinectManager = KinectManager.Instance;
		faceTex = new Texture2D(100, 100, TextureFormat.ARGB32, false);

		if(targetObject)
		{
			targetRenderer = targetObject.GetComponent<Renderer>();

			if(targetRenderer && targetRenderer.material)
			{
				targetRenderer.material.SetTextureScale("_MainTex", new Vector2(1, -1));
			}
		}
	}
	
	void Update () 
	{
		if(faceManager == null)
		{
			faceManager = FacetrackingManager.Instance;
		}

		if(!kinectManager || !kinectManager.IsInitialized())
			return;
		if(!faceManager || !faceManager.IsFaceTrackingInitialized())
			return;

		long userId = kinectManager.GetUserIdByIndex(faceManager.playerIndex);
		if(userId == 0)
			return;

		colorTex = kinectManager.GetUsersClrTex();
		//faceRect = faceManager.GetFaceColorRect(userId);
		faceRect = GetHeadJointFaceRect(userId);

		if(!colorTex || faceRect.width <= 0 || faceRect.height <= 0)
			return;

		int faceX = (int)faceRect.x;
		int faceY = (int)faceRect.y;
		int faceW = (int)faceRect.width;
		int faceH = (int)faceRect.height;

		if(faceX < 0) faceX = 0;
		if(faceY < 0) faceY = 0;
		if((faceX + faceW) > colorTex.width) faceW = colorTex.width - faceX;
		if((faceY + faceH) > colorTex.height) faceH = colorTex.height - faceY;

		if(faceTex.width != faceW || faceTex.height != faceH)
		{
			faceTex.Resize(faceW, faceH);
		}

		Color[] colorPixels = colorTex.GetPixels(faceX, faceY, faceW, faceH, 0);
		faceTex.SetPixels(colorPixels);
		faceTex.Apply();

		if(targetRenderer && targetRenderer.material)
		{
			targetRenderer.material.mainTexture = faceTex;
		}
	}

	private Rect GetHeadJointFaceRect(long userId)
	{
		Rect faceJointRect = new Rect();

		if(kinectManager.IsJointTracked(userId, (int)KinectInterop.JointType.Head))
		{
			Vector3 posHeadRaw = kinectManager.GetJointKinectPosition(userId, (int)KinectInterop.JointType.Head);
			
			if(posHeadRaw != Vector3.zero)
			{
				Vector2 posDepthHead = kinectManager.MapSpacePointToDepthCoords(posHeadRaw);
				ushort depthHead = kinectManager.GetDepthForPixel((int)posDepthHead.x, (int)posDepthHead.y);
				
				Vector3 sizeHalfFace = new Vector3(faceWidth / 2f, faceHeight / 2f, 0f);
				Vector3 posFaceRaw1 = posHeadRaw - sizeHalfFace;
				Vector3 posFaceRaw2 = posHeadRaw + sizeHalfFace;
				
				Vector2 posDepthFace1 = kinectManager.MapSpacePointToDepthCoords(posFaceRaw1);
				Vector2 posDepthFace2 = kinectManager.MapSpacePointToDepthCoords(posFaceRaw2);

				if(posDepthFace1 != Vector2.zero && posDepthFace2 != Vector2.zero && depthHead > 0)
				{
					Vector2 posColorFace1 = kinectManager.MapDepthPointToColorCoords(posDepthFace1, depthHead);
					Vector2 posColorFace2 = kinectManager.MapDepthPointToColorCoords(posDepthFace2, depthHead);
					
					if(!float.IsInfinity(posColorFace1.x) && !float.IsInfinity(posColorFace1.y) &&
					   !float.IsInfinity(posColorFace2.x) && !float.IsInfinity(posColorFace2.y))
					{
						faceJointRect.x = posColorFace1.x;
						faceJointRect.y = posColorFace2.y;
						faceJointRect.width = Mathf.Abs(posColorFace2.x - posColorFace1.x);
						faceJointRect.height = Mathf.Abs(posColorFace2.y - posColorFace1.y);
					}
				}
			}
		}

		return faceJointRect;
	}

}
