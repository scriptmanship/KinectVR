using UnityEngine;
using System.Collections;
//using Windows.Kinect;


public class HandOverlayer : MonoBehaviour 
{
	[Tooltip("GUI-texture used to display the color camera feed on the scene background.")]
	public GUITexture backgroundImage;

	[Tooltip("Index of the player, tracked by this component. 0 means the 1st player, 1 - the 2nd one, 2 - the 3rd one, etc.")]
	public int playerIndex = 0;

	[Tooltip("Whether the player is left handed or not.")]
	public bool isLeftHanded = false;

	[Tooltip("Hand-cursor texture for the hand-grip state.")]
	public Texture gripHandTexture;
	[Tooltip("Hand-cursor texture for the hand-release state.")]
	public Texture releaseHandTexture;
	[Tooltip("Hand-cursor texture for the non-tracked state.")]
	public Texture normalHandTexture;

	[Tooltip("Smooth factor for cursor movement.")]
	public float smoothFactor = 10f;

	// current cursor position
	private Vector2 cursorPos = Vector2.zero;

	// last hand event (grip or release)
	private InteractionManager.HandEventType lastHandEvent = InteractionManager.HandEventType.None;


	/// <summary>
	/// Gets the cursor position.
	/// </summary>
	/// <returns>The cursor position.</returns>
	public Vector2 GetCursorPos()
	{
		return cursorPos;
	}


	/// <summary>
	/// Gets the last hand event of the active hand (right or left).
	/// </summary>
	/// <returns>The last hand event.</returns>
	public InteractionManager.HandEventType GetLastHandEvent()
	{
		return lastHandEvent;
	}


	// ----- end of public functions -----


	void Update () 
	{
		KinectManager manager = KinectManager.Instance;
		if(manager && manager.IsInitialized())
		{
			//backgroundImage.renderer.material.mainTexture = manager.GetUsersClrTex();
			if(backgroundImage && (backgroundImage.texture == null))
			{
				backgroundImage.texture = manager.GetUsersClrTex();
			}

			// overlay the joint
			int iJointIndex = !isLeftHanded ? (int)KinectInterop.JointType.HandRight : (int)KinectInterop.JointType.HandLeft;

			if(manager.IsUserDetected())
			{
				long userId = manager.GetUserIdByIndex(playerIndex);
				
				if(manager.IsJointTracked(userId, iJointIndex))
				{
					Vector3 posJointRaw = manager.GetJointKinectPosition(userId, iJointIndex);
					//Vector3 posJoint = manager.GetJointPosColorOverlay(userId, iJointIndex, Camera.main, Camera.main.pixelRect);

					if(posJointRaw != Vector3.zero)
					{
						Vector2 posDepth = manager.MapSpacePointToDepthCoords(posJointRaw);
						ushort depthValue = manager.GetDepthForPixel((int)posDepth.x, (int)posDepth.y);
						
						if(posDepth != Vector2.zero && depthValue > 0)
						{
							// depth pos to color pos
							Vector2 posColor = manager.MapDepthPointToColorCoords(posDepth, depthValue);
							
							if(!float.IsInfinity(posColor.x) && !float.IsInfinity(posColor.y))
							{
								float xScaled = posColor.x / manager.GetColorImageWidth();
								float yScaled = posColor.y / manager.GetColorImageHeight();
								
								cursorPos = Vector2.Lerp(cursorPos, new Vector2(xScaled, 1f - yScaled), smoothFactor * Time.deltaTime);
							}
						}
					}
				}
				
			}
			
		}
	}

	void OnGUI()
	{
		InteractionManager intManager = InteractionManager.Instance;
		Texture texture = null;

		if(intManager && intManager.IsInteractionInited())
		{
			if(isLeftHanded)
			{
				lastHandEvent = intManager.GetLastLeftHandEvent();

				if(lastHandEvent == InteractionManager.HandEventType.Grip)
					texture = gripHandTexture;
				else if(lastHandEvent == InteractionManager.HandEventType.Release)
					texture = releaseHandTexture;
			}
			else
			{
				lastHandEvent = intManager.GetLastRightHandEvent();

				if(lastHandEvent == InteractionManager.HandEventType.Grip)
					texture = gripHandTexture;
				else if(lastHandEvent == InteractionManager.HandEventType.Release)
					texture = releaseHandTexture;
			}
		}

		if(texture == null)
		{
			texture = normalHandTexture;
		}
		
		if((cursorPos != Vector2.zero) && (texture != null))
		{
			//handCursor.transform.position = cursorScreenPos; // Vector3.Lerp(handCursor.transform.position, cursorScreenPos, 3 * Time.deltaTime);
			Rect rectTexture = new Rect(cursorPos.x * Screen.width - texture.width / 2, (1f - cursorPos.y) * Screen.height - texture.height / 2, 
			                            texture.width, texture.height);
			GUI.DrawTexture(rectTexture, texture);
		}
	}


}
