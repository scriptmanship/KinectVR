using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System;

public class Kinect1Interface : DepthSensorInterface 
{
	public static class Constants
	{
		public const int NuiSkeletonCount = 6;
		public const int JointCount = 20;

		public const NuiImageResolution ColorImageResolution = NuiImageResolution.resolution640x480;
		public const NuiImageResolution DepthImageResolution = NuiImageResolution.resolution640x480;
		
		public const bool IsNearMode = true;
	}

	// Structs and constants for interfacing C# with the Kinect.dll 

	[Flags]
	public enum NuiInitializeFlags : uint
	{
		UsesNone = 0,
		UsesAudio = 0x10000000,
		UsesDepthAndPlayerIndex = 0x00000001,
		UsesColor = 0x00000002,
		UsesSkeleton = 0x00000008,
		UsesDepth = 0x00000020,
		UsesHighQualityColor = 0x00000040
	}
	
	public enum NuiErrorCodes : uint
	{
		FrameNoData = 0x83010001,
		StreamNotEnabled = 0x83010002,
		ImageStreamInUse = 0x83010003,
		FrameLimitExceeded = 0x83010004,
		FeatureNotInitialized = 0x83010005,
		DeviceNotGenuine = 0x83010006,
		InsufficientBandwidth = 0x83010007,
		DeviceNotSupported = 0x83010008,
		DeviceInUse = 0x83010009,
		
		DatabaseNotFound = 0x8301000D,
		DatabaseVersionMismatch = 0x8301000E,
		HardwareFeatureUnavailable = 0x8301000F,
		
		DeviceNotConnected = 0x83010014,
		DeviceNotReady = 0x83010015,
		SkeletalEngineBusy = 0x830100AA,
		DeviceNotPowered = 0x8301027F,
	}
	
	public enum NuiSkeletonPositionIndex : int
	{
		HipCenter = 0,
		Spine,
		ShoulderCenter,
		Head,
		ShoulderLeft,
		ElbowLeft,
		WristLeft,
		HandLeft,
		ShoulderRight,
		ElbowRight,
		WristRight,
		HandRight,
		HipLeft,
		KneeLeft,
		AnkleLeft,
		FootLeft,
		HipRight,
		KneeRight,
		AnkleRight,
		FootRight,
		Count
	}
	
	public enum NuiSkeletonPositionTrackingState
	{
		NotTracked = 0,
		Inferred,
		Tracked
	}
	
	public enum NuiSkeletonTrackingState
	{
		NotTracked = 0,
		PositionOnly,
		SkeletonTracked
	}
	
	public enum NuiImageType
	{
		DepthAndPlayerIndex = 0,	// USHORT
		Color,						// RGB32 data
		ColorYUV,					// YUY2 stream from camera h/w, but converted to RGB32 before user getting it.
		ColorRawYUV,				// YUY2 stream from camera h/w.
		Depth						// USHORT
	}
	
	public enum NuiImageResolution
	{
		resolutionInvalid = -1,
		resolution80x60 = 0,
		resolution320x240 = 1,
		resolution640x480 = 2,
		resolution1280x960 = 3     // for hires color only
	}

	public enum NuiImageStreamFlags
	{
		None = 0x00000000,
		SupressNoFrameData = 0x0001000,
		EnableNearMode = 0x00020000,
		TooFarIsNonZero = 0x0004000
	}
	
