using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
//using System.Runtime.InteropServices;


/// <summary>
/// Facetracking manager is the component that deals with head and face tracking.
/// </summary>
public class FacetrackingManager : MonoBehaviour 
{
	[Tooltip("Index of the player, tracked by this component. 0 means the 1st player, 1 - the 2nd one, 2 - the 3rd one, etc.")]
	public int playerIndex = 0;
	
	[Tooltip("Whether to utilize the HD-face model functionality or not.")]
	public bool getFaceModelData = false;

	[Tooltip("Whether to display the face rectangle over the color camera feed.")]
	public bool displayFaceRect = false;
	
	[Tooltip("Time tolerance (in seconds), when the face is allowed not to be tracked without losing it.")]
	public float faceTrackingTolerance = 0.25f;
	
	[Tooltip("Game object that will be used to display the HD-face model mesh in the scene.")]
	public GameObject faceModelMesh = null;
	
	[Tooltip("Whether the HD-face model mesh should be mirrored or not.")]
	public bool mirroredModelMesh = true;

	[Tooltip("GUI-Text to display the FT-manager debug messages.")]
	public GUIText debugText;

	// Is currently tracking user's face
	private bool isTrackingFace = false;
	private float lastFaceTrackedTime = 0f;
	
	// Skeleton ID of the tracked face
	//private long faceTrackingID = 0;
	
	// Animation units
	private Dictionary<KinectInterop.FaceShapeAnimations, float> dictAU = new Dictionary<KinectInterop.FaceShapeAnimations, float>();
	private bool bGotAU = false;

	// Shape units
	private Dictionary<KinectInterop.FaceShapeDeformations, float> dictSU = new Dictionary<KinectInterop.FaceShapeDeformations, float>();
	private bool bGotSU = false;

	// whether the face model mesh was initialized
	private bool bFaceModelMeshInited = false;

	// Vertices of the face model
	private Vector3[] avModelVertices = null;
	private bool bGotModelVertices = false;

	// Head position and rotation
	private Vector3 headPos = Vector3.zero;
	private bool bGotHeadPos = false;

	private Quaternion headRot = Quaternion.identity;
	private bool bGotHeadRot = false;
	
	// Tracked face rectangle
//	private Rect faceRect;
//	private bool bGotFaceRect;

	// primary user ID, as reported by KinectManager
	private long primaryUserID = 0;

	// primary sensor data structure
	private KinectInterop.SensorData sensorData = null;
	
	// Bool to keep track of whether face-tracking system has been initialized
	private bool isFacetrackingInitialized = false;
	
	// The single instance of FacetrackingManager
	private static FacetrackingManager instance;
	

	/// <summary>
	/// Gets the single FacetrackingManager instance.
	/// </summary>
	/// <value>The FacetrackingManager instance.</value>
    public static FacetrackingManager Instance
    {
        get
        {
            return instance;
        }
    }
	
	/// <summary>
	/// Determines the facetracking system was successfully initialized, false otherwise.
	/// </summary>
	/// <returns><c>true</c> if the facetracking system was successfully initialized; otherwise, <c>false</c>.</returns>
	public bool IsFaceTrackingInitialized()
	{
		return isFacetrackingInitialized;
	}
	
	/// <summary>
	/// Determines whether this the sensor is currently tracking a face.
	/// </summary>
	/// <returns><c>true</c> if the sensor is tracking a face; otherwise, <c>false</c>.</returns>
	public bool IsTrackingFace()
	{
		return isTrackingFace;
	}

	/// <summary>
	/// Gets the current user ID, or 0 if no user is currently tracked.
	/// </summary>
	/// <returns>The face tracking I.</returns>
	public long GetFaceTrackingID()
	{
		return isTrackingFace ? primaryUserID : 0;
	}
	
