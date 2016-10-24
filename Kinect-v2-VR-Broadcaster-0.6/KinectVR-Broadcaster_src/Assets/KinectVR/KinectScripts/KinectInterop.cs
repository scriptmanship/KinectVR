using UnityEngine;
//using Windows.Kinect;

using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System;
using System.IO;
using System.Text;
using ICSharpCode.SharpZipLib.Zip;
using OpenCvSharp;


/// <summary>
/// KinectInterop is a class that contains utility and interop functions and deals with different kinds of sensor interfaces.
/// </summary>
public class KinectInterop
{
	// order of depth sensor interfaces
	public static Type[] SensorInterfaceOrder = new Type[] { 
		typeof(Kinect2Interface), typeof(Kinect1Interface), typeof(OpenNI2Interface)
	};

	// graphics shader level
	private static int graphicsShaderLevel = 0;

	
	// constants
	public static class Constants
	{
		public const int MaxBodyCount = 6;
		public const int MaxJointCount = 25;

		public const float MinTimeBetweenSameGestures = 0.0f;
		public const float PoseCompleteDuration = 1.0f;
		public const float ClickMaxDistance = 0.05f;
		public const float ClickStayDuration = 2.0f;
	}

	// Types of depth sensor platforms
	public enum DepthSensorPlatform : int
	{
		None = 0,
		KinectSDKv1 = 1,
		KinectSDKv2 = 2,
		OpenNIv2 = 3
	}
	
	// Data structures for interfacing C# with the native wrappers

    [Flags]
    public enum FrameSource : uint
    {
		TypeNone = 0x0,
        TypeColor = 0x1,
        TypeInfrared = 0x2,
        TypeDepth = 0x8,
        TypeBodyIndex = 0x10,
        TypeBody = 0x20,
        TypeAudio = 0x40
    }
	
    public enum JointType : int
    {
		SpineBase = 0,
		SpineMid = 1,
        Neck = 2,
        Head = 3,
        ShoulderLeft = 4,
        ElbowLeft = 5,
        WristLeft = 6,
        HandLeft = 7,
        ShoulderRight = 8,
        ElbowRight = 9,
        WristRight = 10,
        HandRight = 11,
        HipLeft = 12,
        KneeLeft = 13,
        AnkleLeft = 14,
        FootLeft = 15,
        HipRight = 16,
        KneeRight = 17,
        AnkleRight = 18,
        FootRight = 19,
        SpineShoulder = 20,
        HandTipLeft = 21,
        ThumbLeft = 22,
        HandTipRight = 23,
        ThumbRight = 24
		//Count = 25
    }

    public static readonly Vector3[] JointBaseDir =
    {
        Vector3.zero,
        Vector3.up,
        Vector3.up,
        Vector3.up,
        Vector3.left,
        Vector3.left,
        Vector3.left,
        Vector3.left,
        Vector3.right,
        Vector3.right,
        Vector3.right,
        Vector3.right,
        Vector3.down,
        Vector3.down,
        Vector3.down,
        Vector3.forward,
        Vector3.down,
        Vector3.down,
        Vector3.down,
        Vector3.forward,
        Vector3.up,
        Vector3.left,
        Vector3.forward,
        Vector3.right,
        Vector3.forward
    };

    public enum TrackingState
    {
        NotTracked = 0,
        Inferred = 1,
        Tracked = 2
    }

	public enum HandState
    {
        Unknown = 0,
        NotTracked = 1,
        Open = 2,
        Closed = 3,
        Lasso = 4
    }
	
	public enum TrackingConfidence
    {
        Low = 0,
        High = 1
    }

//    [Flags]
//    public enum ClippedEdges
//    {
//        None = 0,
//        Right = 1,
//        Left = 2,
//        Top = 4,
//        Bottom = 8
//    }

	public enum FaceShapeAnimations : int
	{
		JawOpen                                  =0,
		LipPucker                                =1,
		JawSlideRight                            =2,
		LipStretcherRight                        =3,
		LipStretcherLeft                         =4,
		LipCornerPullerLeft                      =5,
		LipCornerPullerRight                     =6,
		LipCornerDepressorLeft                   =7,
		LipCornerDepressorRight                  =8,
		LeftcheekPuff                            =9,
		RightcheekPuff                           =10,
		LefteyeClosed                            =11,
		RighteyeClosed                           =12,
		RighteyebrowLowerer                      =13,
		LefteyebrowLowerer                       =14,
		LowerlipDepressorLeft                    =15,
		LowerlipDepressorRight                   =16,
	}
	
	public enum FaceShapeDeformations : int
	{
		PCA01                                    =0,
		PCA02                                    =1,
		PCA03                                    =2,
		PCA04                                    =3,
		PCA05                                    =4,
		PCA06                                    =5,
		PCA07                                    =6,
		PCA08                                    =7,
		PCA09                                    =8,
		PCA10                                    =9,
		Chin03                                   =10,
		Forehead00                               =11,
		Cheeks02                                 =12,
		Cheeks01                                 =13,
		MouthBag01                               =14,
		MouthBag02                               =15,
		Eyes02                                   =16,
		MouthBag03                               =17,
		Forehead04                               =18,
		Nose00                                   =19,
		Nose01                                   =20,
		Nose02                                   =21,
		MouthBag06                               =22,
		MouthBag05                               =23,
		Cheeks00                                 =24,
		Mask03                                   =25,
		Eyes03                                   =26,
		Nose03                                   =27,
		Eyes08                                   =28,
		MouthBag07                               =29,
		Eyes00                                   =30,
		Nose04                                   =31,
		Mask04                                   =32,
		Chin04                                   =33,
		Forehead05                               =34,
		Eyes06                                   =35,
		Eyes11                                   =36,
		Nose05                                   =37,
		Mouth07                                  =38,
		Cheeks08                                 =39,
		Eyes09                                   =40,
		Mask10                                   =41,
		Mouth09                                  =42,
		Nose07                                   =43,
		Nose08                                   =44,
		Cheeks07                                 =45,
		Mask07                                   =46,
		MouthBag09                               =47,
		Nose06                                   =48,
		Chin02                                   =49,
		Eyes07                                   =50,
		Cheeks10                                 =51,
		Rim20                                    =52,
		Mask22                                   =53,
		MouthBag15                               =54,
		Chin01                                   =55,
		Cheeks04                                 =56,
		Eyes17                                   =57,
		Cheeks13                                 =58,
		Mouth02                                  =59,
		MouthBag12                               =60,
		Mask19                                   =61,
		Mask20                                   =62,
		Forehead06                               =63,
		Mouth13                                  =64,
		Mask25                                   =65,
		Chin05                                   =66,
		Cheeks20                                 =67,
		Nose09                                   =68,
		Nose10                                   =69,
		MouthBag27                               =70,
		Mouth11                                  =71,
		Cheeks14                                 =72,
		Eyes16                                   =73,
		Mask29                                   =74,
		Nose15                                   =75,
		Cheeks11                                 =76,
		Mouth16                                  =77,
		Eyes19                                   =78,
		Mouth17                                  =79,
		MouthBag36                               =80,
		Mouth15                                  =81,
		Cheeks25                                 =82,
		Cheeks16                                 =83,
		Cheeks18                                 =84,
		Rim07                                    =85,
		Nose13                                   =86,
		Mouth18                                  =87,
		Cheeks19                                 =88,
		Rim21                                    =89,
		Mouth22                                  =90,
		Nose18                                   =91,
		Nose16                                   =92,
		Rim22                                    =93,
	}
	

	public class SensorData
	{
		public DepthSensorInterface sensorInterface;
		public DepthSensorPlatform sensorIntPlatform;

		public int bodyCount;
		public int jointCount;

		public float depthCameraOffset;
		public float depthCameraFOV;
		public float colorCameraFOV;
		public float faceOverlayOffset;

		public int colorImageWidth;
		public int colorImageHeight;

		public byte[] colorImage;
		public long lastColorFrameTime = 0;

		public int depthImageWidth;
		public int depthImageHeight;

		public ushort[] depthImage;
		public long lastDepthFrameTime = 0;

		public ushort[] infraredImage;
		public long lastInfraredFrameTime = 0;

		public byte[] bodyIndexImage;
		public long lastBodyIndexFrameTime = 0;

		public byte selectedBodyIndex = 255;

		public bool hintHeightAngle = false;
		public Quaternion sensorRotDetected = Quaternion.identity;
		public float sensorHgtDetected = 0f;
		
		public RenderTexture bodyIndexTexture;
		public Material bodyIndexMaterial;
		public ComputeBuffer bodyIndexBuffer;

		public float[] bodyIndexBufferData = null;
		public bool bodyIndexBufferReady = false;
		public object bodyIndexBufferLock = new object();

		public RenderTexture depthImageTexture;
		public Material depthImageMaterial;
		public ComputeBuffer depthImageBuffer;
		public ComputeBuffer depthHistBuffer;

		public float[] depthImageBufferData = null;
		public int[] depthHistBufferData = null;
		public float[] equalHistBufferData = null;
		public int depthHistTotalPoints = 0;
		public int firstUserIndex = -1;

		public bool depthImageBufferReady = false;
		public object depthImageBufferLock = new object();
		public bool depthCoordsBufferReady = false;
		public object depthCoordsBufferLock = new object();
		public bool newDepthImage = false;
		
		public Texture2D colorImageTexture;

		public bool colorImageBufferReady = false;
		public object colorImageBufferLock = new object();
		public bool newColorImage = false;