	public struct NuiSkeletonData
	{
		public NuiSkeletonTrackingState eTrackingState;
		public uint dwTrackingID;
		public uint dwEnrollmentIndex_NotUsed;
		public uint dwUserIndex;
		public Vector4 Position;
		[MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 20, ArraySubType = UnmanagedType.Struct)]
		public Vector4[] SkeletonPositions;
		[MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 20, ArraySubType = UnmanagedType.Struct)]
		public NuiSkeletonPositionTrackingState[] eSkeletonPositionTrackingState;
		public uint dwQualityFlags;
	}
	
	public struct NuiSkeletonFrame
	{
		public long liTimeStamp;
		public uint dwFrameNumber;
		public uint dwFlags;
		public Vector4 vFloorClipPlane;
		public Vector4 vNormalToGravity;
		[MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 6, ArraySubType = UnmanagedType.Struct)]
		public NuiSkeletonData[] SkeletonData;
	}
	
	public struct NuiTransformSmoothParameters
	{
		public float fSmoothing;
		public float fCorrection;
		public float fPrediction;
		public float fJitterRadius;
		public float fMaxDeviationRadius;
	}
	
	public struct NuiSkeletonBoneRotation
	{
		public Matrix4x4 rotationMatrix;
		public Quaternion rotationQuaternion;
	}
	
	public struct NuiSkeletonBoneOrientation
	{
		public NuiSkeletonPositionIndex endJoint;
		public NuiSkeletonPositionIndex startJoint;
		public NuiSkeletonBoneRotation hierarchicalRotation;
		public NuiSkeletonBoneRotation absoluteRotation;
	}
	
	public struct NuiImageViewArea
	{
		public int eDigitalZoom;
		public int lCenterX;
		public int lCenterY;
	}
	
	public class NuiImageBuffer
	{
		public int m_Width;
		public int m_Height;
		public int m_BytesPerPixel;
		public IntPtr m_pBuffer;
	}
	
	public struct NuiImageFrame
	{
		public Int64 liTimeStamp;
		public uint dwFrameNumber;
		public NuiImageType eImageType;
		public NuiImageResolution eResolution;
		//[MarshalAsAttribute(UnmanagedType.Interface)]
		public IntPtr pFrameTexture;
		public uint dwFrameFlags_NotUsed;
		public NuiImageViewArea ViewArea_NotUsed;
	}
	
	public struct NuiLockedRect
	{
		public int pitch;
		public int size;
		//[MarshalAsAttribute(UnmanagedType.U8)] 
		public IntPtr pBits; 
		
	}

	public enum NuiHandpointerState : uint
	{
		None = 0,
		Tracked = 1,
		Active = 2,
		Interactive = 4,
		Pressed = 8,
		PrimaryForUser = 0x10
	}
	
	public enum InteractionHandEventType : int
	{
		None = 0,
		Grip = 1,
		Release = 2
	}
	
	private NuiImageViewArea pcViewArea = new NuiImageViewArea 
	{
		eDigitalZoom = 0,
		lCenterX = 0,
		lCenterY = 0
	};
	
	public struct FaceRect
	{
		public int x;
		public int y;
		public int width;
		public int height;
	}
	

	// private interface data

	private KinectInterop.FrameSource sourceFlags;

	//private IntPtr colorStreamHandle;
	//private IntPtr depthStreamHandle;

	private NuiSkeletonFrame skeletonFrame;
	private NuiTransformSmoothParameters smoothParameters;

	private Dictionary<uint, InteractionHandEventType> lastLeftHandEvent = new Dictionary<uint, InteractionHandEventType>();
	private Dictionary<uint, InteractionHandEventType> lastRightHandEvent = new Dictionary<uint, InteractionHandEventType>();

	private bool isUseFaceModel = false;
	private bool isDrawFaceRect = false;

	private bool bFaceTrackingInited = false;
	private FaceRect faceRect;
	private Vector4 vHeadPos = Vector4.zero;
	private Vector4 vHeadRot = Vector4.zero;

	private float[] afAU = null;
	private float[] afSU = null;

	private bool bBackgroundRemovalInited = false;

	// exported wrapper functions

	[DllImport(@"Kinect10.dll")]
	private static extern int NuiGetSensorCount(out int pCount);

	[DllImport(@"Kinect10.dll")]
	private static extern int NuiTransformSmooth(ref NuiSkeletonFrame pSkeletonFrame, ref NuiTransformSmoothParameters pSmoothingParams);

	[DllImport(@"Kinect10.dll")]
	private static extern int NuiImageGetColorPixelCoordinatesFromDepthPixelAtResolution(NuiImageResolution eColorResolution, NuiImageResolution eDepthResolution, ref NuiImageViewArea pcViewArea, int lDepthX, int lDepthY, ushort sDepthValue, out int plColorX, out int plColorY);
	

	[DllImportAttribute(@"KinectUnityWrapper.dll")]
	private static extern int InitKinectSensor(NuiInitializeFlags dwFlags, bool bEnableEvents, int iColorResolution, int iDepthResolution, bool bNearMode);
	
	[DllImportAttribute(@"KinectUnityWrapper.dll")]
	private static extern void ShutdownKinectSensor();
	
	[DllImportAttribute(@"KinectUnityWrapper.dll")]
	private static extern int SetKinectElevationAngle(int sensorAngle);
	
	[DllImportAttribute(@"KinectUnityWrapper.dll")]
	private static extern int GetKinectElevationAngle();
	
	[DllImportAttribute(@"KinectUnityWrapper.dll")]
	private static extern int UpdateKinectSensor();
	
	[DllImport(@"KinectUnityWrapper.dll")]
	private static extern int GetSkeletonFrameLength();
	
	[DllImport(@"KinectUnityWrapper.dll")]
	private static extern bool GetSkeletonFrameData(ref NuiSkeletonFrame pSkeletonData, ref uint iDataBufLen, bool bNewFrame);
	
	[DllImport(@"KinectUnityWrapper.dll")]
	private static extern int GetNextSkeletonFrame(uint dwWaitMs);

	[DllImport(@"KinectUnityWrapper.dll")]
	private static extern IntPtr GetColorStreamHandle();
	
	[DllImport(@"KinectUnityWrapper.dll")]
	private static extern IntPtr GetDepthStreamHandle();
	
	[DllImport(@"KinectUnityWrapper.dll")]
	private static extern bool GetColorFrameData(IntPtr btVideoBuf, ref uint iVideoBufLen, bool bGetNewFrame);
	
	[DllImport(@"KinectUnityWrapper.dll")]
	private static extern bool GetDepthFrameData(IntPtr shDepthBuf, ref uint iDepthBufLen, bool bGetNewFrame);
	
	[DllImport(@"KinectUnityWrapper.dll")]
	private static extern bool GetInfraredFrameData(IntPtr shInfraredBuf, ref uint iInfraredBufLen, bool bGetNewFrame);


	[DllImport(@"KinectUnityWrapper")]
	private static extern int InitKinectInteraction();
	
	[DllImport(@"KinectUnityWrapper")]
	private static extern void FinishKinectInteraction();
	
	[DllImport( @"KinectUnityWrapper")]
	private static extern uint GetInteractorsCount();
	
	[DllImport( @"KinectUnityWrapper", EntryPoint = "GetInteractorSkeletonTrackingID" )]
	private static extern uint GetSkeletonTrackingID( uint player );
	
	[DllImport( @"KinectUnityWrapper", EntryPoint = "GetInteractorLeftHandState" )]
	private static extern uint GetLeftHandState( uint player );
	
	[DllImport( @"KinectUnityWrapper", EntryPoint = "GetInteractorRightHandState" )]
	private static extern uint GetRightHandState( uint player );
	
	[DllImport( @"KinectUnityWrapper", EntryPoint = "GetInteractorLeftHandEvent" )]
	private static extern InteractionHandEventType GetLeftHandEvent( uint player );
	
	[DllImport( @"KinectUnityWrapper", EntryPoint = "GetInteractorRightHandEvent" )]
	private static extern InteractionHandEventType GetRightHandEvent( uint player );
	

	[DllImport("KinectUnityWrapper", EntryPoint = "InitFaceTracking")]
	private static extern int InitFaceTrackingNative();

	[DllImport("KinectUnityWrapper", EntryPoint = "FinishFaceTracking")]
	private static extern void FinishFaceTrackingNative();

	[DllImport("KinectUnityWrapper", EntryPoint = "UpdateFaceTracking")]
	private static extern int UpdateFaceTrackingNative();
	
	[DllImport("KinectUnityWrapper", EntryPoint = "IsFaceTracked")]
	private static extern bool IsFaceTrackedNative();

	[DllImport("KinectUnityWrapper", EntryPoint = "GetFaceTrackingID")]
	private static extern uint GetFaceTrackingIDNative();
	
	[DllImport(@"KinectUnityWrapper.dll", EntryPoint = "GetHeadPosition")]
	private static extern bool GetHeadPositionNative(ref Vector4 pvHeadPos);

	[DllImport(@"KinectUnityWrapper.dll", EntryPoint = "GetHeadRotation")]
	private static extern bool GetHeadRotationNative(ref Vector4 pvHeadRot);

	[DllImport(@"KinectUnityWrapper.dll", EntryPoint = "GetHeadScale")]
	private static extern bool GetHeadScaleNative(ref Vector4 pvHeadScale);
	
	[DllImport(@"KinectUnityWrapper.dll", EntryPoint = "GetFaceRect")]
	private static extern bool GetFaceRectNative(ref FaceRect pRectFace);

	[DllImport("KinectUnityWrapper", EntryPoint = "GetAnimUnitsCount")]
	private static extern int GetAnimUnitsCountNative();

	[DllImport("KinectUnityWrapper", EntryPoint = "GetAnimUnits")]
	private static extern bool GetAnimUnitsNative(IntPtr afAU, ref int iAUCount);

	[DllImport("KinectUnityWrapper", EntryPoint = "GetShapeUnitsCount")]
	private static extern int GetShapeUnitsCountNative();

	[DllImport("KinectUnityWrapper", EntryPoint = "GetShapeUnits")]
	private static extern bool GetShapeUnitsNative(IntPtr afSU, ref int iSUCount);

	[DllImport("KinectUnityWrapper", EntryPoint = "GetShapePointsCount")]
	private static extern int GetFacePointsCountNative();

	[DllImport("KinectUnityWrapper", EntryPoint = "GetShapePoints")]
	private static extern bool GetFacePointsNative(IntPtr avPoints, ref int iPointsCount);
	
	[DllImport("KinectUnityWrapper", EntryPoint = "Get3DShapePointsCount")]
	private static extern int GetModelPointsCountNative();

	[DllImport("KinectUnityWrapper", EntryPoint = "Get3DShapePoints")]
	private static extern bool GetModelPointsNative(IntPtr avPoints, ref int iPointsCount);

	[DllImport("KinectUnityWrapper", EntryPoint = "GetTriangleCount")]
	private static extern int GetTriangleCountNative();
	
	[DllImport("KinectUnityWrapper", EntryPoint = "GetTriangles")]
	private static extern bool GetTrianglesNative(IntPtr aiTriangles, ref int iPointsCount);
	
	// speech wrapper functions
	[DllImport("KinectUnityWrapper", EntryPoint = "InitSpeechRecognizer")]
	private static extern int InitSpeechRecognizerNative([MarshalAs(UnmanagedType.LPWStr)]string sRecoCriteria, bool bUseKinect, bool bAdaptationOff);

	[DllImport("KinectUnityWrapper", EntryPoint = "FinishSpeechRecognizer")]
	private static extern void FinishSpeechRecognizerNative();
	
	[DllImport("KinectUnityWrapper", EntryPoint = "UpdateSpeechRecognizer")]
	private static extern int UpdateSpeechRecognizerNative();
	
	[DllImport("KinectUnityWrapper", EntryPoint = "LoadSpeechGrammar")]
	private static extern int LoadSpeechGrammarNative([MarshalAs(UnmanagedType.LPWStr)]string sFileName, short iNewLangCode, bool bDynamic);
	
	[DllImport("KinectUnityWrapper", EntryPoint = "AddGrammarPhrase")]
	private static extern int AddGrammarPhraseNative([MarshalAs(UnmanagedType.LPWStr)]string sFromRule, [MarshalAs(UnmanagedType.LPWStr)]string sToRule, [MarshalAs(UnmanagedType.LPWStr)]string sPhrase, bool bClearRule, bool bCommitGrammar);
	
	[DllImport("KinectUnityWrapper", EntryPoint = "SetRequiredConfidence")]
	private static extern void SetSpeechConfidenceNative(float fConfidence);
	
	[DllImport("KinectUnityWrapper", EntryPoint = "IsSoundStarted")]
	private static extern bool IsSpeechStartedNative();
	
	[DllImport("KinectUnityWrapper", EntryPoint = "IsSoundEnded")]
	private static extern bool IsSpeechEndedNative();
	
	[DllImport("KinectUnityWrapper", EntryPoint = "IsPhraseRecognized")]
	private static extern bool IsPhraseRecognizedNative();
	
	[DllImport("KinectUnityWrapper", EntryPoint = "GetPhraseConfidence")]
	private static extern float GetPhraseConfidenceNative();
	
	[DllImport("KinectUnityWrapper", EntryPoint = "GetRecognizedTag")]
	private static extern IntPtr GetRecognizedPhraseTagNative();
	
	[DllImport("KinectUnityWrapper", EntryPoint = "ClearPhraseRecognized")]
	private static extern void ClearRecognizedPhraseNative();


	[DllImport(@"KinectUnityWrapper", EntryPoint = "InitBackgroundRemoval")]
	public static extern int InitBackgroundRemovalNative();
	
	[DllImport(@"KinectUnityWrapper", EntryPoint = "FinishBackgroundRemoval")]
	public static extern void FinishBackgroundRemovalNative();
	
	[DllImport(@"KinectUnityWrapper", EntryPoint = "UpdateBackgroundRemoval")]
	public static extern int UpdateBackgroundRemovalNative();

	[DllImport(@"KinectUnityWrapper", EntryPoint = "IsBackgroundRemovalActive")]
	public static extern bool IsBackgroundRemovalActiveNative();
	
	[DllImport(@"KinectUnityWrapper", EntryPoint = "GetBackgroundRemovalFrameLength")]
	public static extern int GetBackgroundRemovalFrameLengthNative();
	
	[DllImport(@"KinectUnityWrapper.dll", EntryPoint = "GetBackgroundRemovalFrameData")]
	public static extern bool GetBackgroundRemovalFrameDataNative(IntPtr btVideoBuf, ref uint iVideoBufLen, bool bGetNewFrame);
	

	private string GetNuiErrorString(int hr)
	{
		string message = string.Empty;
		uint uhr = (uint)hr;
		
		switch(uhr)
		{
		case (uint)NuiErrorCodes.FrameNoData:
			message = "Frame contains no data.";
			break;
		case (uint)NuiErrorCodes.StreamNotEnabled:
			message = "Stream is not enabled.";
			break;
		case (uint)NuiErrorCodes.ImageStreamInUse:
			message = "Image stream is already in use.";
			break;
		case (uint)NuiErrorCodes.FrameLimitExceeded:
			message = "Frame limit is exceeded.";
			break;
		case (uint)NuiErrorCodes.FeatureNotInitialized:
			message = "Feature is not initialized.";
			break;
		case (uint)NuiErrorCodes.DeviceNotGenuine:
			message = "Device is not genuine.";
			break;
		case (uint)NuiErrorCodes.InsufficientBandwidth:
			message = "Bandwidth is not sufficient.";
			break;
		case (uint)NuiErrorCodes.DeviceNotSupported:
			message = "Device is not supported (e.g. Kinect for XBox 360).";
			break;
		case (uint)NuiErrorCodes.DeviceInUse:
			message = "Device is already in use.";
			break;
		case (uint)NuiErrorCodes.DatabaseNotFound:
			message = "Database not found.";
			break;
		case (uint)NuiErrorCodes.DatabaseVersionMismatch:
			message = "Database version mismatch.";
			break;
		case (uint)NuiErrorCodes.HardwareFeatureUnavailable:
			message = "Hardware feature is not available.";
			break;
		case (uint)NuiErrorCodes.DeviceNotConnected:
			message = "Device is not connected.";
			break;
		case (uint)NuiErrorCodes.DeviceNotReady:
			message = "Device is not ready.";
			break;
		case (uint)NuiErrorCodes.SkeletalEngineBusy:
			message = "Skeletal engine is busy.";
			break;
		case (uint)NuiErrorCodes.DeviceNotPowered:
			message = "Device is not powered.";
			break;
			
		default:
			message = "hr=0x" + uhr.ToString("X");
			break;
		}
		
		return message;
	}

	private bool NuiImageResolutionToSize(NuiImageResolution res, out int refWidth, out int refHeight)
	{
		switch( res )
		{
			case NuiImageResolution.resolution80x60:
				refWidth = 80;
				refHeight = 60;
				return true;
			case NuiImageResolution.resolution320x240:
				refWidth = 320;
				refHeight = 240;
				return true;
			case NuiImageResolution.resolution640x480:
				refWidth = 640;
				refHeight = 480;
				return true;
			case NuiImageResolution.resolution1280x960:
				refWidth = 1280;
				refHeight = 960;
				return true;
			default:
				refWidth = 0;
				refHeight = 0;
				break;
		}

		return false;
	}

	public KinectInterop.DepthSensorPlatform GetSensorPlatform()
	{
		return KinectInterop.DepthSensorPlatform.KinectSDKv1;
	}
	
	public bool InitSensorInterface (bool bCopyLibs, ref bool bNeedRestart)
	{
		bool bOneCopied = false, bAllCopied = true;
		//string sTargetPath = KinectInterop.GetTargetDllPath(".", KinectInterop.Is64bitArchitecture()) + "/";
		string sTargetPath = "./";

		if(!bCopyLibs)
		{
			// check if the native library is there
			string sTargetLib = sTargetPath + "KinectUnityWrapper.dll";
			bNeedRestart = false;

			string sZipFileName = !KinectInterop.Is64bitArchitecture() ? "KinectV1UnityWrapper.x86.zip" : "KinectV1UnityWrapper.x64.zip";
			long iTargetSize = KinectInterop.GetUnzippedEntrySize(sZipFileName, "KinectUnityWrapper.dll");

			System.IO.FileInfo targetFile = new System.IO.FileInfo(sTargetLib);
			return targetFile.Exists && targetFile.Length == iTargetSize;
		}

		if(!KinectInterop.Is64bitArchitecture())
		{
			//Debug.Log("x32-architecture detected.");

			Dictionary<string, string> dictFilesToUnzip = new Dictionary<string, string>();

			dictFilesToUnzip["FaceTrackData.dll"] = sTargetPath + "FaceTrackData.dll";
			KinectInterop.UnzipResourceFiles(dictFilesToUnzip, "KinectV1FaceData.zip", ref bOneCopied, ref bAllCopied);

			dictFilesToUnzip.Clear();
			dictFilesToUnzip["KinectUnityWrapper.dll"] = sTargetPath + "KinectUnityWrapper.dll";
			dictFilesToUnzip["KinectInteraction180_32.dll"] = sTargetPath + "KinectInteraction180_32.dll";
			dictFilesToUnzip["FaceTrackLib.dll"] = sTargetPath + "FaceTrackLib.dll";
			dictFilesToUnzip["KinectBackgroundRemoval180_32.dll"] = sTargetPath + "KinectBackgroundRemoval180_32.dll";

			dictFilesToUnzip["msvcp100.dll"] = sTargetPath + "msvcp100.dll";
			dictFilesToUnzip["msvcr100.dll"] = sTargetPath + "msvcr100.dll";

			KinectInterop.UnzipResourceFiles(dictFilesToUnzip, "KinectV1UnityWrapper.x86.zip", ref bOneCopied, ref bAllCopied);
		}
		else
		{
			//Debug.Log("x64-architecture detected.");

			Dictionary<string, string> dictFilesToUnzip = new Dictionary<string, string>();
			
			dictFilesToUnzip["FaceTrackData.dll"] = sTargetPath + "FaceTrackData.dll";
			KinectInterop.UnzipResourceFiles(dictFilesToUnzip, "KinectV1FaceData.zip", ref bOneCopied, ref bAllCopied);

			dictFilesToUnzip.Clear();
			dictFilesToUnzip["KinectUnityWrapper.dll"] = sTargetPath + "KinectUnityWrapper.dll";
			dictFilesToUnzip["KinectInteraction180_64.dll"] = sTargetPath + "KinectInteraction180_64.dll";
			dictFilesToUnzip["FaceTrackLib.dll"] = sTargetPath + "FaceTrackLib.dll";
			dictFilesToUnzip["KinectBackgroundRemoval180_64.dll"] = sTargetPath + "KinectBackgroundRemoval180_64.dll";
			
			dictFilesToUnzip["msvcp100.dll"] = sTargetPath + "msvcp100.dll";
			dictFilesToUnzip["msvcr100.dll"] = sTargetPath + "msvcr100.dll";
			
			KinectInterop.UnzipResourceFiles(dictFilesToUnzip, "KinectV1UnityWrapper.x64.zip", ref bOneCopied, ref bAllCopied);
		}

		bNeedRestart = (bOneCopied && bAllCopied);

		return true;
	}

	public void FreeSensorInterface (bool bDeleteLibs)
	{
		if(bDeleteLibs)
		{
			KinectInterop.DeleteNativeLib("KinectUnityWrapper.dll", true);
			KinectInterop.DeleteNativeLib("KinectInteraction180_32.dll", false);
			KinectInterop.DeleteNativeLib("KinectInteraction180_64.dll", false);
			KinectInterop.DeleteNativeLib("FaceTrackLib.dll", false);
			KinectInterop.DeleteNativeLib("FaceTrackData.dll", false);
			KinectInterop.DeleteNativeLib("KinectBackgroundRemoval180_32.dll", false);
			KinectInterop.DeleteNativeLib("KinectBackgroundRemoval180_64.dll", false);
			KinectInterop.DeleteNativeLib("msvcp100.dll", false);
			KinectInterop.DeleteNativeLib("msvcr100.dll", false);
		}
	}

	public bool IsSensorAvailable()
	{
		bool bAvailable = GetSensorsCount() > 0;
		return bAvailable;
	}
	
	public int GetSensorsCount ()
	{
		int iSensorCount = 0;
		int hr = NuiGetSensorCount(out iSensorCount);

		if(hr == 0)
			return iSensorCount;
		else
			return 0;
	}

	public KinectInterop.SensorData OpenDefaultSensor (KinectInterop.FrameSource dwFlags, float sensorAngle, bool bUseMultiSource)
	{
		sourceFlags = dwFlags;

		NuiInitializeFlags nuiFlags = // NuiInitializeFlags.UsesNone;
			NuiInitializeFlags.UsesSkeleton | NuiInitializeFlags.UsesDepthAndPlayerIndex;

		if((dwFlags & KinectInterop.FrameSource.TypeBody) != 0)
		{
			nuiFlags |= NuiInitializeFlags.UsesSkeleton;
		}
		
		if((dwFlags & KinectInterop.FrameSource.TypeColor) != 0)
		{
			nuiFlags |= NuiInitializeFlags.UsesColor;
		}
		
		if((dwFlags & KinectInterop.FrameSource.TypeDepth) != 0)
		{
			nuiFlags |= NuiInitializeFlags.UsesDepthAndPlayerIndex;
		}
		
		if((dwFlags & KinectInterop.FrameSource.TypeBodyIndex) != 0)
		{
			nuiFlags |= NuiInitializeFlags.UsesDepthAndPlayerIndex;
		}
		
		if((dwFlags & KinectInterop.FrameSource.TypeInfrared) != 0)
		{
			nuiFlags |= (NuiInitializeFlags.UsesColor | (NuiInitializeFlags)0x8000);
		}
		
		if((dwFlags & KinectInterop.FrameSource.TypeAudio) != 0)
		{
			nuiFlags |= NuiInitializeFlags.UsesAudio;
		}

		FacetrackingManager[] faceManagers = GameObject.FindObjectsOfType(typeof(FacetrackingManager)) as FacetrackingManager[];
		if(faceManagers != null && faceManagers.Length > 0)
		{
			for(int i = 0; i < faceManagers.Length; i++)
			{
				if(faceManagers[i].enabled)
				{
					//Debug.Log("Found FacetrackingManager => UsesColor");
					nuiFlags |= NuiInitializeFlags.UsesColor;
					break;
				}
			}
		}
		
		SpeechManager[] speechManagers = GameObject.FindObjectsOfType(typeof(SpeechManager)) as SpeechManager[];
		if(speechManagers != null && speechManagers.Length > 0)
		{
			for(int i = 0; i < speechManagers.Length; i++)
			{
				if(speechManagers[i].enabled)
				{
					//Debug.Log("Found SpeechManager => UsesAudio");
					nuiFlags |= NuiInitializeFlags.UsesAudio;
					break;
				}
			}
		}
		
		int hr = InitKinectSensor(nuiFlags, true, (int)Constants.ColorImageResolution, (int)Constants.DepthImageResolution, Constants.IsNearMode);

		if(hr == 0)
		{
			// set sensor angle
			SetKinectElevationAngle((int)sensorAngle);

			// initialize Kinect interaction
			hr = InitKinectInteraction();
			if(hr != 0)
			{
				Debug.LogError(string.Format("Error initializing KinectInteraction: hr=0x{0:X}", hr));
			}
			
			KinectInterop.SensorData sensorData = new KinectInterop.SensorData();

			sensorData.bodyCount = Constants.NuiSkeletonCount;
			sensorData.jointCount = Constants.JointCount;

			sensorData.depthCameraFOV = 46.6f;
			sensorData.colorCameraFOV = 48.6f;
			sensorData.depthCameraOffset = 0.01f;
			sensorData.faceOverlayOffset = 0.01f;

			NuiImageResolutionToSize(Constants.ColorImageResolution, out sensorData.colorImageWidth, out sensorData.colorImageHeight);
//			sensorData.colorImageWidth = Constants.ColorImageWidth;
//			sensorData.colorImageHeight = Constants.ColorImageHeight;

			if((dwFlags & KinectInterop.FrameSource.TypeColor) != 0)
			{
				//colorStreamHandle =  GetColorStreamHandle();
				sensorData.colorImage = new byte[sensorData.colorImageWidth * sensorData.colorImageHeight * 4];
			}

			NuiImageResolutionToSize(Constants.DepthImageResolution, out sensorData.depthImageWidth, out sensorData.depthImageHeight);
//			sensorData.depthImageWidth = Constants.DepthImageWidth;
//			sensorData.depthImageHeight = Constants.DepthImageHeight;
			
			if((dwFlags & KinectInterop.FrameSource.TypeDepth) != 0)
			{
				//depthStreamHandle = GetDepthStreamHandle();
				sensorData.depthImage = new ushort[sensorData.depthImageWidth * sensorData.depthImageHeight];
			}
			
			if((dwFlags & KinectInterop.FrameSource.TypeBodyIndex) != 0)
			{
				sensorData.bodyIndexImage = new byte[sensorData.depthImageWidth * sensorData.depthImageHeight];
			}
			
			if((dwFlags & KinectInterop.FrameSource.TypeInfrared) != 0)
			{
				sensorData.infraredImage = new ushort[sensorData.colorImageWidth * sensorData.colorImageHeight];
			}

			if((dwFlags & KinectInterop.FrameSource.TypeBody) != 0)
			{
				skeletonFrame = new NuiSkeletonFrame() 
				{ 
					SkeletonData = new NuiSkeletonData[Constants.NuiSkeletonCount] 
				};
				
				// default values used to pass to smoothing function
				smoothParameters = new NuiTransformSmoothParameters();

				smoothParameters.fSmoothing = 0.5f;
				smoothParameters.fCorrection = 0.5f;
				smoothParameters.fPrediction = 0.5f;
				smoothParameters.fJitterRadius = 0.05f;
				smoothParameters.fMaxDeviationRadius = 0.04f;
			}
			
			return sensorData;
		}
		else
		{
			Debug.LogError("InitKinectSensor failed: " + GetNuiErrorString(hr));
		}

		return null;
	}

	public void CloseSensor (KinectInterop.SensorData sensorData)
	{
		FinishKinectInteraction();
		ShutdownKinectSensor();
	}

	public bool UpdateSensorData (KinectInterop.SensorData sensorData)
	{
		int hr = UpdateKinectSensor();
		return (hr == 0);
	}

	public bool GetMultiSourceFrame (KinectInterop.SensorData sensorData)
	{
		return false;
	}

	public void FreeMultiSourceFrame (KinectInterop.SensorData sensorData)
	{
	}

	private int NuiSkeletonGetNextFrame(uint dwMillisecondsToWait, ref NuiSkeletonFrame pSkeletonFrame)
	{
		if(sourceFlags != KinectInterop.FrameSource.TypeAudio)
		{
			// non-audio sources
			uint iFrameLength = (uint)GetSkeletonFrameLength();
			bool bSuccess = GetSkeletonFrameData(ref pSkeletonFrame, ref iFrameLength, true);
			return bSuccess ? 0 : -1;
		}
		else
		{
			// audio only
			int hr = GetNextSkeletonFrame(dwMillisecondsToWait);

			if(hr == 0)
			{
				uint iFrameLength = (uint)GetSkeletonFrameLength();
				bool bSuccess = GetSkeletonFrameData(ref pSkeletonFrame, ref iFrameLength, true);
				
				return bSuccess ? 0 : -1;
			}
			
			return hr;
		}
	}

	private void GetHandStateAndConf(uint skeletonId, bool isRightHand, uint handState, InteractionHandEventType handEvent, 
	                                 ref Dictionary<uint, InteractionHandEventType> lastHandEvent,
	                                 ref KinectInterop.HandState refHandState, 
	                                 ref KinectInterop.TrackingConfidence refHandConf)
	{
		bool bHandPrimary = (handState & (uint)NuiHandpointerState.Active) != 0;
		refHandConf = bHandPrimary ? KinectInterop.TrackingConfidence.High : KinectInterop.TrackingConfidence.Low;

		if(!lastHandEvent.ContainsKey(skeletonId))
		{
			lastHandEvent[skeletonId] = InteractionHandEventType.Release;
		}

		if(handEvent == InteractionHandEventType.Grip || 
		   handEvent == InteractionHandEventType.Release)
		{
			if(lastHandEvent[skeletonId] != handEvent)
			{
				//Debug.Log(string.Format("{0} - {1}, event: {2}", skeletonId, !isRightHand ? "left" : "right", handEvent));
				lastHandEvent[skeletonId] = handEvent;
			}
		}
		
		if(bHandPrimary)
		{
			switch(lastHandEvent[skeletonId])
			{
				case InteractionHandEventType.Grip:
					refHandState = KinectInterop.HandState.Closed;
					break;

				case InteractionHandEventType.Release:
					refHandState = KinectInterop.HandState.Open;
					break;
			}
		}
		else
		{
//			if(lastHandEvent[skeletonId] != InteractionHandEventType.Release)
//			{
//				Debug.Log(string.Format("{0} - old: {1}, NONE: {2}", skeletonId, lastHandEvent[skeletonId], InteractionHandEventType.Release));
//				lastHandEvent[skeletonId] = InteractionHandEventType.Release;
//			}

			refHandState = KinectInterop.HandState.NotTracked;
		}
	}

	public bool PollBodyFrame (KinectInterop.SensorData sensorData, ref KinectInterop.BodyFrameData bodyFrame, 
	                           ref Matrix4x4 kinectToWorld, bool bIgnoreJointZ)
	{
		// get next body frame
		int hr = NuiSkeletonGetNextFrame(0, ref skeletonFrame);
		bool newSkeleton = (hr == 0);

		if(newSkeleton)
		{
			bodyFrame.liPreviousTime = bodyFrame.liRelativeTime;
			bodyFrame.liRelativeTime = skeletonFrame.liTimeStamp;

			hr = NuiTransformSmooth(ref skeletonFrame, ref smoothParameters);
			if(hr < 0)
			{
				Debug.LogError("Skeleton Data Smoothing failed");
			}

			for(uint i = 0; i < sensorData.bodyCount; i++)
			{
				NuiSkeletonData body = skeletonFrame.SkeletonData[i];
				
				bodyFrame.bodyData[i].bIsTracked = (short)(body.eTrackingState ==  NuiSkeletonTrackingState.SkeletonTracked ? 1 : 0);
				
				if(body.eTrackingState ==  NuiSkeletonTrackingState.SkeletonTracked)
				{
					// transfer body and joints data
					bodyFrame.bodyData[i].liTrackingID = (long)body.dwTrackingID;
					
					for(int j = 0; j < sensorData.jointCount; j++)
					{
						KinectInterop.JointData jointData = bodyFrame.bodyData[i].joint[j];
						
						//jointData.jointType = (KinectInterop.JointType)j;
						jointData.trackingState = (KinectInterop.TrackingState)body.eSkeletonPositionTrackingState[j];
						
						if(jointData.trackingState != KinectInterop.TrackingState.NotTracked)
						{
							//jointData.kinectPos = body.SkeletonPositions[j];
							float jPosZ = (bIgnoreJointZ && j > 0) ? bodyFrame.bodyData[i].joint[0].kinectPos.z : body.SkeletonPositions[j].z;
							jointData.kinectPos = new Vector3(body.SkeletonPositions[j].x, body.SkeletonPositions[j].y, jPosZ);
							jointData.position = kinectToWorld.MultiplyPoint3x4(jointData.kinectPos);
						}
						
						jointData.orientation = Quaternion.identity;
//							Windows.Kinect.Vector4 vQ = body.JointOrientations[jointData.jointType].Orientation;
//							jointData.orientation = new Quaternion(vQ.X, vQ.Y, vQ.Z, vQ.W);
						
						if(j == 0)
						{
							bodyFrame.bodyData[i].position = jointData.position;
							bodyFrame.bodyData[i].orientation = jointData.orientation;
						}
						
						bodyFrame.bodyData[i].joint[j] = jointData;
					}


					// tranfer hand states
					uint intCount = GetInteractorsCount();
					string sDebug = string.Empty;

					for(uint intIndex = 0; intIndex < intCount; intIndex++)
					{
						uint skeletonId = GetSkeletonTrackingID(intIndex);

						if(skeletonId == body.dwTrackingID)
						{
							uint leftHandState = GetLeftHandState(intIndex);
							InteractionHandEventType leftHandEvent = GetLeftHandEvent(intIndex);

							uint rightHandState = GetRightHandState(intIndex);
							InteractionHandEventType rightHandEvent = GetRightHandEvent(intIndex);
							
//							sDebug += string.Format("{0}-L:{1},{2};R:{3},{4}    ", skeletonId, 
//							                        (int)leftHandState, (int)leftHandEvent, 
//							                        (int)rightHandState, (int)rightHandEvent);
							
							GetHandStateAndConf(skeletonId, false, leftHandState, leftHandEvent, 
							                    ref lastLeftHandEvent,
							                    ref bodyFrame.bodyData[i].leftHandState, 
							                    ref bodyFrame.bodyData[i].leftHandConfidence);
							
							GetHandStateAndConf(skeletonId, true, rightHandState, rightHandEvent, 
							                    ref lastRightHandEvent,
							                    ref bodyFrame.bodyData[i].rightHandState, 
							                    ref bodyFrame.bodyData[i].rightHandConfidence);
						}
					}

					if(intCount > 0 && sDebug.Length > 0)
					{
						Debug.Log(sDebug);
					}
				}
			}

			if(sensorData.hintHeightAngle)
			{
				// get the floor plane
				Vector4 vFloorPlane = skeletonFrame.vFloorClipPlane;
				Vector3 floorPlane = new Vector3(vFloorPlane.x, vFloorPlane.y, vFloorPlane.z);
				
				sensorData.sensorRotDetected = Quaternion.FromToRotation(floorPlane, Vector3.up);
				sensorData.sensorHgtDetected = vFloorPlane.w;
			}
		}
		
		return newSkeleton;
	}

	public bool PollColorFrame (KinectInterop.SensorData sensorData)
	{
		uint videoBufLen = (uint)sensorData.colorImage.Length;
		
		var pColorData = GCHandle.Alloc(sensorData.colorImage, GCHandleType.Pinned);
		bool newColor = GetColorFrameData(pColorData.AddrOfPinnedObject(), ref videoBufLen, true);
		pColorData.Free();
		
		if (newColor)
		{
			for (int i = 0; i < videoBufLen; i += 4)
			{
				byte btTmp = sensorData.colorImage[i];
				sensorData.colorImage[i] = sensorData.colorImage[i + 2];
				sensorData.colorImage[i + 2] = btTmp;
				sensorData.colorImage[i + 3] = 255;
			}
		}

		return newColor;
	}

	public bool PollDepthFrame (KinectInterop.SensorData sensorData)
	{
		uint depthBufLen = (uint)(sensorData.depthImage.Length * sizeof(ushort));
		
		var pDepthData = GCHandle.Alloc(sensorData.depthImage, GCHandleType.Pinned);
		bool newDepth = GetDepthFrameData(pDepthData.AddrOfPinnedObject(), ref depthBufLen, true);
		pDepthData.Free();

		if(newDepth)
		{
			uint depthLen = (uint)sensorData.depthImage.Length;

			for (int i = 0; i < depthLen; i++)
			{
				if((sensorData.depthImage[i] & 7) != 0)
					sensorData.bodyIndexImage[i] = (byte)((sensorData.depthImage[i] & 7) - 1);
				else
					sensorData.bodyIndexImage[i] = 255;

				sensorData.depthImage[i] = (ushort)(sensorData.depthImage[i] >> 3);
			}
		}

		return newDepth;
	}

	public bool PollInfraredFrame (KinectInterop.SensorData sensorData)
	{
		uint infraredBufLen = (uint)(sensorData.infraredImage.Length * sizeof(ushort));
		
		var pInfraredData = GCHandle.Alloc(sensorData.infraredImage, GCHandleType.Pinned);
		bool newInfrared = GetInfraredFrameData(pInfraredData.AddrOfPinnedObject(), ref infraredBufLen, true);
		pInfraredData.Free();
		
		return newInfrared;
	}

	public void FixJointOrientations(KinectInterop.SensorData sensorData, ref KinectInterop.BodyData bodyData)
	{
		// fix the hips-to-spine tilt (it is about 40 degrees to the back)
		int hipsIndex = (int)KinectInterop.JointType.SpineBase;

		Quaternion quat = bodyData.joint[hipsIndex].normalRotation;
		quat *= Quaternion.Euler(40f, 0f, 0f);
		bodyData.joint[hipsIndex].normalRotation = quat;

		Vector3 mirroredAngles = quat.eulerAngles;
		mirroredAngles.y = -mirroredAngles.y;
		mirroredAngles.z = -mirroredAngles.z;
		bodyData.joint[hipsIndex].mirroredRotation = Quaternion.Euler(mirroredAngles);

		bodyData.normalRotation = bodyData.joint[hipsIndex].normalRotation;
		bodyData.mirroredRotation = bodyData.joint[hipsIndex].mirroredRotation;
	}

	public bool IsBodyTurned(ref KinectInterop.BodyData bodyData)
	{
		return false;
	}
	
	private void NuiTransformSkeletonToDepthImage(Vector3 vPoint, out float pfDepthX, out float pfDepthY, out float pfDepthZ)
	{
		if (vPoint.z > float.Epsilon)
		{
			pfDepthX = 0.5f + ((vPoint.x * 285.63f) / (vPoint.z * 320f));
			pfDepthY = 0.5f - ((vPoint.y * 285.63f) / (vPoint.z * 240f));
			pfDepthZ = vPoint.z * 1000f;
		}
		else
		{
			pfDepthX = 0f;
			pfDepthY = 0f;
			pfDepthZ = 0f;
		}
	}
	
	public Vector2 MapSpacePointToDepthCoords (KinectInterop.SensorData sensorData, Vector3 spacePos)
	{
		float fDepthX, fDepthY, fDepthZ;
		NuiTransformSkeletonToDepthImage(spacePos, out fDepthX, out fDepthY, out fDepthZ);
		
		fDepthX = Mathf.RoundToInt(fDepthX * sensorData.depthImageWidth);
		fDepthY = Mathf.RoundToInt(fDepthY * sensorData.depthImageHeight);
		fDepthZ = Mathf.RoundToInt(fDepthZ);

		Vector3 point = new Vector3(fDepthX, fDepthY, fDepthZ);

		return point;
	}

	private Vector3 NuiTransformDepthImageToSkeleton(float fDepthX, float fDepthY, int depthValue)
	{
		Vector3 point = Vector3.zero;
		
		if (depthValue > 0)
		{
			float fSpaceZ = ((float)depthValue) / 1000f;
			float fSpaceX = ((fDepthX - 0.5f) * (0.003501f * fSpaceZ)) * 320f;
			float fSpaceY = ((0.5f - fDepthY) * (0.003501f * fSpaceZ)) * 240f;
			
			point = new Vector3(fSpaceX, fSpaceY, fSpaceZ);
		}
		
		return point;
	}
	
	public Vector3 MapDepthPointToSpaceCoords (KinectInterop.SensorData sensorData, Vector2 depthPos, ushort depthVal)
	{
		float fDepthX = depthPos.x / sensorData.depthImageWidth;
		float fDepthY = depthPos.y / sensorData.depthImageHeight;
		
		Vector3 point = NuiTransformDepthImageToSkeleton(fDepthX, fDepthY, depthVal);
		
		return point;
	}

	public Vector2 MapDepthPointToColorCoords (KinectInterop.SensorData sensorData, Vector2 depthPos, ushort depthVal)
	{
		int cx, cy;
		NuiImageGetColorPixelCoordinatesFromDepthPixelAtResolution(
			Constants.ColorImageResolution,
			Constants.DepthImageResolution,
			ref pcViewArea,
			(int)depthPos.x, (int)depthPos.y, (ushort)(depthVal << 3),
			out cx, out cy);
		
		return new Vector2(cx, cy);
	}

	public bool MapDepthFrameToColorCoords (KinectInterop.SensorData sensorData, ref Vector2[] vColorCoords)
	{
		// comment out this block of code, if you experience big lags in FPS (it is used for creating cut-out texture of the users)
		if(sensorData.depthImage != null && sensorData.colorImage != null)
		{
			int i = 0, cx = 0, cy = 0;
			
			for(int y = 0; y < sensorData.depthImageHeight; y++)
			{
				for(int x = 0; x < sensorData.depthImageWidth; x++)
				{
					ushort dv = sensorData.depthImage[i];

					NuiImageGetColorPixelCoordinatesFromDepthPixelAtResolution(
						Constants.ColorImageResolution,
						Constants.DepthImageResolution,
						ref pcViewArea,
						x, y, (ushort)(dv << 3),
						out cx, out cy);

					vColorCoords[i] = new Vector2(cx, cy);
					i++;
				}
			}
			
			return true;
		}

		return false;
	}

	public bool MapColorFrameToDepthCoords (KinectInterop.SensorData sensorData, ref Vector2[] vDepthCoords)
	{
		return false;
	}
	
	// returns the index of the given joint in joint's array or -1 if joint is not applicable
	public int GetJointIndex(KinectInterop.JointType joint)
	{
		switch(joint)
		{
			case KinectInterop.JointType.SpineBase:
				return (int)NuiSkeletonPositionIndex.HipCenter;
			case KinectInterop.JointType.SpineMid:
				return (int)NuiSkeletonPositionIndex.Spine;
			case KinectInterop.JointType.SpineShoulder:
			case KinectInterop.JointType.Neck:
				return (int)NuiSkeletonPositionIndex.ShoulderCenter;
			case KinectInterop.JointType.Head:
				return (int)NuiSkeletonPositionIndex.Head;
				
			case KinectInterop.JointType.ShoulderLeft:
				return (int)NuiSkeletonPositionIndex.ShoulderLeft;
			case KinectInterop.JointType.ElbowLeft:
				return (int)NuiSkeletonPositionIndex.ElbowLeft;
			case KinectInterop.JointType.WristLeft:
				return (int)NuiSkeletonPositionIndex.WristLeft;
			case KinectInterop.JointType.HandLeft:
				return (int)NuiSkeletonPositionIndex.HandLeft;
				
			case KinectInterop.JointType.ShoulderRight:
				return (int)NuiSkeletonPositionIndex.ShoulderRight;
			case KinectInterop.JointType.ElbowRight:
				return (int)NuiSkeletonPositionIndex.ElbowRight;
			case KinectInterop.JointType.WristRight:
				return (int)NuiSkeletonPositionIndex.WristRight;
			case KinectInterop.JointType.HandRight:
				return (int)NuiSkeletonPositionIndex.HandRight;
				
			case KinectInterop.JointType.HipLeft:
				return (int)NuiSkeletonPositionIndex.HipLeft;
			case KinectInterop.JointType.KneeLeft:
				return (int)NuiSkeletonPositionIndex.KneeLeft;
			case KinectInterop.JointType.AnkleLeft:
				return (int)NuiSkeletonPositionIndex.AnkleLeft;
			case KinectInterop.JointType.FootLeft:
				return (int)NuiSkeletonPositionIndex.FootLeft;
				
			case KinectInterop.JointType.HipRight:
				return (int)NuiSkeletonPositionIndex.HipRight;
			case KinectInterop.JointType.KneeRight:
				return (int)NuiSkeletonPositionIndex.KneeRight;
			case KinectInterop.JointType.AnkleRight:
				return (int)NuiSkeletonPositionIndex.AnkleRight;
			case KinectInterop.JointType.FootRight:
				return (int)NuiSkeletonPositionIndex.FootRight;
		}
		
		return -1;
	}