	/// <summary>
	/// Determines whether the sensor is currently tracking the face of the specified user.
	/// </summary>
	/// <returns><c>true</c> if the sensor is currently tracking the face of the specified user; otherwise, <c>false</c>.</returns>
	/// <param name="userId">User ID</param>
	public bool IsTrackingFace(long userId)
	{
		if(sensorData != null && sensorData.sensorInterface != null)
		{
			return sensorData.sensorInterface.IsFaceTracked(userId);
		}

		return false;
	}
	
	/// <summary>
	/// Gets the head position of the currently tracked user.
	/// </summary>
	/// <returns>The head position.</returns>
	/// <param name="bMirroredMovement">If set to <c>true</c> returns mirorred head position.</param>
	public Vector3 GetHeadPosition(bool bMirroredMovement)
	{
		Vector3 vHeadPos = bGotHeadPos ? headPos : Vector3.zero;

		if(!bMirroredMovement)
		{
			vHeadPos.z = -vHeadPos.z;
		}
		
		return vHeadPos;
	}
	
	/// <summary>
	/// Gets the head position of the specified user.
	/// </summary>
	/// <returns>The head position.</returns>
	/// <param name="userId">User ID</param>
	/// <param name="bMirroredMovement">If set to <c>true</c> returns mirorred head position.</param>
	public Vector3 GetHeadPosition(long userId, bool bMirroredMovement)
	{
		Vector3 vHeadPos = Vector3.zero;
		bool bGotPosition = sensorData.sensorInterface.GetHeadPosition(userId, ref vHeadPos);

		if(bGotPosition)
		{
			if(!bMirroredMovement)
			{
				vHeadPos.z = -vHeadPos.z;
			}
			
			return vHeadPos;
		}

		return Vector3.zero;
	}
	
	/// <summary>
	/// Gets the head rotation of the currently tracked user.
	/// </summary>
	/// <returns>The head rotation.</returns>
	/// <param name="bMirroredMovement">If set to <c>true</c> returns mirorred head rotation.</param>
	public Quaternion GetHeadRotation(bool bMirroredMovement)
	{
		Vector3 rotAngles = bGotHeadRot ? headRot.eulerAngles : Vector3.zero;

		if(bMirroredMovement)
		{
			rotAngles.x = -rotAngles.x;
			rotAngles.z = -rotAngles.z;
		}
		else
		{
			rotAngles.x = -rotAngles.x;
			rotAngles.y = -rotAngles.y;
		}
		
		return Quaternion.Euler(rotAngles);
	}
	
	/// <summary>
	/// Gets the head rotation of the specified user.
	/// </summary>
	/// <returns>The head rotation.</returns>
	/// <param name="userId">User ID</param>
	/// <param name="bMirroredMovement">If set to <c>true</c> returns mirorred head rotation.</param>
	public Quaternion GetHeadRotation(long userId, bool bMirroredMovement)
	{
		Quaternion vHeadRot = Quaternion.identity;
		bool bGotRotation = sensorData.sensorInterface.GetHeadRotation(userId, ref vHeadRot);

		if(bGotRotation)
		{
			Vector3 rotAngles = vHeadRot.eulerAngles;
			
			if(bMirroredMovement)
			{
				rotAngles.x = -rotAngles.x;
				rotAngles.z = -rotAngles.z;
			}
			else
			{
				rotAngles.x = -rotAngles.x;
				rotAngles.y = -rotAngles.y;
			}
			
			return Quaternion.Euler(rotAngles);
		}

		return Quaternion.identity;
	}

	/// <summary>
	/// Gets the tracked face rectangle of the specified user in color image coordinates, or zero-rect if the user's face is not tracked.
	/// </summary>
	/// <returns>The face rectangle, in color image coordinates.</returns>
	/// <param name="userId">User ID</param>
	public Rect GetFaceColorRect(long userId)
	{
		Rect faceRect = new Rect();
		sensorData.sensorInterface.GetFaceRect(userId, ref faceRect);

		return faceRect;
	}
	
	/// <summary>
	/// Determines whether there are valid anim units.
	/// </summary>
	/// <returns><c>true</c> if there are valid anim units; otherwise, <c>false</c>.</returns>
	public bool IsGotAU()
	{
		return bGotAU;
	}
	