		public RenderTexture depth2ColorTexture;
		public Material depth2ColorMaterial;
		public ComputeBuffer depth2ColorBuffer;
		public Vector2[] depth2ColorCoords;

		public RenderTexture color2DepthTexture;
		public Material color2DepthMaterial;
		public ComputeBuffer color2DepthBuffer;
		public Vector2[] color2DepthCoords;
		
		public RenderTexture alphaBodyTexture;
		public Material erodeBodyMaterial, dilateBodyMaterial, blurBodyMaterial;

		public RenderTexture colorBackgroundTexture;
		public Material colorBackgroundMaterial;

		public bool newInfraredImage = false;

		public bool bodyFrameReady = false;
		public object bodyFrameLock = new object();
		public bool newBodyFrame = false;
		
		public bool isPlayModeEnabled;
		public string playModeData;
	}

	public struct SmoothParameters
	{
		public float smoothing;
		public float correction;
		public float prediction;
		public float jitterRadius;
		public float maxDeviationRadius;
	}
	
	public struct JointData
    {
		// parameters filled in by the sensor interface
		//public JointType jointType;
    	public TrackingState trackingState;
    	public Vector3 kinectPos;
    	public Vector3 position;
		public Quaternion orientation;  // deprecated

		public Vector3 posPrev;
		public Vector3 posRel;
		public Vector3 posDrv;

		// KM calculated parameters
		public Vector3 direction;
		public Quaternion normalRotation;
		public Quaternion mirroredRotation;
		
		// Constraint parameters
		public float lastAngle;
    }
	
	public struct BodyData
    {
		// parameters filled in by the sensor interface
        public Int64 liTrackingID;
        public Vector3 position;
		public Quaternion orientation;  // deprecated

		public JointData[] joint;

		// KM calculated parameters
		public Quaternion normalRotation;
		public Quaternion mirroredRotation;
		
		public Vector3 hipsDirection;
		public Vector3 shouldersDirection;
		public float bodyTurnAngle;
		//public float bodyFullAngle;
		public bool isTurnedAround;
		public float turnAroundFactor;

		public Quaternion leftHandOrientation;
		public Quaternion rightHandOrientation;

		public Quaternion headOrientation;

//		public Vector3 leftArmDirection;
//		public Vector3 leftThumbForward;
//		public Vector3 leftThumbDirection;
//		//public float leftThumbAngle;
//
//		public Vector3 rightArmDirection;
//		public Vector3 rightThumbForward;
//		public Vector3 rightThumbDirection;
//		//public float rightThumbAngle;

		//public Vector3 leftLegDirection;
		//public Vector3 leftFootDirection;
		//public Vector3 rightLegDirection;
		//public Vector3 rightFootDirection;

		public HandState leftHandState;
		public TrackingConfidence leftHandConfidence;
		public HandState rightHandState;
		public TrackingConfidence rightHandConfidence;
		
        public uint dwClippedEdges;
        public short bIsTracked;
		public short bIsRestricted;
    }
	