//	// returns the joint at given index
//	public KinectInterop.JointType GetJointAtIndex(int index)
//	{
//		switch(index)
//		{
//		case (int)NuiSkeletonPositionIndex.HipCenter:
//			return KinectInterop.JointType.SpineBase;
//		case (int)NuiSkeletonPositionIndex.Spine:
//			return KinectInterop.JointType.SpineMid;
//		case (int)NuiSkeletonPositionIndex.ShoulderCenter:
//			return KinectInterop.JointType.Neck;
//		case (int)NuiSkeletonPositionIndex.Head:
//			return KinectInterop.JointType.Head;
//			
//		case (int)NuiSkeletonPositionIndex.ShoulderLeft:
//			return KinectInterop.JointType.ShoulderLeft;
//		case (int)NuiSkeletonPositionIndex.ElbowLeft:
//			return KinectInterop.JointType.ElbowLeft;
//		case (int)NuiSkeletonPositionIndex.WristLeft:
//			return KinectInterop.JointType.WristLeft;
//		case (int)NuiSkeletonPositionIndex.HandLeft:
//			return KinectInterop.JointType.HandLeft;
//			
//		case (int)NuiSkeletonPositionIndex.ShoulderRight:
//			return KinectInterop.JointType.ShoulderRight;
//		case (int)NuiSkeletonPositionIndex.ElbowRight:
//			return KinectInterop.JointType.ElbowRight;
//		case (int)NuiSkeletonPositionIndex.WristRight:
//			return KinectInterop.JointType.WristRight;
//		case (int)NuiSkeletonPositionIndex.HandRight:
//			return KinectInterop.JointType.HandRight;
//			
//		case (int)NuiSkeletonPositionIndex.HipLeft:
//			return KinectInterop.JointType.HipLeft;
//		case (int)NuiSkeletonPositionIndex.KneeLeft:
//			return KinectInterop.JointType.KneeLeft;
//		case (int)NuiSkeletonPositionIndex.AnkleLeft:
//			return KinectInterop.JointType.AnkleLeft;
//		case (int)NuiSkeletonPositionIndex.FootLeft:
//			return KinectInterop.JointType.FootLeft;
//			
//		case (int)NuiSkeletonPositionIndex.HipRight:
//			return KinectInterop.JointType.HipRight;
//		case (int)NuiSkeletonPositionIndex.KneeRight:
//			return KinectInterop.JointType.KneeRight;
//		case (int)NuiSkeletonPositionIndex.AnkleRight:
//			return KinectInterop.JointType.AnkleRight;
//		case (int)NuiSkeletonPositionIndex.FootRight:
//			return KinectInterop.JointType.FootRight;
//		}
//		
//		return (KinectInterop.JointType)(-1);
//	}

	// returns the parent joint of the given joint
	public KinectInterop.JointType GetParentJoint(KinectInterop.JointType joint)
	{
		switch(joint)
		{
			case KinectInterop.JointType.SpineBase:
				return KinectInterop.JointType.SpineBase;
				
			case KinectInterop.JointType.ShoulderLeft:
			case KinectInterop.JointType.ShoulderRight:
				return KinectInterop.JointType.Neck;
				
			case KinectInterop.JointType.HipLeft:
			case KinectInterop.JointType.HipRight:
				return KinectInterop.JointType.SpineBase;
		}
		
		return (KinectInterop.JointType)((int)joint - 1);
	}
	
	// returns the next joint in the hierarchy, as to the given joint
	public KinectInterop.JointType GetNextJoint(KinectInterop.JointType joint)
	{
		switch(joint)
		{
			case KinectInterop.JointType.SpineBase:
				return KinectInterop.JointType.SpineMid;
			case KinectInterop.JointType.SpineMid:
				return KinectInterop.JointType.Neck;
			case KinectInterop.JointType.Neck:
				return KinectInterop.JointType.Head;
				
			case KinectInterop.JointType.ShoulderLeft:
				return KinectInterop.JointType.ElbowLeft;
			case KinectInterop.JointType.ElbowLeft:
				return KinectInterop.JointType.WristLeft;
			case KinectInterop.JointType.WristLeft:
				return KinectInterop.JointType.HandLeft;
				
			case KinectInterop.JointType.ShoulderRight:
				return KinectInterop.JointType.ElbowRight;
			case KinectInterop.JointType.ElbowRight:
				return KinectInterop.JointType.WristRight;
			case KinectInterop.JointType.WristRight:
				return KinectInterop.JointType.HandRight;
				
			case KinectInterop.JointType.HipLeft:
				return KinectInterop.JointType.KneeLeft;
			case KinectInterop.JointType.KneeLeft:
				return KinectInterop.JointType.AnkleLeft;
			case KinectInterop.JointType.AnkleLeft:
				return KinectInterop.JointType.FootLeft;
				
			case KinectInterop.JointType.HipRight:
				return KinectInterop.JointType.KneeRight;
			case KinectInterop.JointType.KneeRight:
				return KinectInterop.JointType.AnkleRight;
			case KinectInterop.JointType.AnkleRight:
				return KinectInterop.JointType.FootRight;
		}
		
		return joint;  // in case of end joint - Head, HandLeft, HandRight, FootLeft, FootRight
	}

	public bool IsFaceTrackingAvailable(ref bool bNeedRestart)
	{
		bNeedRestart = false;
		return true;
	}
	
	public bool InitFaceTracking(bool bUseFaceModel, bool bDrawFaceRect)
	{
		isUseFaceModel = bUseFaceModel;
		isDrawFaceRect = bDrawFaceRect;

		int hr = InitFaceTrackingNative();
		if(hr < 0)
		{
			Debug.LogError(string.Format("Error initializing Facetracker: hr=0x{0:X}", hr));
		}

		bFaceTrackingInited = (hr >= 0);

		return bFaceTrackingInited;
	}
	
	public void FinishFaceTracking()
	{
		FinishFaceTrackingNative();
		bFaceTrackingInited = false;
	}

	public bool UpdateFaceTracking()
	{
		int hr = UpdateFaceTrackingNative();
		return (hr >= 0);
	}

	public bool IsFaceTrackingActive()
	{
		return bFaceTrackingInited;
	}

	public bool IsDrawFaceRect()
	{
		return isDrawFaceRect;
	}
	
	public bool IsFaceTracked(long userId)
	{
		if(GetFaceTrackingIDNative() == userId)
		{
			return IsFaceTrackedNative();
		}

		return false;
	}
	
	public bool GetFaceRect(long userId, ref Rect rectFace)
	{
		if(GetFaceTrackingIDNative() == userId)
		{
			if(GetFaceRectNative(ref faceRect))
			{
				rectFace.x = faceRect.x;
				rectFace.y = faceRect.y;
				rectFace.width = faceRect.width;
				rectFace.height = faceRect.height;

				return true;
			}
		}
		
		return false;
	}

	public void VisualizeFaceTrackerOnColorTex(Texture2D texColor)
	{
		if(bFaceTrackingInited)
		{
			Rect faceRect = new Rect();

			if(GetFaceRect(GetFaceTrackingIDNative(), ref faceRect))
			{
				Color color = Color.magenta;
				Vector2 pt1, pt2;
				
				// bottom
				pt1.x = faceRect.x; pt1.y = faceRect.y;
				pt2.x = faceRect.x + faceRect.width - 1; pt2.y = pt1.y;
				DrawLine(texColor, pt1, pt2, color);
				
				// right
				pt1.x = pt2.x; pt1.y = pt2.y;
				pt2.x = pt1.x; pt2.y = faceRect.y + faceRect.height - 1;
				DrawLine(texColor, pt1, pt2, color);
				
				// top
				pt1.x = pt2.x; pt1.y = pt2.y;
				pt2.x = faceRect.x; pt2.y = pt1.y;
				DrawLine(texColor, pt1, pt2, color);
				
				// left
				pt1.x = pt2.x; pt1.y = pt2.y;
				pt2.x = pt1.x; pt2.y = faceRect.y;
				DrawLine(texColor, pt1, pt2, color);
			}
		}
	}

	private void DrawLine(Texture2D a_Texture, Vector2 ptStart, Vector2 ptEnd, Color a_Color)
	{
//		int width = a_Texture.width;
//		int height = a_Texture.height;
//		
//		KinectInterop.DrawLine(a_Texture, width - (int)ptStart.x, height - (int)ptStart.y, 
//		    width - (int)ptEnd.x, height - (int)ptEnd.y, a_Color);
		KinectInterop.DrawLine(a_Texture, (int)ptStart.x, (int)ptStart.y, (int)ptEnd.x, (int)ptEnd.y, a_Color);
	}

	public bool GetHeadPosition(long userId, ref Vector3 headPos)
	{
		if(GetFaceTrackingIDNative() == userId)
		{
			if(GetHeadPositionNative(ref vHeadPos))
			{
				headPos = vHeadPos;
				return true;
			}
		}
		
		return false;
	}
	
	public bool GetHeadRotation(long userId, ref Quaternion headRot)
	{
		if(GetFaceTrackingIDNative() == userId)
		{
			if(GetHeadRotationNative(ref vHeadRot))
			{
				headRot = Quaternion.Euler((Vector3)vHeadRot);
				return true;
			}
		}
		
		return false;
	}

	public bool GetAnimUnits(long userId, ref Dictionary<KinectInterop.FaceShapeAnimations, float> dictAU)
	{
		if(isUseFaceModel && dictAU != null && GetFaceTrackingIDNative() == userId)
		{
			int iAUCount = GetAnimUnitsCountNative();

			if(afAU == null || afAU.Length == 0)
			{
				afAU = new float[iAUCount];
			}

			var pArrayData = GCHandle.Alloc(afAU, GCHandleType.Pinned);
			bool bSuccess = GetAnimUnitsNative(pArrayData.AddrOfPinnedObject(), ref iAUCount);
			pArrayData.Free();

			if(iAUCount >= 6)
			{
				//Debug.Log(string.Format("0:{0:F2} | 1:{1:F2} | 2:{2:F2} | 3:{3:F2} | 4:{4:F2} | 5:{5:F2}", afAU[0], afAU[1], afAU[2], afAU[3], afAU[4], afAU[5]));

				dictAU[KinectInterop.FaceShapeAnimations.LipPucker] = afAU[0];  // AU0 - Upper Lip Raiser
				dictAU[KinectInterop.FaceShapeAnimations.JawOpen] = afAU[1];  // AU1 - Jaw Lowerer

				dictAU[KinectInterop.FaceShapeAnimations.LipStretcherLeft] = afAU[2];  // AU2 – Lip Stretcher
				dictAU[KinectInterop.FaceShapeAnimations.LipStretcherRight] = afAU[2];  // AU2 – Lip Stretcher

				dictAU[KinectInterop.FaceShapeAnimations.LefteyebrowLowerer] = afAU[3] - afAU[5];  // AU3 – Brow Lowerer
				dictAU[KinectInterop.FaceShapeAnimations.RighteyebrowLowerer] = afAU[3] - afAU[5];  // AU3 – Brow Lowerer

				dictAU[KinectInterop.FaceShapeAnimations.LipCornerDepressorLeft] = afAU[4];  // AU4 – Lip Corner Depressor
				dictAU[KinectInterop.FaceShapeAnimations.LipCornerDepressorRight] = afAU[4];  // AU4 – Lip Corner Depressor

				if(iAUCount >= 7)
				{
					dictAU[KinectInterop.FaceShapeAnimations.LefteyeClosed] = afAU[6];  // AU6, AU7 – Eyelid closed
					dictAU[KinectInterop.FaceShapeAnimations.RighteyeClosed] = afAU[6];  // AU6, AU7 – Eyelid closed
				}
			}

			return bSuccess;
		}

		return false;
	}
	
	public bool GetShapeUnits(long userId, ref Dictionary<KinectInterop.FaceShapeDeformations, float> dictSU)
	{
		if(isUseFaceModel && dictSU != null && GetFaceTrackingIDNative() == userId)
		{
			int iSUCount = GetShapeUnitsCountNative();
			
			if(afSU == null || afSU.Length == 0)
			{
				afSU = new float[iSUCount];
			}
			
			var pArrayData = GCHandle.Alloc(afSU, GCHandleType.Pinned);
			bool bSuccess = GetShapeUnitsNative(pArrayData.AddrOfPinnedObject(), ref iSUCount);
			pArrayData.Free();

			if(iSUCount >= 11)
			{
				// here we must convert the old SUs to the new ones, but there is no info about this kind of conversation
			}
			
			return bSuccess;
		}
		
		return false;
	}
	
	public int GetFaceModelVerticesCount(long userId)
	{
		if(isUseFaceModel && GetFaceTrackingIDNative() == userId || userId == 0)
		{
			return GetModelPointsCountNative();
		}

		return 0;
	}
	
	public bool GetFaceModelVertices(long userId, ref Vector3[] avVertices)
	{
		if(isUseFaceModel && avVertices != null && (GetFaceTrackingIDNative() == userId || userId == 0))
		{
			int iPointsCount = avVertices.Length;
			
			var pArrayData = GCHandle.Alloc(avVertices, GCHandleType.Pinned);
			bool bSuccess = GetModelPointsNative(pArrayData.AddrOfPinnedObject(), ref iPointsCount);
			pArrayData.Free();

//			for(int i = 0; i < avVertices.Length; i++)
//			{
//				avVertices[i].z = -avVertices[i].z;
//			}
			
			return bSuccess;
		}
		
		return false;
	}
	
	public int GetFaceModelTrianglesCount()
	{
		return isUseFaceModel ? GetTriangleCountNative() : 0;
	}
	
	public bool GetFaceModelTriangles(bool bMirrored, ref int[] avTriangles)
	{
		if(isUseFaceModel && avTriangles != null)
		{
			int iTriangleCount = avTriangles.Length;
			
			var pArrayData = GCHandle.Alloc(avTriangles, GCHandleType.Pinned);
			bool bSuccess = GetTrianglesNative(pArrayData.AddrOfPinnedObject(), ref iTriangleCount);
			pArrayData.Free();
			
			if(bMirrored)
			{
				Array.Reverse(avTriangles);
			}
			
			return bSuccess;
		}
		
		return false;
	}

	public bool IsSpeechRecognitionAvailable(ref bool bNeedRestart)
	{
		bNeedRestart = false;
		return true;
	}
	
	public int InitSpeechRecognition(string sRecoCriteria, bool bUseKinect, bool bAdaptationOff)
	{
		return InitSpeechRecognizerNative(sRecoCriteria, bUseKinect, bAdaptationOff);
	}
	
	public void FinishSpeechRecognition()
	{
		FinishSpeechRecognizerNative();
	}
	
	public int UpdateSpeechRecognition()
	{
		return UpdateSpeechRecognizerNative();
	}
	
	public int LoadSpeechGrammar(string sFileName, short iLangCode, bool bDynamic)
	{
		return LoadSpeechGrammarNative(sFileName, iLangCode, bDynamic);
	}
	
	public int AddGrammarPhrase(string sFromRule, string sToRule, string sPhrase, bool bClearRulePhrases, bool bCommitGrammar)
	{
		return AddGrammarPhraseNative(sFromRule, sToRule, sPhrase, bClearRulePhrases, bCommitGrammar);
	}
	
	public void SetSpeechConfidence(float fConfidence)
	{
		SetSpeechConfidenceNative(fConfidence);
	}
	
	public bool IsSpeechStarted()
	{
		return IsSpeechStartedNative();
	}
	
	public bool IsSpeechEnded()
	{
		return IsSpeechEndedNative();
	}
	
	public bool IsPhraseRecognized()
	{
		return IsPhraseRecognizedNative();
	}
	
	public float GetPhraseConfidence()
	{
		return GetPhraseConfidenceNative();
	}
	
	public string GetRecognizedPhraseTag()
	{
		IntPtr pPhraseTag = GetRecognizedPhraseTagNative();
		string sPhraseTag = Marshal.PtrToStringUni(pPhraseTag);
		
		return sPhraseTag;
	}
	
	public void ClearRecognizedPhrase()
	{
		ClearRecognizedPhraseNative();
	}
	
	public bool IsBackgroundRemovalAvailable(ref bool bNeedRestart)
	{
		bNeedRestart = false;
		return true;
	}
	
	public bool InitBackgroundRemoval(KinectInterop.SensorData sensorData, bool isHiResPrefered)
	{
		int hr = InitBackgroundRemovalNative();

		if(hr < 0)
		{
			Debug.LogError(string.Format("Error initializing BackgroundRemoval: hr=0x{0:X}", hr));
		}

		bBackgroundRemovalInited = (hr >= 0);

		return bBackgroundRemovalInited;
	}
	
	public void FinishBackgroundRemoval(KinectInterop.SensorData sensorData)
	{
		FinishBackgroundRemovalNative();
		bBackgroundRemovalInited = false;
	}
	
	public bool UpdateBackgroundRemoval(KinectInterop.SensorData sensorData, bool isHiResPrefered, Color32 defaultColor)
	{
		int hr = UpdateBackgroundRemovalNative();
		return (hr >= 0);
	}

	public bool IsBackgroundRemovalActive()
	{
		return bBackgroundRemovalInited;
	}

	public bool IsBRHiResSupported()
	{
		return false;
	}
	
	public Rect GetForegroundFrameRect(KinectInterop.SensorData sensorData, bool isHiResPrefered)
	{
		return new Rect(0f, 0f, sensorData.depthImageWidth, sensorData.depthImageHeight);
	}
	
	public int GetForegroundFrameLength(KinectInterop.SensorData sensorData, bool isHiResPrefered)
	{
		return GetBackgroundRemovalFrameLengthNative();
	}
	
	public bool PollForegroundFrame(KinectInterop.SensorData sensorData, bool isHiResPrefered, Color32 defaultColor, bool bLimitedUsers, ICollection<int> alTrackedIndexes, ref byte[] foregroundImage)
	{
		uint frameLen = (uint)foregroundImage.Length;
		
		var pFrameData = GCHandle.Alloc(foregroundImage, GCHandleType.Pinned);
		bool newFrame = GetBackgroundRemovalFrameDataNative(pFrameData.AddrOfPinnedObject(), ref frameLen, true);
		pFrameData.Free();
		
		return newFrame;
	}

}