	/// <summary>
	/// Gets the animation unit value at given index, or 0 if the index is invalid.
	/// </summary>
	/// <returns>The animation unit value.</returns>
	/// <param name="faceAnimKey">Face animation unit.</param>
	public float GetAnimUnit(KinectInterop.FaceShapeAnimations faceAnimKey)
	{
		if(dictAU.ContainsKey(faceAnimKey))
		{
			return dictAU[faceAnimKey];
		}
		
		return 0.0f;
	}
	
	/// <summary>
	/// Gets all animation units for the specified user.
	/// </summary>
	/// <returns><c>true</c>, if the user's face is tracked, <c>false</c> otherwise.</returns>
	/// <param name="userId">User ID</param>
	/// <param name="dictAnimUnits">Animation units dictionary, to get the results.</param>
	public bool GetUserAnimUnits(long userId, ref Dictionary<KinectInterop.FaceShapeAnimations, float> dictAnimUnits)
	{
		if(sensorData != null && sensorData.sensorInterface != null)
		{
			bool bGotIt = sensorData.sensorInterface.GetAnimUnits(userId, ref dictAnimUnits);
			return bGotIt;
		}

		return false;
	}
	
	/// <summary>
	/// Determines whether there are valid shape units.
	/// </summary>
	/// <returns><c>true</c> if there are valid shape units; otherwise, <c>false</c>.</returns>
	public bool IsGotSU()
	{
		return bGotSU;
	}
	
	/// <summary>
	/// Gets the shape unit value at given index, or 0 if the index is invalid.
	/// </summary>
	/// <returns>The shape unit value.</returns>
	/// <param name="faceShapeKey">Face shape unit.</param>
	public float GetShapeUnit(KinectInterop.FaceShapeDeformations faceShapeKey)
	{
		if(dictSU.ContainsKey(faceShapeKey))
		{
			return dictSU[faceShapeKey];
		}
		
		return 0.0f;
	}
	
	/// <summary>
	/// Gets all animation units for the specified user.
	/// </summary>
	/// <returns><c>true</c>, if the user's face is tracked, <c>false</c> otherwise.</returns>
	/// <param name="userId">User ID</param>
	/// <param name="dictShapeUnits">Shape units dictionary, to get the results.</param>
	public bool GetUserShapeUnits(long userId, ref Dictionary<KinectInterop.FaceShapeDeformations, float> dictShapeUnits)
	{
		if(sensorData != null && sensorData.sensorInterface != null)
		{
			bool bGotIt = sensorData.sensorInterface.GetShapeUnits(userId, ref dictShapeUnits);
			return bGotIt;
		}
		
		return false;
	}
	
	/// <summary>
	/// Gets the count of face model vertices.
	/// </summary>
	/// <returns>The count of face model vertices.</returns>
	public int GetFaceModelVertexCount()
	{
		if (bGotModelVertices) 
		{
			return avModelVertices.Length;
		} 

		return 0;
	}

	/// <summary>
	/// Gets the face model vertex, if a face model is available and the index is in range; Vector3.zero otherwise.
	/// </summary>
	/// <returns>The face model vertex.</returns>
	/// <param name="index">Vertex index, or Vector3.zero</param>
	public Vector3 GetFaceModelVertex(int index)
	{
		if (bGotModelVertices) 
		{
			if(index >= 0 && index < avModelVertices.Length)
			{
				return avModelVertices[index];
			}
		}
		
		return Vector3.zero;
	}
	
	/// <summary>
	/// Gets all face model vertices, if a face model is available; null otherwise.
	/// </summary>
	/// <returns>The face model vertices, or null.</returns>
	public Vector3[] GetFaceModelVertices()
	{
		if (bGotModelVertices) 
		{
			return avModelVertices;
		}

		return null;
	}