    public struct BodyFrameData
    {
        public Int64 liRelativeTime, liPreviousTime;
        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 6, ArraySubType = UnmanagedType.Struct)]
        public BodyData[] bodyData;
        //public UnityEngine.Vector4 floorClipPlane;
		public bool bTurnAnalisys;
		
		public BodyFrameData(int bodyCount, int jointCount)
		{
			liRelativeTime = liPreviousTime = 0;
			//floorClipPlane = UnityEngine.Vector4.zero;
			bTurnAnalisys = false;

			bodyData = new BodyData[bodyCount];

			for(int i = 0; i < bodyCount; i++)
			{
				bodyData[i].joint = new JointData[jointCount];

				bodyData[i].leftHandOrientation = Quaternion.identity;
				bodyData[i].rightHandOrientation = Quaternion.identity;
				bodyData[i].headOrientation = Quaternion.identity;
			}
		}
    }
	

	// initializes the available sensor interfaces
	public static List<DepthSensorInterface> InitSensorInterfaces(bool bOnceRestarted, ref bool bNeedRestart)
	{
		List<DepthSensorInterface> listInterfaces = new List<DepthSensorInterface>();

		var typeInterface = typeof(DepthSensorInterface);
		//Type[] typesAvailable = typeInterface.Assembly.GetTypes();

		for(int pass = 0; pass <= 1; pass++)
		{
			bool bCopyLibs = (pass != 0);

			foreach(Type type in SensorInterfaceOrder)
			{
				if(typeInterface.IsAssignableFrom(type) && type != typeInterface)
				{
					DepthSensorInterface sensorInt = null;
					
					try 
					{
						sensorInt = (DepthSensorInterface)Activator.CreateInstance(type);
						
						bool bIntNeedRestart = false;
						if(sensorInt.InitSensorInterface(bCopyLibs, ref bIntNeedRestart))
						{
							bNeedRestart |= bIntNeedRestart;
						}
						else
						{
							sensorInt.FreeSensorInterface(bCopyLibs);
							sensorInt = null;

							continue;
						}
						
						if(!bNeedRestart && !sensorInt.IsSensorAvailable())
						{
							sensorInt.FreeSensorInterface(false);
							sensorInt = null;
						}
					}
					catch (Exception ex) 
					{
						//Debug.Log(ex);
						
						if(sensorInt != null)
						{
							try 
							{
								sensorInt.FreeSensorInterface(bCopyLibs);
							}
							catch (Exception) 
							{
								// do nothing
							}
							finally
							{
								sensorInt = null;
							}
						}
					}
					
					if(sensorInt != null)
					{
						listInterfaces.Add(sensorInt);
					}
				}
			}

			if(listInterfaces.Count > 0)
			{
				// we found working interface(s), don't go any further
				break;
			}

			if(bOnceRestarted)
			{
				// we have restarted once, don't do it again
				break;
			}
		}

		return listInterfaces;
	}

	// opens the default sensor and needed readers
	public static SensorData OpenDefaultSensor(List<DepthSensorInterface> listInterfaces, FrameSource dwFlags, 
		float sensorAngle, bool bUseMultiSource, KinectManager.UserMapType userMapType)
	{
		SensorData sensorData = null;
		if(listInterfaces == null)
			return sensorData;

		foreach(DepthSensorInterface sensorInt in listInterfaces)
		{
			try 
			{
				if(sensorData == null)
				{
					sensorData = sensorInt.OpenDefaultSensor(dwFlags, sensorAngle, bUseMultiSource);

					if(sensorData != null)
					{
						sensorData.sensorInterface = sensorInt;
						sensorData.sensorIntPlatform = sensorInt.GetSensorPlatform();
						Debug.Log("Interface used: " + sensorInt.GetType().Name);

						Debug.Log("Shader level: " + SystemInfo.graphicsShaderLevel);
						if(sensorData.bodyIndexImage != null && IsDirectX11Available())
						{
							Shader bodyIndexShader = Shader.Find("Kinect/BodyShader");

							if(bodyIndexShader != null)
							{
								sensorData.bodyIndexTexture = new RenderTexture(sensorData.depthImageWidth, sensorData.depthImageHeight, 0);
								sensorData.bodyIndexTexture.wrapMode = TextureWrapMode.Clamp;
								sensorData.bodyIndexTexture.filterMode = FilterMode.Point;
								//Debug.Log(sensorData.bodyIndexTexture.format);
								
								sensorData.bodyIndexMaterial = new Material(bodyIndexShader);
								
								sensorData.bodyIndexMaterial.SetFloat("_TexResX", (float)sensorData.depthImageWidth);
								sensorData.bodyIndexMaterial.SetFloat("_TexResY", (float)sensorData.depthImageHeight);
								
								sensorData.bodyIndexBuffer = new ComputeBuffer(sensorData.bodyIndexImage.Length, sizeof(float));
								sensorData.bodyIndexMaterial.SetBuffer("_BodyIndexBuffer", sensorData.bodyIndexBuffer);
							}
						}
						
						if(sensorData.depthImage != null && IsDirectX11Available())
						{
							Shader depthImageShader = Shader.Find("Kinect/DepthShader");

							if(depthImageShader != null)
							{
								sensorData.depthImageTexture = new RenderTexture(sensorData.depthImageWidth, sensorData.depthImageHeight, 0);
								sensorData.depthImageTexture.wrapMode = TextureWrapMode.Clamp;
								sensorData.depthImageTexture.filterMode = FilterMode.Point;
								
								sensorData.depthImageMaterial = new Material(depthImageShader);
								
								sensorData.depthImageMaterial.SetTexture("_MainTex", sensorData.bodyIndexTexture);
								
								sensorData.depthImageMaterial.SetFloat("_TexResX", (float)sensorData.depthImageWidth);
								sensorData.depthImageMaterial.SetFloat("_TexResY", (float)sensorData.depthImageHeight);
								
								sensorData.depthImageBuffer = new ComputeBuffer(sensorData.depthImage.Length, sizeof(float));
								sensorData.depthImageMaterial.SetBuffer("_DepthBuffer", sensorData.depthImageBuffer);
								
								sensorData.depthHistBuffer = new ComputeBuffer(5001, sizeof(float));
								sensorData.depthImageMaterial.SetBuffer("_HistBuffer", sensorData.depthHistBuffer);

								// use body index buffer to overcome the linear color correction
								sensorData.depthImageMaterial.SetBuffer("_BodyIndexBuffer", sensorData.bodyIndexBuffer);
							}
						}

						if(sensorData.colorImage != null)
						{
							sensorData.colorImageTexture = new Texture2D(sensorData.colorImageWidth, sensorData.colorImageHeight, TextureFormat.RGBA32, false);
						}
						
						if(sensorData.bodyIndexImage != null && sensorData.colorImage != null && IsDirectX11Available() &&
							userMapType == KinectManager.UserMapType.CutOutTexture)
						{
							Shader depth2ColorShader = Shader.Find("Kinect/Depth2ColorShader");

							if(depth2ColorShader)
							{
								sensorData.depth2ColorTexture = new RenderTexture(sensorData.depthImageWidth, sensorData.depthImageHeight, 0);
								sensorData.depth2ColorTexture.wrapMode = TextureWrapMode.Clamp;
								//sensorData.depth2ColorTexture.filterMode = FilterMode.Point;
								
								sensorData.depth2ColorMaterial = new Material(depth2ColorShader);
								
								sensorData.depth2ColorMaterial.SetFloat("_ColorResX", (float)sensorData.colorImageWidth);
								sensorData.depth2ColorMaterial.SetFloat("_ColorResY", (float)sensorData.colorImageHeight);
								sensorData.depth2ColorMaterial.SetFloat("_DepthResX", (float)sensorData.depthImageWidth);
								sensorData.depth2ColorMaterial.SetFloat("_DepthResY", (float)sensorData.depthImageHeight);
								
								sensorData.depth2ColorBuffer = new ComputeBuffer(sensorData.depthImage.Length, sizeof(float) * 2);
								sensorData.depth2ColorMaterial.SetBuffer("_ColorCoords", sensorData.depth2ColorBuffer);
							}
						}

						if(sensorData.bodyIndexImage != null && sensorData.colorImage != null && IsDirectX11Available() &&
							userMapType == KinectManager.UserMapType.CutOutTexture)
						{
							sensorData.depth2ColorCoords = new Vector2[sensorData.depthImage.Length];
						}
						
					}
				}
				else
				{
					sensorInt.FreeSensorInterface(false);
				}
			}
			catch (Exception ex) 
			{
				Debug.LogError("Initialization of the sensor failed.");
				Debug.LogError(ex.ToString());

				try 
				{
					sensorInt.FreeSensorInterface(false);
				} 
				catch (Exception) 
				{
					// do nothing
				}
			}
		}

		return sensorData;
	}

	// closes opened readers and closes the sensor
	public static void CloseSensor(SensorData sensorData)
	{
		FinishBackgroundRemoval(sensorData);
		//FinishColorBackground(sensorData);

		if(sensorData != null && sensorData.sensorInterface != null)
		{
			sensorData.sensorInterface.CloseSensor(sensorData);
		}
		
		if(sensorData.bodyIndexBuffer != null)
		{
			sensorData.bodyIndexBuffer.Release();
			sensorData.bodyIndexBuffer = null;
		}
		
		if(sensorData.depthImageBuffer != null)
		{
			sensorData.depthImageBuffer.Release();
			sensorData.depthImageBuffer = null;
		}

		if(sensorData.depthHistBuffer != null)
		{
			sensorData.depthHistBuffer.Release();
			sensorData.depthHistBuffer = null;
		}
		
		if(sensorData.depth2ColorBuffer != null)
		{
			sensorData.depth2ColorBuffer.Release();
			sensorData.depth2ColorBuffer = null;
		}
	}

	// invoked periodically to update sensor data, if needed
	public static bool UpdateSensorData(SensorData sensorData)
	{
		bool bResult = false;

		if(sensorData.sensorInterface != null)
		{
			bResult = sensorData.sensorInterface.UpdateSensorData(sensorData);
		}

		return bResult;
	}
	
	// returns the mirror joint of the given joint
	public static JointType GetMirrorJoint(JointType joint)
	{
		switch(joint)
		{
			case JointType.ShoulderLeft:
				return JointType.ShoulderRight;
	        case JointType.ElbowLeft:
				return JointType.ElbowRight;
	        case JointType.WristLeft:
				return JointType.WristRight;
	        case JointType.HandLeft:
				return JointType.HandRight;
					
	        case JointType.ShoulderRight:
				return JointType.ShoulderLeft;
	        case JointType.ElbowRight:
				return JointType.ElbowLeft;
	        case JointType.WristRight:
				return JointType.WristLeft;
	        case JointType.HandRight:
				return JointType.HandLeft;
					
	        case JointType.HipLeft:
				return JointType.HipRight;
	        case JointType.KneeLeft:
				return JointType.KneeRight;
	        case JointType.AnkleLeft:
				return JointType.AnkleRight;
	        case JointType.FootLeft:
				return JointType.FootRight;
					
	        case JointType.HipRight:
				return JointType.HipLeft;
	        case JointType.KneeRight:
				return JointType.KneeLeft;
	        case JointType.AnkleRight:
				return JointType.AnkleLeft;
	        case JointType.FootRight:
				return JointType.FootLeft;
					
	        case JointType.HandTipLeft:
				return JointType.HandTipRight;
	        case JointType.ThumbLeft:
				return JointType.ThumbRight;
			
	        case JointType.HandTipRight:
				return JointType.HandTipLeft;
	        case JointType.ThumbRight:
				return JointType.ThumbLeft;
		}
	
		return joint;
	}

	// gets new multi source frame
	public static bool GetMultiSourceFrame(SensorData sensorData)
	{
		bool bResult = false;

		if(sensorData.sensorInterface != null)
		{
			bResult = sensorData.sensorInterface.GetMultiSourceFrame(sensorData);
		}

		return bResult;
	}

	// frees last multi source frame
	public static void FreeMultiSourceFrame(SensorData sensorData)
	{
		if(sensorData.sensorInterface != null)
		{
			sensorData.sensorInterface.FreeMultiSourceFrame(sensorData);
		}
	}

	// converts current body frame to a single csv line. returns empty string if there is no new data
	public static string GetBodyFrameAsCsv(SensorData sensorData, ref BodyFrameData bodyFrame, ref long liRelTime, ref float fUnityTime)
	{
		// check for invalid sensor data and if the frame is still the same
		if(sensorData == null)
			return string.Empty;
		if(bodyFrame.liRelativeTime == liRelTime)
			return string.Empty;

		// create the output string
		StringBuilder sbBuf = new StringBuilder();
		const char delimiter = ',';

		sbBuf.Append("kb").Append(delimiter);
		sbBuf.Append(bodyFrame.liRelativeTime).Append(delimiter);

		liRelTime = bodyFrame.liRelativeTime;
		fUnityTime = Time.time;

		sbBuf.Append(sensorData.bodyCount).Append(delimiter);
		sbBuf.Append(sensorData.jointCount).Append(delimiter);

		// add information for all bodies
		for(int i = 0; i < sensorData.bodyCount; i++)
		{
			sbBuf.Append(bodyFrame.bodyData[i].bIsTracked).Append(delimiter);

			if(bodyFrame.bodyData[i].bIsTracked != 0)
			{
				// add information for the tracked body - body-id and joints
				sbBuf.Append(bodyFrame.bodyData[i].liTrackingID).Append(delimiter);

				for(int j = 0; j < sensorData.jointCount; j++)
				{
					KinectInterop.JointData jointData = bodyFrame.bodyData[i].joint[j];

					sbBuf.Append((int)jointData.trackingState).Append(delimiter);

					if(jointData.trackingState != TrackingState.NotTracked)
					{
						sbBuf.AppendFormat("{0:F3}", jointData.kinectPos.x).Append(delimiter);
						sbBuf.AppendFormat("{0:F3}", jointData.kinectPos.y).Append(delimiter);
						sbBuf.AppendFormat("{0:F3}", jointData.kinectPos.z).Append(delimiter);
					}
				}
			}
		}

		// remove the last delimiter
		if(sbBuf.Length > 0 && sbBuf[sbBuf.Length - 1] == delimiter)
		{
			sbBuf.Remove(sbBuf.Length - 1, 1);
		}

		return sbBuf.ToString();
	}

	// sets current body frame from the given csv line. returns true on success, false otherwise
	public static bool SetBodyFrameFromCsv(string sCsvLine, SensorData sensorData, ref BodyFrameData bodyFrame, ref Matrix4x4 kinectToWorld)
	{
		// check for invalid sensor data and for same frame time
		if(sensorData == null)
			return false;
		if(sCsvLine.Length == 0)
			return false;

		// split the csv line in parts
		char[] delimiters = { ',' };
		string[] alCsvParts = sCsvLine.Split(delimiters);

		if(alCsvParts.Length < 4)
			return false;

		// wait for buffer release
		while(sensorData.bodyFrameReady)
		{
			System.Threading.Thread.Sleep(1);
		}
		
		// check the id, body count & joint count
		int bodyCount = 0, jointCount = 0;
		int.TryParse(alCsvParts[2], out bodyCount);
		int.TryParse(alCsvParts[3], out jointCount);

		long liRelTime = 0;
		long.TryParse(alCsvParts[1], out liRelTime);

		if(alCsvParts[0] != "kb" || bodyCount == 0 || jointCount == 0 || liRelTime == 0)
			return false;
		if(bodyCount != sensorData.bodyCount || jointCount != sensorData.jointCount)
			return false;

		// update body frame data
		bodyFrame.liPreviousTime = bodyFrame.liRelativeTime;
		bodyFrame.liRelativeTime = liRelTime;

		int iIndex = 4;
		for(int i = 0; i < sensorData.bodyCount; i++)
		{
			if(alCsvParts.Length < (iIndex + 1))
				return false;

			// update the tracked-flag and body id
			short bIsTracked = 0;
			long liTrackingID = 0;

			short.TryParse(alCsvParts[iIndex], out bIsTracked);
			iIndex++;

			if(bIsTracked != 0 && alCsvParts.Length >= (iIndex + 1))
			{
				long.TryParse(alCsvParts[iIndex], out liTrackingID);
				iIndex++;

				if(liTrackingID == 0)
				{
					bIsTracked = 0;
				}
			}

			bodyFrame.bodyData[i].bIsTracked = bIsTracked;
			bodyFrame.bodyData[i].liTrackingID = liTrackingID;

			if(bIsTracked != 0)
			{
				// update joints' data
				for(int j = 0; j < sensorData.jointCount; j++)
				{
					KinectInterop.JointData jointData = bodyFrame.bodyData[i].joint[j];
					int iTrackingState = 0;

					if(alCsvParts.Length >= (iIndex + 1))
					{
						int.TryParse(alCsvParts[iIndex], out iTrackingState);
						iIndex++;

						jointData.trackingState = (KinectInterop.TrackingState)iTrackingState;

						if(iTrackingState != (int)TrackingState.NotTracked && alCsvParts.Length >= (iIndex + 3))
						{
							float x = 0f, y = 0f, z = 0f;

							float.TryParse(alCsvParts[iIndex], out x);
							float.TryParse(alCsvParts[iIndex + 1], out y);
							float.TryParse(alCsvParts[iIndex + 2], out z);
							iIndex += 3;
						
							jointData.kinectPos = new Vector3(x, y, z);
						}
						else
						{
							jointData.kinectPos = Vector3.zero;
						}

						jointData.position = kinectToWorld.MultiplyPoint3x4(jointData.kinectPos);
						jointData.orientation = Quaternion.identity;

						if(j == 0)
						{
							// set body position
							bodyFrame.bodyData[i].position = jointData.position;
							bodyFrame.bodyData[i].orientation = jointData.orientation;
						}
					}

					bodyFrame.bodyData[i].joint[j] = jointData;
				}
			}
		}

		// calculate bone directions
		CalcBodyFrameBoneDirs(sensorData, ref bodyFrame);

		// frame is ready
		lock(sensorData.bodyFrameLock)
		{
			sensorData.bodyFrameReady = true;
		}

		return true;
	}

	// Polls for new skeleton data
	public static bool PollBodyFrame(SensorData sensorData, ref BodyFrameData bodyFrame, ref Matrix4x4 kinectToWorld, bool bIgnoreJointZ)
	{
		bool bNewFrame = false;

		if(sensorData.sensorInterface != null)
		{
			// wait for buffer release
			while(sensorData.bodyFrameReady)
			{
				System.Threading.Thread.Sleep(1);
			}
			
			bNewFrame = sensorData.sensorInterface.PollBodyFrame(sensorData, ref bodyFrame, ref kinectToWorld, bIgnoreJointZ);

			if(bNewFrame)
			{
				CalcBodyFrameBoneDirs(sensorData, ref bodyFrame);
				
				// frame is ready
				lock(sensorData.bodyFrameLock)
				{
					sensorData.bodyFrameReady = true;
				}
			}
		}
		
		return bNewFrame;
	}

	// Calculates all valid bone directions in a body frame
	private static void CalcBodyFrameBoneDirs(SensorData sensorData, ref BodyFrameData bodyFrame)
	{
		for(int i = 0; i < sensorData.bodyCount; i++)
		{
			if(bodyFrame.bodyData[i].bIsTracked != 0)
			{
				for(int j = 0; j < sensorData.jointCount; j++)
				{
					if(j == 0)
					{
						bodyFrame.bodyData[i].joint[j].direction = Vector3.zero;
					}
					else
					{
						int jParent = (int)sensorData.sensorInterface.GetParentJoint((KinectInterop.JointType)j);
						
						if(bodyFrame.bodyData[i].joint[j].trackingState != TrackingState.NotTracked && 
						   bodyFrame.bodyData[i].joint[jParent].trackingState != TrackingState.NotTracked)
						{
							bodyFrame.bodyData[i].joint[j].direction = 
								bodyFrame.bodyData[i].joint[j].position - bodyFrame.bodyData[i].joint[jParent].position;
						}
					}
				}
			}
		}

	}
	
	// Recalculates bone directions for the given body
	public static void RecalcBoneDirs(SensorData sensorData, ref BodyData bodyData)
	{
		for(int j = 0; j < bodyData.joint.Length; j++)
		{
			if(j == 0)
			{
				bodyData.joint[j].direction = Vector3.zero;
			}
			else
			{
				int jParent = (int)sensorData.sensorInterface.GetParentJoint((KinectInterop.JointType)j);
				
				if(bodyData.joint[j].trackingState != TrackingState.NotTracked && 
				   bodyData.joint[jParent].trackingState != TrackingState.NotTracked)
				{
					bodyData.joint[j].direction = bodyData.joint[j].position - bodyData.joint[jParent].position;
				}
			}
		}
	}
	
	// Polls for new color frame data
	public static bool PollColorFrame(SensorData sensorData)
	{
		bool bNewFrame = false;

		if(sensorData.sensorInterface != null && !sensorData.isPlayModeEnabled)
		{
			// wait for buffer release
			while(sensorData.colorImageBufferReady)
			{
				System.Threading.Thread.Sleep(1);
			}
			
			bNewFrame = sensorData.sensorInterface.PollColorFrame(sensorData);

			if(bNewFrame)
			{
				// buffer is ready
				lock(sensorData.colorImageBufferLock)
				{
					sensorData.colorImageBufferReady = true;
				}
			}
		}

		return bNewFrame;
	}

	// Renders color texture
	public static void RenderColorTexture(SensorData sensorData)
	{
		if(sensorData.colorImageBufferReady && sensorData.colorImageTexture)
		{
			sensorData.colorImageTexture.LoadRawTextureData(sensorData.colorImage);
			sensorData.colorImageTexture.Apply();
		
			// buffer is released
			lock(sensorData.colorImageBufferLock)
			{
				sensorData.colorImageBufferReady = false;
			}
		}
	}
	
	// Polls for new depth frame data
	public static bool PollDepthFrame(SensorData sensorData, KinectManager.UserMapType userMapType,
	                                  bool bLimitedUsers, ICollection<int> alTrackedIndexes)
	{
		bool bNewFrame = false;

		if(sensorData.sensorInterface != null && !sensorData.isPlayModeEnabled)
		{
			// wait for buffer releases
			while(sensorData.bodyIndexBufferReady || sensorData.depthImageBufferReady)
			{
				System.Threading.Thread.Sleep(1);
			}
			
			bNewFrame = sensorData.sensorInterface.PollDepthFrame(sensorData);

			if(bNewFrame)
			{
				if(userMapType != KinectManager.UserMapType.RawUserDepth && sensorData.bodyIndexBuffer != null)
				{
					byte btSelBI = sensorData.selectedBodyIndex;
					int iBodyIndexLength = sensorData.bodyIndexImage.Length;

					// convert the body indices to string
					string sTrackedIndices = string.Empty;
					foreach(int bodyIndex in alTrackedIndexes)
					{
						sTrackedIndices += (char)(0x30 + bodyIndex);
					}

					// create body index texture
					if(sensorData.bodyIndexBufferData == null)
					{
						sensorData.bodyIndexBufferData = new float[iBodyIndexLength];
					}

					for (int i = 0; i < iBodyIndexLength; i++)
					{
						byte btBufBI = sensorData.bodyIndexImage[i];

						bool bUserTracked = btSelBI != 255 ? btSelBI == btBufBI : 
								//(bLimitedUsers ? alTrackedIndexes.Contains((int)btBufBI) : btBufBI != 255);
								(bLimitedUsers ? sTrackedIndices.IndexOf((char)(0x30 + btBufBI)) >= 0 : btBufBI != 255);

						if(bUserTracked)
						{
							sensorData.bodyIndexBufferData[i] = (float)btBufBI;
						}
						else
						{
							sensorData.bodyIndexBufferData[i] = 255f;
						}
					}

					// buffer is ready
					lock(sensorData.bodyIndexBufferLock)
					{
						sensorData.bodyIndexBufferReady = true;
					}
				}
				
				if(sensorData.depthImageBuffer != null && sensorData.depthHistBuffer != null &&
				   userMapType == KinectManager.UserMapType.UserTexture)
				{
					// create depth texture
					if(sensorData.depthImageBufferData == null)
					{
						sensorData.depthImageBufferData = new float[sensorData.depthImage.Length];
						sensorData.depthHistBufferData = new int[5001];
						sensorData.equalHistBufferData = new float[sensorData.depthHistBufferData.Length];
					}

					Array.Clear(sensorData.depthHistBufferData, 0, sensorData.depthHistBufferData.Length);
					Array.Clear(sensorData.equalHistBufferData, 0, sensorData.equalHistBufferData.Length);
					sensorData.depthHistTotalPoints = 0;

					for (int i = 0; i < sensorData.depthImage.Length; i++)
					{
						int depth = sensorData.depthImage[i] < 5000 ? (int)sensorData.depthImage[i] : 5000;
						sensorData.depthImageBufferData[i] = (float)depth;

						if(sensorData.bodyIndexImage[i] != 255)
						{
							sensorData.depthHistBufferData[depth]++;
							sensorData.depthHistTotalPoints++;
						}
					}

					sensorData.equalHistBufferData[0] = (float)sensorData.depthHistBufferData[0];
					for(int i = 1; i < sensorData.depthHistBufferData.Length; i++)
					{
						sensorData.equalHistBufferData[i] = sensorData.equalHistBufferData[i - 1] + (float)sensorData.depthHistBufferData[i];
					}

					// buffer is ready
					lock(sensorData.depthImageBufferLock)
					{
						sensorData.depthImageBufferReady = true;
					}
				}

				if(sensorData.color2DepthCoords != null)
				{
					// wait for buffer release
					while(sensorData.depthCoordsBufferReady)
					{
						System.Threading.Thread.Sleep(1);
					}
					
					if(!MapColorFrameToDepthCoords(sensorData, ref sensorData.color2DepthCoords))
					{
						sensorData.color2DepthCoords = null;
					}

					// buffer is ready
					lock(sensorData.depthCoordsBufferLock)
					{
						sensorData.depthCoordsBufferReady = (sensorData.color2DepthCoords != null);
					}
				}
				else if(sensorData.depth2ColorCoords != null && (userMapType == KinectManager.UserMapType.CutOutTexture || 
					    (sensorData.sensorInterface.IsBackgroundRemovalActive() && 
						 sensorData.sensorInterface.GetSensorPlatform() != KinectInterop.DepthSensorPlatform.KinectSDKv1)))
				{
					// wait for buffer release
					while(sensorData.depthCoordsBufferReady)
					{
						System.Threading.Thread.Sleep(1);
					}
					
					if(!MapDepthFrameToColorCoords(sensorData, ref sensorData.depth2ColorCoords))
					{
						sensorData.depth2ColorCoords = null;
					}
					
					// buffer is ready
					lock(sensorData.depthCoordsBufferLock)
					{
						sensorData.depthCoordsBufferReady = (sensorData.depth2ColorCoords != null);
					}
				}

			}
		}
		
		return bNewFrame;
	}

	// Renders body-index texture
	public static void RenderBodyIndexTexture(SensorData sensorData, KinectManager.UserMapType userMapType)
	{
		// check if buffer is ready
		if(sensorData.bodyIndexBufferReady)
		{
			sensorData.bodyIndexBuffer.SetData(sensorData.bodyIndexBufferData);
			Graphics.Blit(null, sensorData.bodyIndexTexture, sensorData.bodyIndexMaterial);

			if(userMapType != KinectManager.UserMapType.UserTexture)
			{
				// buffer is released
				lock(sensorData.bodyIndexBufferLock)
				{
					sensorData.bodyIndexBufferReady = false;
				}
			}
		}
	}

	// Renders depth image texture
	public static void RenderDepthImageTexture(SensorData sensorData)
	{
		if(sensorData.depthImageBufferReady)
		{
			sensorData.depthImageMaterial.SetFloat("_TotalPoints", (float)sensorData.depthHistTotalPoints);
			sensorData.depthImageMaterial.SetInt("_FirstUserIndex", sensorData.firstUserIndex);
			sensorData.depthImageBuffer.SetData(sensorData.depthImageBufferData);
			sensorData.depthHistBuffer.SetData(sensorData.equalHistBufferData);
			
			Graphics.Blit(sensorData.bodyIndexTexture, sensorData.depthImageTexture, sensorData.depthImageMaterial);
			
			// release the buffers for the next poll
			lock(sensorData.depthImageBufferLock)
			{
				sensorData.depthImageBufferReady = false;
			}

			lock(sensorData.bodyIndexBufferLock)
			{
				sensorData.bodyIndexBufferReady = false;
			}
		}
	}

	// renders depth2color texture
	public static bool RenderDepth2ColorTex(SensorData sensorData)
	{
		if(sensorData.depth2ColorMaterial != null && sensorData.depth2ColorCoords != null && sensorData.depthCoordsBufferReady)
		{
			sensorData.depth2ColorBuffer.SetData(sensorData.depth2ColorCoords);
			
			sensorData.depth2ColorMaterial.SetTexture("_BodyTex", sensorData.bodyIndexTexture);
			sensorData.depth2ColorMaterial.SetTexture("_ColorTex", sensorData.colorImageTexture);
			
			Graphics.Blit(null, sensorData.depth2ColorTexture, sensorData.depth2ColorMaterial);
			
			// buffer is released
			lock(sensorData.depthCoordsBufferLock)
			{
				sensorData.depthCoordsBufferReady = false;
			}

			return true;
		}

		return false;
	}

	// Polls for new infrared frame data
	public static bool PollInfraredFrame(SensorData sensorData)
	{
		bool bNewFrame = false;

		if(sensorData.sensorInterface != null && !sensorData.isPlayModeEnabled)
		{
			bNewFrame = sensorData.sensorInterface.PollInfraredFrame(sensorData);
		}

		return bNewFrame;
	}

	// returns depth frame coordinates for the given 3d Kinect-space point
	public static Vector2 MapSpacePointToDepthCoords(SensorData sensorData, Vector3 kinectPos)
	{
		Vector2 vPoint = Vector2.zero;

		if(sensorData.sensorInterface != null)
		{
			vPoint = sensorData.sensorInterface.MapSpacePointToDepthCoords(sensorData, kinectPos);
		}

		return vPoint;
	}

	// returns 3d coordinates for the given depth-map point
	public static Vector3 MapDepthPointToSpaceCoords(SensorData sensorData, Vector2 depthPos, ushort depthVal)
	{
		Vector3 vPoint = Vector3.zero;

		if(sensorData.sensorInterface != null)
		{
			vPoint = sensorData.sensorInterface.MapDepthPointToSpaceCoords(sensorData, depthPos, depthVal);
		}

		return vPoint;
	}

	// returns color-map coordinates for the given depth point
	public static Vector2 MapDepthPointToColorCoords(SensorData sensorData, Vector2 depthPos, ushort depthVal)
	{
		Vector2 vPoint = Vector2.zero;

		if(sensorData.sensorInterface != null)
		{
			vPoint = sensorData.sensorInterface.MapDepthPointToColorCoords(sensorData, depthPos, depthVal);
		}

		return vPoint;
	}

	// estimates color-map coordinates for the current depth frame
	public static bool MapDepthFrameToColorCoords(SensorData sensorData, ref Vector2[] vColorCoords)
	{
		bool bResult = false;
		
		if(sensorData.sensorInterface != null)
		{
			bResult = sensorData.sensorInterface.MapDepthFrameToColorCoords(sensorData, ref vColorCoords);
		}
		
		return bResult;
	}
	
	// estimates depth-map coordinates for the current color frame
	public static bool MapColorFrameToDepthCoords(SensorData sensorData, ref Vector2[] vDepthCoords)
	{
		bool bResult = false;
		
		if(sensorData.sensorInterface != null)
		{
			bResult = sensorData.sensorInterface.MapColorFrameToDepthCoords(sensorData, ref vDepthCoords);
		}
		
		return bResult;
	}
	
	// draws a line in a texture
	public static void DrawLine(Texture2D a_Texture, int x1, int y1, int x2, int y2, Color a_Color)
	{
		int width = a_Texture.width;
		int height = a_Texture.height;
		
		int dy = y2 - y1;
		int dx = x2 - x1;
		
		int stepy = 1;
		if (dy < 0) 
		{
			dy = -dy; 
			stepy = -1;
		}
		
		int stepx = 1;
		if (dx < 0) 
		{
			dx = -dx; 
			stepx = -1;
		}
		
		dy <<= 1;
		dx <<= 1;
		
		if(x1 >= 0 && x1 < width && y1 >= 0 && y1 < height)
			for(int x = -1; x <= 1; x++)
				for(int y = -1; y <= 1; y++)
					a_Texture.SetPixel(x1 + x, y1 + y, a_Color);
		
		if (dx > dy) 
		{
			int fraction = dy - (dx >> 1);
			
			while (x1 != x2) 
			{
				if (fraction >= 0) 
				{
					y1 += stepy;
					fraction -= dx;
				}
				
				x1 += stepx;
				fraction += dy;
				
				if(x1 >= 0 && x1 < width && y1 >= 0 && y1 < height)
					for(int x = -1; x <= 1; x++)
						for(int y = -1; y <= 1; y++)
							a_Texture.SetPixel(x1 + x, y1 + y, a_Color);
			}
		}
		else 
		{
			int fraction = dx - (dy >> 1);
			
			while (y1 != y2) 
			{
				if (fraction >= 0) 
				{
					x1 += stepx;
					fraction -= dy;
				}
				
				y1 += stepy;
				fraction += dx;
				
				if(x1 >= 0 && x1 < width && y1 >= 0 && y1 < height)
					for(int x = -1; x <= 1; x++)
						for(int y = -1; y <= 1; y++)
							a_Texture.SetPixel(x1 + x, y1 + y, a_Color);
			}
		}
		
	}

	// copy source file to the target
	public static bool CopyFile(string sourceFilePath, string targetFilePath, ref bool bOneCopied, ref bool bAllCopied)
	{
		FileInfo sourceFile = new FileInfo(sourceFilePath);
		if(!sourceFile.Exists)
		{
			return false;
		}

		FileInfo targetFile = new FileInfo(targetFilePath);
		if(!targetFile.Directory.Exists)
		{
			targetFile.Directory.Create();
		}
		
		if(!targetFile.Exists || targetFile.Length !=  sourceFile.Length)
		{
			Debug.Log("Copying " + sourceFile.Name + "...");
			File.Copy(sourceFilePath, targetFilePath);
			
			bool bFileCopied = File.Exists(targetFilePath);
			
			bOneCopied = bOneCopied || bFileCopied;
			bAllCopied = bAllCopied && bFileCopied;
			
			return bFileCopied;
		}

		return false;
	}
	
	// Copy a resource file to the target
	public static bool CopyResourceFile(string targetFilePath, string resFileName, ref bool bOneCopied, ref bool bAllCopied)
	{
		TextAsset textRes = Resources.Load(resFileName, typeof(TextAsset)) as TextAsset;
		if(textRes == null)
		{
			bOneCopied = false;
			bAllCopied = false;
			
			return false;
		}
		
		FileInfo targetFile = new FileInfo(targetFilePath);
		if(!targetFile.Directory.Exists)
		{
			targetFile.Directory.Create();
		}
		
		if(!targetFile.Exists || targetFile.Length !=  textRes.bytes.Length)
		{
			Debug.Log("Copying " + resFileName + "...");

			if(textRes != null)
			{
				using (FileStream fileStream = new FileStream (targetFilePath, FileMode.Create, FileAccess.Write, FileShare.Read))
				{
					fileStream.Write(textRes.bytes, 0, textRes.bytes.Length);
				}
				
				bool bFileCopied = File.Exists(targetFilePath);
				
				bOneCopied = bOneCopied || bFileCopied;
				bAllCopied = bAllCopied && bFileCopied;
				
				return bFileCopied;
			}
		}
		
		return false;
	}

	// Unzips resource file to the target path
	public static bool UnzipResourceDirectory(string targetDirPath, string resZipFileName, string checkForDir)
	{
		if(checkForDir != string.Empty && Directory.Exists(checkForDir))
		{
			return false;
		}

		TextAsset textRes = Resources.Load(resZipFileName, typeof(TextAsset)) as TextAsset;
		if(textRes == null || textRes.bytes.Length == 0)
		{
			return false;
		}

		Debug.Log("Unzipping " + resZipFileName + "...");

		// get the resource steam
		MemoryStream memStream = new MemoryStream(textRes.bytes);

		// fix invalid code page 437 error
		ZipConstants.DefaultCodePage = 0;

		using(ZipInputStream s = new ZipInputStream(memStream))
		{
			ZipEntry theEntry;
			while ((theEntry = s.GetNextEntry()) != null) 
			{
				//Debug.Log(theEntry.Name);
				
				string directoryName = targetDirPath + Path.GetDirectoryName(theEntry.Name);
				string fileName = Path.GetFileName(theEntry.Name);

				if(!Directory.Exists(directoryName))
				{
					// create directory
					Directory.CreateDirectory(directoryName);
				}

				if (fileName != string.Empty && !fileName.EndsWith(".meta")) 
				{
					string targetFilePath = directoryName + "/" + fileName;

					using (FileStream streamWriter = File.Create(targetFilePath)) 
					{
						int size = 2048;
						byte[] data = new byte[2048];
						
						while (true) 
						{
							size = s.Read(data, 0, data.Length);
							
							if (size > 0) 
							{
								streamWriter.Write(data, 0, size);
							} 
							else 
							{
								break;
							}
						}
					}
				}
			}
		}

		// close the resource stream
		memStream.Close();

		return true;
	}

	// Unzips resource file to the target path
	public static bool UnzipResourceFiles(Dictionary<string, string> dictFilesToUnzip, string resZipFileName, 
	                                      ref bool bOneCopied, ref bool bAllCopied)
	{
		TextAsset textRes = Resources.Load(resZipFileName, typeof(TextAsset)) as TextAsset;
		if(textRes == null || textRes.bytes.Length == 0)
		{
			bOneCopied = false;
			bAllCopied = false;

			return false;
		}
		
		//Debug.Log("Unzipping " + resZipFileName + "...");
		
		// get the resource steam
		MemoryStream memStream = new MemoryStream(textRes.bytes);
		
		// fix invalid code page 437 error
		ZipConstants.DefaultCodePage = 0;
		
		using(ZipInputStream s = new ZipInputStream(memStream))
		{
			ZipEntry theEntry;
			while ((theEntry = s.GetNextEntry()) != null) 
			{
				//Debug.Log(theEntry.Name);

				if(dictFilesToUnzip.ContainsKey(theEntry.Name))
			   	{
					string targetFilePath = dictFilesToUnzip[theEntry.Name];

					string directoryName = Path.GetDirectoryName(targetFilePath);
					string fileName = Path.GetFileName(theEntry.Name);
					
					if(!Directory.Exists(directoryName))
					{
						// create directory
						Directory.CreateDirectory(directoryName);
					}

					FileInfo targetFile = new FileInfo(targetFilePath);
					bool bTargetFileNewOrUpdated = !targetFile.Exists || targetFile.Length !=  theEntry.Size;
					
					if (fileName != string.Empty && bTargetFileNewOrUpdated) 
					{
						using (FileStream streamWriter = File.Create(targetFilePath)) 
						{
							int size = 2048;
							byte[] data = new byte[2048];
							
							while (true) 
							{
								size = s.Read(data, 0, data.Length);
								
								if (size > 0) 
								{
									streamWriter.Write(data, 0, size);
								} 
								else 
								{
									break;
								}
							}
						}
						
						bool bFileCopied = File.Exists(targetFilePath);
						
						bOneCopied = bOneCopied || bFileCopied;
						bAllCopied = bAllCopied && bFileCopied;
					}
				}

			}
		}
		
		// close the resource stream
		memStream.Close();
		
		return true;
	}
	
	// returns the unzipped file size in bytes, or -1 if the entry is not found in the zip
	public static long GetUnzippedEntrySize(string resZipFileName, string sEntryName)
	{
		TextAsset textRes = Resources.Load(resZipFileName, typeof(TextAsset)) as TextAsset;
		if(textRes == null || textRes.bytes.Length == 0)
		{
			return -1;
		}
		
		// get the resource steam
		MemoryStream memStream = new MemoryStream(textRes.bytes);
		
		// fix invalid code page 437 error
		ZipConstants.DefaultCodePage = 0;
		long entryFileSize = -1;
		
		using(ZipInputStream s = new ZipInputStream(memStream))
		{
			ZipEntry theEntry;
			while ((theEntry = s.GetNextEntry()) != null) 
			{
				if(theEntry.Name == sEntryName)
				{
					entryFileSize = theEntry.Size;
					break;
				}
				
			}
		}
		
		// close the resource stream
		memStream.Close();
		
		return entryFileSize;
	}
	
	// returns true if the project is running on 64-bit architecture, false if 32-bit
	public static bool Is64bitArchitecture()
	{
		int sizeOfPtr = Marshal.SizeOf(typeof(IntPtr));
		return (sizeOfPtr > 4);
	}

	// returns the target dll path for the current platform (x86 or x64)
	public static string GetTargetDllPath(string sAppPath, bool bIs64bitApp)
	{
		string sTargetPath = sAppPath;
//		string sPluginsPath = Application.dataPath + "/Plugins";
//		
//		if(Directory.Exists(sPluginsPath))
//		{
//			sTargetPath = sPluginsPath;
//			
//			//if(Application.isEditor)
//			{
//				string sPlatformPath = sPluginsPath + "/" + (!bIs64bitApp ? "x86" : "x86_64");
//				
//				if(Directory.Exists(sPlatformPath))
//				{
//					sTargetPath = sPlatformPath;
//				}
//			}
//		}
		
		return sTargetPath;
	}

	// cleans up objects and restarts the current level
	public static void RestartLevel(GameObject parentObject, string callerName)
	{
		Debug.Log(callerName + " is restarting level...");

		// destroy parent object if any
		if(parentObject)
		{
			GameObject.Destroy(parentObject);
		}

		// clean up memory assets
		Resources.UnloadUnusedAssets();
		GC.Collect();

		//if(Application.HasProLicense() && Application.isEditor)
		{
#if UNITY_EDITOR
			// refresh the assets database
			UnityEditor.AssetDatabase.Refresh();
#endif
		}
		
		// reload the same level
		Application.LoadLevel(Application.loadedLevel);
		//SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}

	// sets the graphics shader level
	public static void SetGraphicsShaderLevel(int shaderLevel)
	{
		graphicsShaderLevel = shaderLevel;
	}

	// checks if DirectX11/Direct3D-11 is turned on or not
	public static bool IsDirectX11Available()
	{
		return (graphicsShaderLevel >= 50);
	}

	// copies open-cv dlls to the root folder, if needed
	public static bool IsOpenCvAvailable(ref bool bNeedRestart)
	{
		if(IsDirectX11Available())
		{
			// use shaders
			bNeedRestart = false;
			return true;
		}

		bool bOneCopied = false, bAllCopied = true;
		string sTargetPath = GetTargetDllPath(".", Is64bitArchitecture()) + "/";
		//string sTargetPath = ".";
		
		if(!Is64bitArchitecture())
		{
			// 32 bit architecture
			sTargetPath = GetTargetDllPath(".", false) + "/";
			
			Dictionary<string, string> dictFilesToUnzip = new Dictionary<string, string>();
			dictFilesToUnzip["opencv_core2410.dll"] = sTargetPath + "opencv_core2410.dll";
			dictFilesToUnzip["opencv_imgproc2410.dll"] = sTargetPath + "opencv_imgproc2410.dll";
			dictFilesToUnzip["msvcp120.dll"] = sTargetPath + "msvcp120.dll";
			dictFilesToUnzip["msvcr120.dll"] = sTargetPath + "msvcr120.dll";
			
			UnzipResourceFiles(dictFilesToUnzip, "opencv.x86.zip", ref bOneCopied, ref bAllCopied);
		}
		else
		{
			// 64 bit architecture
			sTargetPath = GetTargetDllPath(".", true) + "/";
			
			Dictionary<string, string> dictFilesToUnzip = new Dictionary<string, string>();
			dictFilesToUnzip["opencv_core2410.dll"] = sTargetPath + "opencv_core2410.dll";
			dictFilesToUnzip["opencv_imgproc2410.dll"] = sTargetPath + "opencv_imgproc2410.dll";
			dictFilesToUnzip["msvcp120.dll"] = sTargetPath + "msvcp120.dll";
			dictFilesToUnzip["msvcr120.dll"] = sTargetPath + "msvcr120.dll";
			
			UnzipResourceFiles(dictFilesToUnzip, "opencv.x64.zip", ref bOneCopied, ref bAllCopied);
		}

		bNeedRestart = (bOneCopied && bAllCopied);
		
		return true;
	}

	// initializes background removal with shaders
	public static bool InitBackgroundRemoval(SensorData sensorData, bool isHiResPrefered)
	{
		if(sensorData != null && sensorData.bodyIndexImage != null && sensorData.colorImage != null
		   && IsDirectX11Available())
		{
			sensorData.alphaBodyTexture = new RenderTexture(sensorData.depthImageWidth, sensorData.depthImageHeight, 0);
			sensorData.alphaBodyTexture.wrapMode = TextureWrapMode.Clamp;
			//sensorData.alphaBodyTexture.filterMode = FilterMode.Point;
			
			Shader erodeBodyShader = Shader.Find("Custom/Erode");
			sensorData.erodeBodyMaterial = new Material(erodeBodyShader);
			sensorData.erodeBodyMaterial.SetFloat("_TexResX", (float)sensorData.depthImageWidth);
			sensorData.erodeBodyMaterial.SetFloat("_TexResY", (float)sensorData.depthImageHeight);
			//sensorData.erodeBodyMaterial.SetTexture("_MainTex", sensorData.bodyIndexTexture);
			
			Shader dilateBodyShader = Shader.Find("Custom/Dilate");
			sensorData.dilateBodyMaterial = new Material(dilateBodyShader);
			sensorData.dilateBodyMaterial.SetFloat("_TexResX", (float)sensorData.depthImageWidth);
			sensorData.dilateBodyMaterial.SetFloat("_TexResY", (float)sensorData.depthImageHeight);
			//sensorData.dilateBodyMaterial.SetTexture("_MainTex", sensorData.bodyIndexTexture);

			Shader blurBodyShader = Shader.Find("Custom/BlurShader2");
			sensorData.blurBodyMaterial = new Material(blurBodyShader);
			//sensorData.blurBodyMaterial.SetFloat("_Amount", 1.5f);
			sensorData.blurBodyMaterial.SetFloat("_BlurSizeXY", 2f);

			if(isHiResPrefered)
			{
				Shader color2DepthShader = Shader.Find("Kinect/Color2DepthShader");
				
				if(color2DepthShader)
				{
					sensorData.color2DepthTexture = new RenderTexture(sensorData.colorImageWidth, sensorData.colorImageHeight, 0);
					sensorData.color2DepthTexture.wrapMode = TextureWrapMode.Clamp;
					//sensorData.color2DepthTexture.filterMode = FilterMode.Point;
					
					sensorData.color2DepthMaterial = new Material(color2DepthShader);
					
					sensorData.color2DepthMaterial.SetFloat("_ColorResX", (float)sensorData.colorImageWidth);
					sensorData.color2DepthMaterial.SetFloat("_ColorResY", (float)sensorData.colorImageHeight);
					sensorData.color2DepthMaterial.SetFloat("_DepthResX", (float)sensorData.depthImageWidth);
					sensorData.color2DepthMaterial.SetFloat("_DepthResY", (float)sensorData.depthImageHeight);
					
					sensorData.color2DepthBuffer = new ComputeBuffer(sensorData.colorImageWidth * sensorData.colorImageHeight, sizeof(float) * 2);
					sensorData.color2DepthMaterial.SetBuffer("_DepthCoords", sensorData.color2DepthBuffer);
				}
			}
		}

		if(isHiResPrefered && sensorData != null && sensorData.bodyIndexImage != null && sensorData.colorImage != null)
		{
			sensorData.color2DepthCoords = new Vector2[sensorData.colorImageWidth * sensorData.colorImageHeight];
		}

		return true;
	}

	// releases background removal shader resources
	public static void FinishBackgroundRemoval(SensorData sensorData)
	{
		if(sensorData == null)
			return;

		if(sensorData.alphaBodyTexture != null)
		{
			sensorData.alphaBodyTexture.Release();
			sensorData.alphaBodyTexture = null;
		}
		
		sensorData.erodeBodyMaterial = null;
		sensorData.dilateBodyMaterial = null;
		sensorData.blurBodyMaterial = null;

		if(sensorData.color2DepthBuffer != null)
		{
			sensorData.color2DepthBuffer.Release();
			sensorData.color2DepthBuffer = null;
		}

		if(sensorData.color2DepthTexture != null)
		{
			sensorData.color2DepthTexture.Release();
			sensorData.color2DepthTexture = null;
		}

		sensorData.color2DepthMaterial = null;
		sensorData.color2DepthCoords = null;
	}

	// computes current background removal texture
	public static bool UpdateBackgroundRemoval(SensorData sensorData, bool isHiResPrefered, Color32 defaultColor)
	{
		if(sensorData.erodeBodyMaterial != null && sensorData.dilateBodyMaterial != null && sensorData.blurBodyMaterial)
		{
			ApplyErodeDilate(sensorData.bodyIndexTexture, sensorData.alphaBodyTexture, sensorData.erodeBodyMaterial, 
			                 sensorData.dilateBodyMaterial, 3, 3);
			ApplyImageBlur(sensorData.alphaBodyTexture, sensorData.alphaBodyTexture, sensorData.blurBodyMaterial);
		}

		if(sensorData.color2DepthMaterial != null && sensorData.color2DepthCoords != null && sensorData.depthCoordsBufferReady)
		{
			// blit the hi-res texture
			sensorData.color2DepthBuffer.SetData(sensorData.color2DepthCoords);

			sensorData.color2DepthMaterial.SetTexture("_BodyTex", sensorData.alphaBodyTexture);
			sensorData.color2DepthMaterial.SetTexture("_ColorTex", sensorData.colorImageTexture);
			//sensorData.color2DepthMaterial.SetColor("_DefaultClr", defaultColor);
			
			Graphics.Blit(null, sensorData.color2DepthTexture, sensorData.color2DepthMaterial);
			
			// buffer is released
			lock(sensorData.depthCoordsBufferLock)
			{
				sensorData.depthCoordsBufferReady = false;
			}
		}
		else if(sensorData.depth2ColorMaterial != null && sensorData.depth2ColorCoords != null && sensorData.depthCoordsBufferReady)
		{
			// blit the lo-res texture
			sensorData.depth2ColorBuffer.SetData(sensorData.depth2ColorCoords);

			sensorData.depth2ColorMaterial.SetTexture("_BodyTex", sensorData.alphaBodyTexture);
			sensorData.depth2ColorMaterial.SetTexture("_ColorTex", sensorData.colorImageTexture);
			//sensorData.depth2ColorMaterial.SetColor("_DefaultClr", defaultColor);
			
			Graphics.Blit(null, sensorData.depth2ColorTexture, sensorData.depth2ColorMaterial);
			
			// buffer is released
			lock(sensorData.depthCoordsBufferLock)
			{
				sensorData.depthCoordsBufferReady = false;
			}
		}
		
		return true;
	}

	private static void ApplyErodeDilate(RenderTexture source, RenderTexture destination, Material erodeMaterial, 
	                                     Material dilateMaterial, int erodeIterations, int dilateIterations)
	{
		if(!source || !destination || !erodeMaterial || !dilateMaterial)
			return;

		RenderTexture[] tempTexture = new RenderTexture[2];
		tempTexture[0] = RenderTexture.GetTemporary(source.width, source.height, 0);
		tempTexture[1] = RenderTexture.GetTemporary(source.width, source.height, 0);

		Graphics.Blit(source, tempTexture[0]);

		for(int i = 0; i < erodeIterations; i++)
		{
			Graphics.Blit(tempTexture[i % 2], tempTexture[(i + 1) % 2], erodeMaterial);
		}

		if((erodeIterations % 2) != 0)
		{
			Graphics.Blit(tempTexture[1], tempTexture[0]);
		}
		
		for(int i = 0; i < dilateIterations; i++)
		{
			Graphics.Blit(tempTexture[i % 2], tempTexture[(i + 1) % 2], dilateMaterial);
		}

		Graphics.Blit(tempTexture[dilateIterations % 2], destination);

		RenderTexture.ReleaseTemporary(tempTexture[0]);
		RenderTexture.ReleaseTemporary(tempTexture[1]);
	}

	private static void ApplyImageBlur(RenderTexture source, RenderTexture destination, Material blurMaterial)
	{
		if(!source || !destination || !blurMaterial)
			return;

		Graphics.Blit(source, destination, blurMaterial);
	}

	// returns the foregound frame rectangle, as to the required resolution
	public static Rect GetForegroundFrameRect(SensorData sensorData, bool isHiResPrefered)
	{
		if(isHiResPrefered && sensorData != null && sensorData.sensorInterface != null)
		{
			if(sensorData.sensorInterface.IsBRHiResSupported() && sensorData.colorImage != null)
			{
				return new Rect(0f, 0f, sensorData.colorImageWidth, sensorData.colorImageHeight);
			}
		}
		
		return sensorData != null ? new Rect(0f, 0f, sensorData.depthImageWidth, sensorData.depthImageHeight) : new Rect();
	}

	// returns the foregound frame length, as to the required resolution
	public static int GetForegroundFrameLength(SensorData sensorData, bool isHiResPrefered)
	{
		if(isHiResPrefered && sensorData != null && sensorData.sensorInterface != null)
		{
			if(sensorData.sensorInterface.IsBRHiResSupported() && sensorData.colorImage != null)
			{
				return sensorData.colorImage.Length;
			}
		}

		return (sensorData != null && sensorData.bodyIndexImage != null) ? sensorData.bodyIndexImage.Length * 4 : 0;
	}
	
	private static bool GetForegroundAlphaFrame(SensorData sensorData, bool bLimitedUsers, ICollection<int> alTrackedIndexes, ref byte[] fgAlphaFrame)
	{
		if(sensorData == null || sensorData.bodyIndexImage == null)
			return false;

		CvMat cvAlphaMap = new CvMat(sensorData.depthImageHeight, sensorData.depthImageWidth, MatrixType.U8C1);

		System.IntPtr rawPtrAlpha;
		cvAlphaMap.GetRawData(out rawPtrAlpha);

		if(sensorData.selectedBodyIndex != 255 || bLimitedUsers)
		{
			// copy body-index selectively
			byte btSelBI = sensorData.selectedBodyIndex;
			int iBodyIndexLength = sensorData.bodyIndexImage.Length;

			for (int i = 0; i < iBodyIndexLength; i++)
			{
				byte btBufBI = sensorData.bodyIndexImage[i];

				bool bUserTracked = btSelBI != 255 ? btSelBI == btBufBI : 
					(btBufBI != 255 ? alTrackedIndexes.Contains((int)btBufBI) : false);

				if(bUserTracked)
				{
					cvAlphaMap.Set1D(i, btBufBI);
				}
				else
				{
					cvAlphaMap.Set1D(i, 255);
				}
			}
		}
		else
		{
			// copy the entire body-index buffer
			Marshal.Copy(sensorData.bodyIndexImage, 0, rawPtrAlpha, sensorData.bodyIndexImage.Length);
		}

		// make the image b&w
		cvAlphaMap.Threshold(cvAlphaMap, 254, 255, ThresholdType.BinaryInv);

		// apply erode, dilate and blur
		cvAlphaMap.Erode(cvAlphaMap);
		cvAlphaMap.Dilate(cvAlphaMap);
		cvAlphaMap.Smooth(cvAlphaMap, SmoothType.Blur, 5, 5);
		//cvAlphaMap.Smooth(cvAlphaMap, SmoothType.Median, 7);

		// get the foreground image
		Marshal.Copy(rawPtrAlpha, fgAlphaFrame, 0, fgAlphaFrame.Length);

		return true;
	}
	
	// gets the updated foreground frame, as to the required resolution
	public static bool PollForegroundFrame(SensorData sensorData, bool isHiResPrefered, Color32 defaultColor, bool bLimitedUsers, ICollection<int> alTrackedIndexes, ref byte[] fgImageFrame)
	{
		if(IsDirectX11Available())
			return false;

		if(sensorData.colorImage == null)
			return false;

		// get the alpha frame
		byte[] fgAlphaFrame = new byte[sensorData.bodyIndexImage.Length];
		if(!GetForegroundAlphaFrame(sensorData, bLimitedUsers, alTrackedIndexes, ref fgAlphaFrame))
			return false;
		
		int alphaImageLength = fgAlphaFrame.Length;
		int colorImageLength = sensorData.colorImageWidth * sensorData.colorImageHeight;
		
		Array.Clear(fgImageFrame, 0, fgImageFrame.Length);
		
		// try to get the full color frame coordinates
		if(isHiResPrefered && sensorData.color2DepthCoords != null && sensorData.depthCoordsBufferReady)
		{
			for (int i = 0, fi = 0; i < colorImageLength; i++, fi += 4)
			{
				Vector2 vDepthPos = sensorData.color2DepthCoords[i];

				if(!float.IsInfinity(vDepthPos.x) && !float.IsInfinity(vDepthPos.y))
				{
					int dx = Mathf.RoundToInt(vDepthPos.x);
					int dy = Mathf.RoundToInt(vDepthPos.y);

					int di = dx + dy * sensorData.depthImageWidth;

					if(di >= 0 && di < fgAlphaFrame.Length)
					{
						int ci = i << 2;

						fgImageFrame[fi] = sensorData.colorImage[ci];
						fgImageFrame[fi + 1] = sensorData.colorImage[ci + 1];
						fgImageFrame[fi + 2] = sensorData.colorImage[ci + 2];
						fgImageFrame[fi + 3] = fgAlphaFrame[di];
					}
				}
				else
				{
					fgImageFrame[fi + 3] = 0;
				}
			}
			
			// buffer is released
			lock(sensorData.depthCoordsBufferLock)
			{
				sensorData.depthCoordsBufferReady = false;
			}
		}
		else
		{
			for (int i = 0, fi = 0; i < alphaImageLength; i++, fi += 4)
			{
				Vector2 vColorPos = Vector2.zero;
				
				if(sensorData.depth2ColorCoords != null && sensorData.depthCoordsBufferReady)
				{
					vColorPos = sensorData.depth2ColorCoords[i];
				}
				else
				{
					Vector2 vDepthPos = Vector2.zero;
					vDepthPos.x = i % sensorData.depthImageWidth;
					vDepthPos.y = i / sensorData.depthImageWidth;
					
					ushort userDepth = sensorData.depthImage[i];
					vColorPos = MapDepthPointToColorCoords(sensorData, vDepthPos, userDepth);
				}
				
				if(!float.IsInfinity(vColorPos.x) && !float.IsInfinity(vColorPos.y))
				{
					int cx = (int)vColorPos.x;
					int cy = (int)vColorPos.y;
					int colorIndex = cx + cy * sensorData.colorImageWidth;
					
					if(colorIndex >= 0 && colorIndex < colorImageLength)
					{
						int ci = colorIndex << 2;
						
						fgImageFrame[fi] = sensorData.colorImage[ci];
						fgImageFrame[fi + 1] = sensorData.colorImage[ci + 1];
						fgImageFrame[fi + 2] = sensorData.colorImage[ci + 2];
						fgImageFrame[fi + 3] = fgAlphaFrame[i];
					}
				}
				else
				{
					fgImageFrame[fi] = defaultColor.r;
					fgImageFrame[fi + 1] = defaultColor.g;
					fgImageFrame[fi + 2] = defaultColor.b;
					fgImageFrame[fi + 3] = fgAlphaFrame[i];
				}
			}
			
			// buffer is released
			lock(sensorData.depthCoordsBufferLock)
			{
				sensorData.depthCoordsBufferReady = false;
			}
		}

		return true;
	}
	
	// reads render texture contents into tex2d (it must have the same width and height).
	public static bool RenderTex2Tex2D(RenderTexture rt, ref Texture2D tex) 
	{
		if(!rt || !tex || rt.width != tex.width || rt.height != tex.height)
			return false;
		
		RenderTexture currentActiveRT = RenderTexture.active;
		RenderTexture.active = rt;
		
		tex.ReadPixels(new Rect(0, 0, tex.width, tex.height), 0, 0);
		tex.Apply();
		
		RenderTexture.active = currentActiveRT;
		
		return true;
	}

	// DLL Imports for native library functions
	[DllImport("kernel32", SetLastError=true, CharSet = CharSet.Ansi)]
	static extern IntPtr LoadLibrary([MarshalAs(UnmanagedType.LPStr)]string lpFileName);
	
	[DllImport("kernel32", SetLastError=true)]
	static extern bool FreeLibrary(IntPtr hModule);

	// load the native dll to ensure the library is loaded
	public static bool LoadNativeLib(string sLibName)
	{
		string sTargetPath = KinectInterop.GetTargetDllPath(".", Is64bitArchitecture());
		string sFullLibPath = sTargetPath + "/" + sLibName;

		IntPtr hLibrary = LoadLibrary(sFullLibPath);

		return (hLibrary != IntPtr.Zero);
	}
	
	// unloads and deletes native library
	public static void DeleteNativeLib(string sLibName, bool bUnloadLib)
	{
		string sTargetPath = KinectInterop.GetTargetDllPath(".", Is64bitArchitecture());
		string sFullLibPath = sTargetPath + "/" + sLibName;
		
		if(bUnloadLib)
		{
			IntPtr hLibrary = LoadLibrary(sFullLibPath);
			
			if(hLibrary != IntPtr.Zero)
			{
				FreeLibrary(hLibrary);
				FreeLibrary(hLibrary);
			}
		}
		
		try 
		{
			// delete file
			if(File.Exists(sFullLibPath))
			{
				File.Delete(sFullLibPath);
			}
		} 
		catch (Exception) 
		{
			Debug.Log("Could not delete file: " + sFullLibPath);
		}
	}
	
}