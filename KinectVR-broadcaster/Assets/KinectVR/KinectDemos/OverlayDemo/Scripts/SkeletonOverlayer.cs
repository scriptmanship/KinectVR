using UnityEngine;
using System.Collections;
//using Windows.Kinect;


public class SkeletonOverlayer : MonoBehaviour 
{
	[Tooltip("GUI-texture used to display the color camera feed on the scene background.")]
	public GUITexture backgroundImage;

	[Tooltip("Camera that will be used to overlay the 3D-objects over the background.")]
	public Camera foregroundCamera;
	
	[Tooltip("Index of the player, tracked by this component. 0 means the 1st player, 1 - the 2nd one, 2 - the 3rd one, etc.")]
	public int playerIndex = 0;
	
	[Tooltip("Game object used to overlay the joints.")]
	public GameObject jointPrefab;

	[Tooltip("Line object used to overlay the bones.")]
	public LineRenderer linePrefab;
	//public float smoothFactor = 10f;

	//public GUIText debugText;
	
	private GameObject[] joints = null;
	private LineRenderer[] lines = null;

	private Quaternion initialRotation = Quaternion.identity;


	void Start()
	{
		KinectManager manager = KinectManager.Instance;

		if(manager && manager.IsInitialized())
		{
			int jointsCount = manager.GetJointCount();

			if(jointPrefab)
			{
				// array holding the skeleton joints
				joints = new GameObject[jointsCount];
				
				for(int i = 0; i < joints.Length; i++)
				{
					joints[i] = Instantiate(jointPrefab) as GameObject;
					joints[i].transform.parent = transform;
					joints[i].name = ((KinectInterop.JointType)i).ToString();
					joints[i].SetActive(false);
				}
			}

			// array holding the skeleton lines
			lines = new LineRenderer[jointsCount];

//			if(linePrefab)
//			{
//				for(int i = 0; i < lines.Length; i++)
//				{
//					lines[i] = Instantiate(linePrefab) as LineRenderer;
//					lines[i].transform.parent = transform;
//					lines[i].gameObject.SetActive(false);
//				}
//			}
		}

		// always mirrored
		initialRotation = Quaternion.Euler(new Vector3(0f, 180f, 0f));
	}
	
	void Update () 
	{
		KinectManager manager = KinectManager.Instance;
		
		if(manager && manager.IsInitialized() && foregroundCamera)
		{
			//backgroundImage.renderer.material.mainTexture = manager.GetUsersClrTex();
			if(backgroundImage && (backgroundImage.texture == null))
			{
				backgroundImage.texture = manager.GetUsersClrTex();
			}

			// get the background rectangle (use the portrait background, if available)
			Rect backgroundRect = foregroundCamera.pixelRect;
			PortraitBackground portraitBack = PortraitBackground.Instance;
			
			if(portraitBack && portraitBack.enabled)
			{
				backgroundRect = portraitBack.GetBackgroundRect();
			}

			// overlay all joints in the skeleton
			if(manager.IsUserDetected())
			{
				long userId = manager.GetUserIdByIndex(playerIndex);
				int jointsCount = manager.GetJointCount();

				for(int i = 0; i < jointsCount; i++)
				{
					int joint = i;

					if(manager.IsJointTracked(userId, joint))
					{
						Vector3 posJoint = manager.GetJointPosColorOverlay(userId, joint, foregroundCamera, backgroundRect);
						//Vector3 posJoint = manager.GetJointPosition(userId, joint);

						if(joints != null)
						{
							// overlay the joint
							if(posJoint != Vector3.zero)
							{
//								if(debugText && joint == 0)
//								{
//									debugText.GetComponent<GUIText>().text = string.Format("{0} - {1}\nRealPos: {2}", 
//									                                       (KinectInterop.JointType)joint, posJoint,
//									                                       manager.GetJointPosition(userId, joint));
//								}
								
								joints[i].SetActive(true);
								joints[i].transform.position = posJoint;

								Quaternion rotJoint = manager.GetJointOrientation(userId, joint, false);
								rotJoint = initialRotation * rotJoint;
								joints[i].transform.rotation = rotJoint;
							}
							else
							{
								joints[i].SetActive(false);
							}
						}

						if(lines[i] == null && linePrefab != null)
						{
							lines[i] = Instantiate(linePrefab) as LineRenderer;
							lines[i].transform.parent = transform;
							lines[i].gameObject.SetActive(false);
						}

						if(lines[i] != null)
						{
							// overlay the line to the parent joint
							int jointParent = (int)manager.GetParentJoint((KinectInterop.JointType)joint);
							Vector3 posParent = manager.GetJointPosColorOverlay(userId, jointParent, foregroundCamera, backgroundRect);

							if(posJoint != Vector3.zero && posParent != Vector3.zero)
							{
								lines[i].gameObject.SetActive(true);
								
								//lines[i].SetVertexCount(2);
								lines[i].SetPosition(0, posParent);
								lines[i].SetPosition(1, posJoint);
							}
							else
							{
								lines[i].gameObject.SetActive(false);
							}
						}
						
					}
					else
					{
						if(joints != null)
						{
							joints[i].SetActive(false);
						}
						
						if(lines[i] != null)
						{
							lines[i].gameObject.SetActive(false);
						}
					}
				}

			}
		}
	}

}