	/// <summary>
	/// Gets all face model vertices for the specified user.
	/// </summary>
	/// <returns><c>true</c>, if the user's face is tracked, <c>false</c> otherwise.</returns>
	/// <param name="userId">User ID</param>
	/// <param name="avVertices">Reference to array of vertices, to get the result.</param>
	public bool GetUserFaceVertices(long userId, ref Vector3[] avVertices)
	{
		if(sensorData != null && sensorData.sensorInterface != null)
		{
			bool bGotIt = sensorData.sensorInterface.GetFaceModelVertices(userId, ref avVertices);
			return bGotIt;
		}
		
		return false;
	}
	
	/// <summary>
	/// Gets the face model triangle indices, if a face model is available; null otherwise.
	/// </summary>
	/// <returns>The face model triangle indices, or null.</returns>
	/// <param name="bMirroredModel">If set to <c>true</c> gets mirorred model indices.</param>
	public int[] GetFaceModelTriangleIndices(bool bMirroredModel)
	{
		if(sensorData != null && sensorData.sensorInterface != null)
		{
			int iNumTriangles = sensorData.sensorInterface.GetFaceModelTrianglesCount();

			if(iNumTriangles > 0)
			{
				int[] avModelTriangles = new int[iNumTriangles];
				bool bGotModelTriangles = sensorData.sensorInterface.GetFaceModelTriangles(bMirroredModel, ref avModelTriangles);

				if(bGotModelTriangles)
				{
					return avModelTriangles;
				}
			}
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
				throw new Exception("Face tracking cannot be started, because KinectManager is missing or not initialized.");
			}

			if(debugText != null)
			{
				debugText.GetComponent<GUIText>().text = "Please, wait...";
			}
			
			// ensure the needed dlls are in place and face tracking is available for this interface
			bool bNeedRestart = false;
			if(sensorData.sensorInterface.IsFaceTrackingAvailable(ref bNeedRestart))
			{
				if(bNeedRestart)
				{
					KinectInterop.RestartLevel(gameObject, "FM");
					return;
				}
			}
			else
			{
				string sInterfaceName = sensorData.sensorInterface.GetType().Name;
				throw new Exception(sInterfaceName + ": Face tracking is not supported!");
			}

			// Initialize the face tracker
			if (!sensorData.sensorInterface.InitFaceTracking(getFaceModelData, displayFaceRect))
	        {
	            throw new Exception("Face tracking could not be initialized.");
	        }
			
			instance = this;
			isFacetrackingInitialized = true;

			//DontDestroyOnLoad(gameObject);

			if(debugText != null)
			{
				debugText.GetComponent<GUIText>().text = "Ready.";
			}
		} 
		catch(DllNotFoundException ex)
		{
			Debug.LogError(ex.ToString());
			if(debugText != null)
				debugText.GetComponent<GUIText>().text = "Please check the Kinect and FT-Library installations.";
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
		if(isFacetrackingInitialized && sensorData != null && sensorData.sensorInterface != null)
		{
			// finish face tracking
			sensorData.sensorInterface.FinishFaceTracking();
		}

//		// clean up
//		Resources.UnloadUnusedAssets();
//		GC.Collect();
		
		isFacetrackingInitialized = false;
		instance = null;
	}
	
	void Update() 
	{
		if(isFacetrackingInitialized)
		{
			KinectManager kinectManager = KinectManager.Instance;
			if(kinectManager && kinectManager.IsInitialized())
			{
				primaryUserID = kinectManager.GetUserIdByIndex(playerIndex);
			}

			// update the face tracker
			if(sensorData.sensorInterface.UpdateFaceTracking())
			{
				// estimate the tracking state
				isTrackingFace = sensorData.sensorInterface.IsFaceTracked(primaryUserID);

				// get the facetracking parameters
				if(isTrackingFace)
				{
					lastFaceTrackedTime = Time.realtimeSinceStartup;
					
					// get face rectangle
					//bGotFaceRect = sensorData.sensorInterface.GetFaceRect(primaryUserID, ref faceRect);
					
					// get head position
					bGotHeadPos = sensorData.sensorInterface.GetHeadPosition(primaryUserID, ref headPos);

					// get head rotation
					bGotHeadRot = sensorData.sensorInterface.GetHeadRotation(primaryUserID, ref headRot);

					// get the animation units
					bGotAU = sensorData.sensorInterface.GetAnimUnits(primaryUserID, ref dictAU);

					// get the shape units
					bGotSU = sensorData.sensorInterface.GetShapeUnits(primaryUserID, ref dictSU);

					if(faceModelMesh != null && faceModelMesh.activeInHierarchy)
					{
						// apply model vertices to the mesh
						if(!bFaceModelMeshInited)
						{
							CreateFaceModelMesh();
						}
					}
					
					if(getFaceModelData)
					{
						UpdateFaceModelMesh();
					}
				}
				else if((Time.realtimeSinceStartup - lastFaceTrackedTime) <= faceTrackingTolerance)
				{
					// allow tolerance in tracking
					isTrackingFace = true;
				}

				if(faceModelMesh != null && bFaceModelMeshInited)
				{
					faceModelMesh.SetActive(isTrackingFace);
				}
			}
		}
	}
	
	void OnGUI()
	{
		if(isFacetrackingInitialized)
		{
			if(debugText != null)
			{
				if(isTrackingFace)
				{
					debugText.GetComponent<GUIText>().text = "Tracking - BodyID: " + primaryUserID;
				}
				else
				{
					debugText.GetComponent<GUIText>().text = "Not tracking...";
				}
			}
		}
	}
	
	private void CreateFaceModelMesh()
	{
		if(faceModelMesh == null)
			return;

		int iNumTriangles = sensorData.sensorInterface.GetFaceModelTrianglesCount();
		if(iNumTriangles <= 0)
			return;

		int[] avModelTriangles = new int[iNumTriangles];
		bool bGotModelTriangles = sensorData.sensorInterface.GetFaceModelTriangles(mirroredModelMesh, ref avModelTriangles);

		if(!bGotModelTriangles)
			return;
		
		int iNumVertices = sensorData.sensorInterface.GetFaceModelVerticesCount(0);
		if(iNumVertices < 0)
			return;

		avModelVertices = new Vector3[iNumVertices];
		bGotModelVertices = sensorData.sensorInterface.GetFaceModelVertices(0, ref avModelVertices);

		if(!bGotModelVertices)
			return;

		Vector2[] avModelUV = new Vector2[iNumVertices];

		//Quaternion faceModelRot = faceModelMesh.transform.rotation;
		//faceModelMesh.transform.rotation = Quaternion.identity;

		Mesh mesh = new Mesh();
		mesh.name = "FaceMesh";
		faceModelMesh.GetComponent<MeshFilter>().mesh = mesh;
		
		mesh.vertices = avModelVertices;
		mesh.uv = avModelUV;
		mesh.triangles = avModelTriangles;
		mesh.RecalculateNormals();

		//faceModelMesh.transform.rotation = faceModelRot;

		bFaceModelMeshInited = true;
	}

	private void UpdateFaceModelMesh()
	{
		// init the vertices array if needed
		if(avModelVertices == null)
		{
			int iNumVertices = sensorData.sensorInterface.GetFaceModelVerticesCount(primaryUserID);
			avModelVertices = new Vector3[iNumVertices];
		}

		// get face model vertices
		bGotModelVertices = sensorData.sensorInterface.GetFaceModelVertices(primaryUserID, ref avModelVertices);
		
		if(bGotModelVertices && faceModelMesh != null && bFaceModelMeshInited)
		{
			//Quaternion faceModelRot = faceModelMesh.transform.rotation;
			//faceModelMesh.transform.rotation = Quaternion.identity;
			
			Mesh mesh = faceModelMesh.GetComponent<MeshFilter>().mesh;
			mesh.vertices = avModelVertices;
			mesh.RecalculateNormals();
			mesh.RecalculateBounds();

			//faceModelMesh.transform.rotation = faceModelRot;
		}
	}
	
}
