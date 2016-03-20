using UnityEngine;
using System.Collections;
using Windows.Kinect;
using System.Runtime.InteropServices;
using Microsoft.Kinect.Face;
using System.Collections.Generic;
using System;

public class Kinect2Interface : DepthSensorInterface
{
	// change this to false, if you aren't using Kinect-v2 only and want KM to check for available sensors
	public static bool sensorAlwaysAvailable = true;

	private KinectInterop.FrameSource sensorFlags;
	public KinectSensor kinectSensor;
	public CoordinateMapper coordMapper;
	
	private BodyFrameReader bodyFrameReader;
	private BodyIndexFrameReader bodyIndexFrameReader;
	private ColorFrameReader colorFrameReader;
	private DepthFrameReader depthFrameReader;
	private InfraredFrameReader infraredFrameReader;
	
	private MultiSourceFrameReader multiSourceFrameReader;
	private MultiSourceFrame multiSourceFrame;

	private BodyFrame msBodyFrame = null;
	private BodyIndexFrame msBodyIndexFrame = null;
	private ColorFrame msColorFrame = null;
	private DepthFrame msDepthFrame = null;
	private InfraredFrame msInfraredFrame = null;

	private int bodyCount;
	private Body[] bodyData;

	private bool bFaceTrackingInited = false;
	public FaceFrameSource[] faceFrameSources = null;
	public FaceFrameReader[] faceFrameReaders = null;
	public FaceFrameResult[] faceFrameResults = null;

//	private int faceDisplayWidth;
//	private int faceDisplayHeight;

	private bool isDrawFaceRect = false;
	public HighDefinitionFaceFrameSource[] hdFaceFrameSources = null;
	public HighDefinitionFaceFrameReader[] hdFaceFrameReaders = null;
	public FaceAlignment[] hdFaceAlignments = null;
	public FaceModel[] hdFaceModels = null;

	private bool bBackgroundRemovalInited = false;


	// DLL Imports for speech wrapper functions
	[DllImport("Kinect2SpeechWrapper", EntryPoint = "InitSpeechRecognizer")]
	private static extern int InitSpeechRecognizerNative([MarshalAs(UnmanagedType.LPWStr)]string sRecoCriteria, bool bUseKinect, bool bAdaptationOff);

	[DllImport("Kinect2SpeechWrapper", EntryPoint = "FinishSpeechRecognizer")]
	private static extern void FinishSpeechRecognizerNative();

	[DllImport("Kinect2SpeechWrapper", EntryPoint = "UpdateSpeechRecognizer")]
	private static extern int UpdateSpeechRecognizerNative();
	
	[DllImport("Kinect2SpeechWrapper", EntryPoint = "LoadSpeechGrammar")]
	private static extern int LoadSpeechGrammarNative([MarshalAs(UnmanagedType.LPWStr)]string sFileName, short iNewLangCode, bool bDynamic);

	[DllImport("Kinect2SpeechWrapper", EntryPoint = "AddGrammarPhrase")]
	private static extern int AddGrammarPhraseNative([MarshalAs(UnmanagedType.LPWStr)]string sFromRule, [MarshalAs(UnmanagedType.LPWStr)]string sToRule, [MarshalAs(UnmanagedType.LPWStr)]string sPhrase, bool bClearRule, bool bCommitGrammar);

	[DllImport("Kinect2SpeechWrapper", EntryPoint = "SetRequiredConfidence")]
	private static extern void SetSpeechConfidenceNative(float fConfidence);
	
	[DllImport("Kinect2SpeechWrapper", EntryPoint = "IsSoundStarted")]
	private static extern bool IsSpeechStartedNative();

	[DllImport("Kinect2SpeechWrapper", EntryPoint = "IsSoundEnded")]
	private static extern bool IsSpeechEndedNative();

	[DllImport("Kinect2SpeechWrapper", EntryPoint = "IsPhraseRecognized")]
	private static extern bool IsPhraseRecognizedNative();

	[DllImport("Kinect2SpeechWrapper", EntryPoint = "GetPhraseConfidence")]
	private static extern float GetPhraseConfidenceNative();

	[DllImport("Kinect2SpeechWrapper", EntryPoint = "GetRecognizedTag")]
	private static extern IntPtr GetRecognizedPhraseTagNative();

	[DllImport("Kinect2SpeechWrapper", EntryPoint = "ClearPhraseRecognized")]
	private static extern void ClearRecognizedPhraseNative();
	

	public KinectInterop.DepthSensorPlatform GetSensorPlatform()
	{
		return KinectInterop.DepthSensorPlatform.KinectSDKv2;
	}
	
	public bool InitSensorInterface (bool bCopyLibs, ref bool bNeedRestart)
	{
		bool bOneCopied = false, bAllCopied = true;
		string sTargetPath = KinectInterop.GetTargetDllPath(".", KinectInterop.Is64bitArchitecture()) + "/";

		if(!bCopyLibs)
		{
			// check if the native library is there
			string sTargetLib = sTargetPath + "KinectUnityAddin.dll";
			bNeedRestart = false;

			string sZipFileName = !KinectInterop.Is64bitArchitecture() ? "KinectV2UnityAddin.x86.zip" : "KinectV2UnityAddin.x64.zip";
			long iTargetSize = KinectInterop.GetUnzippedEntrySize(sZipFileName, "KinectUnityAddin.dll");
			
			System.IO.FileInfo targetFile = new System.IO.FileInfo(sTargetLib);
			return targetFile.Exists && targetFile.Length == iTargetSize;
		}
		
		if(!KinectInterop.Is64bitArchitecture())
		{
			Debug.Log("x32-architecture detected.");

			//KinectInterop.CopyResourceFile(sTargetPath + "KinectUnityAddin.dll", "KinectUnityAddin.dll", ref bOneCopied, ref bAllCopied);

			Dictionary<string, string> dictFilesToUnzip = new Dictionary<string, string>();
			dictFilesToUnzip["KinectUnityAddin.dll"] = sTargetPath + "KinectUnityAddin.dll";
			dictFilesToUnzip["Kinect20.Face.dll"] = sTargetPath + "Kinect20.Face.dll";
			dictFilesToUnzip["KinectFaceUnityAddin.dll"] = sTargetPath + "KinectFaceUnityAddin.dll";
			dictFilesToUnzip["Kinect2SpeechWrapper.dll"] = sTargetPath + "Kinect2SpeechWrapper.dll";
			dictFilesToUnzip["Kinect20.VisualGestureBuilder.dll"] = sTargetPath + "Kinect20.VisualGestureBuilder.dll";
			dictFilesToUnzip["KinectVisualGestureBuilderUnityAddin.dll"] = sTargetPath + "KinectVisualGestureBuilderUnityAddin.dll";
			dictFilesToUnzip["vgbtechs/AdaBoostTech.dll"] = sTargetPath + "vgbtechs/AdaBoostTech.dll";
			dictFilesToUnzip["vgbtechs/RFRProgressTech.dll"] = sTargetPath + "vgbtechs/RFRProgressTech.dll";
			dictFilesToUnzip["msvcp110.dll"] = sTargetPath + "msvcp110.dll";
			dictFilesToUnzip["msvcr110.dll"] = sTargetPath + "msvcr110.dll";

			KinectInterop.UnzipResourceFiles(dictFilesToUnzip, "KinectV2UnityAddin.x86.zip", ref bOneCopied, ref bAllCopied);
		}
		else
		{
			Debug.Log("x64-architecture detected.");

			//KinectInterop.CopyResourceFile(sTargetPath + "KinectUnityAddin.dll", "KinectUnityAddin.dll.x64", ref bOneCopied, ref bAllCopied);
			
			Dictionary<string, string> dictFilesToUnzip = new Dictionary<string, string>();
			dictFilesToUnzip["KinectUnityAddin.dll"] = sTargetPath + "KinectUnityAddin.dll";
			dictFilesToUnzip["Kinect20.Face.dll"] = sTargetPath + "Kinect20.Face.dll";
			dictFilesToUnzip["KinectFaceUnityAddin.dll"] = sTargetPath + "KinectFaceUnityAddin.dll";
			dictFilesToUnzip["Kinect2SpeechWrapper.dll"] = sTargetPath + "Kinect2SpeechWrapper.dll";
			dictFilesToUnzip["Kinect20.VisualGestureBuilder.dll"] = sTargetPath + "Kinect20.VisualGestureBuilder.dll";
			dictFilesToUnzip["KinectVisualGestureBuilderUnityAddin.dll"] = sTargetPath + "KinectVisualGestureBuilderUnityAddin.dll";
			dictFilesToUnzip["vgbtechs/AdaBoostTech.dll"] = sTargetPath + "vgbtechs/AdaBoostTech.dll";
			dictFilesToUnzip["vgbtechs/RFRProgressTech.dll"] = sTargetPath + "vgbtechs/RFRProgressTech.dll";
			dictFilesToUnzip["msvcp110.dll"] = sTargetPath + "msvcp110.dll";
			dictFilesToUnzip["msvcr110.dll"] = sTargetPath + "msvcr110.dll";

			KinectInterop.UnzipResourceFiles(dictFilesToUnzip, "KinectV2UnityAddin.x64.zip", ref bOneCopied, ref bAllCopied);
		}

		KinectInterop.UnzipResourceDirectory(sTargetPath, "NuiDatabase.zip", sTargetPath + "NuiDatabase");

		bNeedRestart = (bOneCopied && bAllCopied);

		return true;
	}

	public void FreeSensorInterface (bool bDeleteLibs)
	{
		if(bDeleteLibs)
		{
			KinectInterop.DeleteNativeLib("KinectUnityAddin.dll", true);
			KinectInterop.DeleteNativeLib("msvcp110.dll", false);
			KinectInterop.DeleteNativeLib("msvcr110.dll", false);
		}
	}

	public bool IsSensorAvailable()
	{
		KinectSensor sensor = KinectSensor.GetDefault();

		if(sensor != null)
		{
			if(sensorAlwaysAvailable)
			{
				sensor = null;
				return true;
			}

			if(!sensor.IsOpen)
			{
				sensor.Open();
			}

			float fWaitTime = Time.realtimeSinceStartup + 3f;
			while(!sensor.IsAvailable && Time.realtimeSinceStartup < fWaitTime)
			{
				// wait for availability
			}
			
			bool bAvailable = sensor.IsAvailable;

			if(sensor.IsOpen)
			{
				sensor.Close();
			}
			
			fWaitTime = Time.realtimeSinceStartup + 3f;
			while(sensor.IsOpen && Time.realtimeSinceStartup < fWaitTime)
			{
				// wait for sensor to close
			}

			sensor = null;
			return bAvailable;
		}

		return false;
	}

	public int GetSensorsCount()
	{
		int numSensors = 0;

		KinectSensor sensor = KinectSensor.GetDefault();
		if(sensor != null)
		{
			if(!sensor.IsOpen)
			{
				sensor.Open();
			}
			
			float fWaitTime = Time.realtimeSinceStartup + 3f;
			while(!sensor.IsAvailable && Time.realtimeSinceStartup < fWaitTime)
			{
				// wait for availability
			}
			
			numSensors = sensor.IsAvailable ? 1 : 0;

			if(sensor.IsOpen)
			{
				sensor.Close();
			}
			
			fWaitTime = Time.realtimeSinceStartup + 3f;
			while(sensor.IsOpen && Time.realtimeSinceStartup < fWaitTime)
			{
				// wait for sensor to close
			}
		}

		return numSensors;
	}

	public KinectInterop.SensorData OpenDefaultSensor (KinectInterop.FrameSource dwFlags, float sensorAngle, bool bUseMultiSource)
	{
		KinectInterop.SensorData sensorData = new KinectInterop.SensorData();
		sensorFlags = dwFlags;
		
		kinectSensor = KinectSensor.GetDefault();
		if(kinectSensor == null)
			return null;
		
		coordMapper = kinectSensor.CoordinateMapper;

		this.bodyCount = kinectSensor.BodyFrameSource.BodyCount;
		sensorData.bodyCount = this.bodyCount;
		sensorData.jointCount = 25;

		sensorData.depthCameraFOV = 60f;
		sensorData.colorCameraFOV = 53.8f;
		sensorData.depthCameraOffset = -0.05f;
		sensorData.faceOverlayOffset = -0.04f;
		
		if((dwFlags & KinectInterop.FrameSource.TypeBody) != 0)
		{
			if(!bUseMultiSource)
				bodyFrameReader = kinectSensor.BodyFrameSource.OpenReader();
			
			bodyData = new Body[sensorData.bodyCount];
		}
		
		var frameDesc = kinectSensor.ColorFrameSource.CreateFrameDescription(ColorImageFormat.Rgba);
		sensorData.colorImageWidth = frameDesc.Width;
		sensorData.colorImageHeight = frameDesc.Height;
		
		if((dwFlags & KinectInterop.FrameSource.TypeColor) != 0)
		{
			if(!bUseMultiSource)
				colorFrameReader = kinectSensor.ColorFrameSource.OpenReader();
			
			sensorData.colorImage = new byte[frameDesc.BytesPerPixel * frameDesc.LengthInPixels];
		}
		
		sensorData.depthImageWidth = kinectSensor.DepthFrameSource.FrameDescription.Width;
		sensorData.depthImageHeight = kinectSensor.DepthFrameSource.FrameDescription.Height;
		
		if((dwFlags & KinectInterop.FrameSource.TypeDepth) != 0)
		{
			if(!bUseMultiSource)
				depthFrameReader = kinectSensor.DepthFrameSource.OpenReader();
			
			sensorData.depthImage = new ushort[kinectSensor.DepthFrameSource.FrameDescription.LengthInPixels];
		}
		
		if((dwFlags & KinectInterop.FrameSource.TypeBodyIndex) != 0)
		{
			if(!bUseMultiSource)
				bodyIndexFrameReader = kinectSensor.BodyIndexFrameSource.OpenReader();
			
			sensorData.bodyIndexImage = new byte[kinectSensor.BodyIndexFrameSource.FrameDescription.LengthInPixels];
		}
		
		if((dwFlags & KinectInterop.FrameSource.TypeInfrared) != 0)
		{
			if(!bUseMultiSource)
				infraredFrameReader = kinectSensor.InfraredFrameSource.OpenReader();
			
			sensorData.infraredImage = new ushort[kinectSensor.InfraredFrameSource.FrameDescription.LengthInPixels];
		}
		
		//if(!kinectSensor.IsOpen)
		{
			//Debug.Log("Opening sensor, available: " + kinectSensor.IsAvailable);
			kinectSensor.Open();
		}

		float fWaitTime = Time.realtimeSinceStartup + 3f;
		while(!kinectSensor.IsAvailable && Time.realtimeSinceStartup < fWaitTime)
		{
			// wait for sensor to open
		}

		Debug.Log("K2-sensor " + (kinectSensor.IsOpen ? "opened" : "closed") + 
		          ", available: " + kinectSensor.IsAvailable);

		if(bUseMultiSource && dwFlags != KinectInterop.FrameSource.TypeNone && kinectSensor.IsOpen)
		{
			multiSourceFrameReader = kinectSensor.OpenMultiSourceFrameReader((FrameSourceTypes)((int)dwFlags & 0x3F));
		}
		
		return sensorData;
	}

	public void CloseSensor (KinectInterop.SensorData sensorData)
	{
		if(coordMapper != null)
		{
			coordMapper = null;
		}
		
		if(bodyFrameReader != null)
		{
			bodyFrameReader.Dispose();
			bodyFrameReader = null;
		}
		
		if(bodyIndexFrameReader != null)
		{
			bodyIndexFrameReader.Dispose();
			bodyIndexFrameReader = null;
		}
		
		if(colorFrameReader != null)
		{
			colorFrameReader.Dispose();
			colorFrameReader = null;
		}
		
		if(depthFrameReader != null)
		{
			depthFrameReader.Dispose();
			depthFrameReader = null;
		}
		
		if(infraredFrameReader != null)
		{
			infraredFrameReader.Dispose();
			infraredFrameReader = null;
		}
		
		if(multiSourceFrameReader != null)
		{
			multiSourceFrameReader.Dispose();
			multiSourceFrameReader = null;
		}
		
		if(kinectSensor != null)
		{
			//if (kinectSensor.IsOpen)
			{
				//Debug.Log("Closing sensor, available: " + kinectSensor.IsAvailable);
				kinectSensor.Close();
			}
			
			float fWaitTime = Time.realtimeSinceStartup + 3f;
			while(kinectSensor.IsOpen && Time.realtimeSinceStartup < fWaitTime)
			{
				// wait for sensor to close
			}
			
			Debug.Log("K2-sensor " + (kinectSensor.IsOpen ? "opened" : "closed") + 
			          ", available: " + kinectSensor.IsAvailable);
			
			kinectSensor = null;
		}
	}

	public bool UpdateSensorData (KinectInterop.SensorData sensorData)
	{
		return true;
	}

	public bool GetMultiSourceFrame (KinectInterop.SensorData sensorData)
	{
		if(multiSourceFrameReader != null)
		{
			multiSourceFrame = multiSourceFrameReader.AcquireLatestFrame();

			if(multiSourceFrame != null)
			{
				// try to get all frames at once
				msBodyFrame = (sensorFlags & KinectInterop.FrameSource.TypeBody) != 0 ? multiSourceFrame.BodyFrameReference.AcquireFrame() : null;
				msBodyIndexFrame = (sensorFlags & KinectInterop.FrameSource.TypeBodyIndex) != 0 ? multiSourceFrame.BodyIndexFrameReference.AcquireFrame() : null;
				msColorFrame = (sensorFlags & KinectInterop.FrameSource.TypeColor) != 0 ? multiSourceFrame.ColorFrameReference.AcquireFrame() : null;
				msDepthFrame = (sensorFlags & KinectInterop.FrameSource.TypeDepth) != 0 ? multiSourceFrame.DepthFrameReference.AcquireFrame() : null;
				msInfraredFrame = (sensorFlags & KinectInterop.FrameSource.TypeInfrared) != 0 ? multiSourceFrame.InfraredFrameReference.AcquireFrame() : null;

				bool bAllSet =
					((sensorFlags & KinectInterop.FrameSource.TypeBody) == 0 || msBodyFrame != null) &&
					((sensorFlags & KinectInterop.FrameSource.TypeBodyIndex) == 0 || msBodyIndexFrame != null) &&
					((sensorFlags & KinectInterop.FrameSource.TypeColor) == 0 || msColorFrame != null) &&
					((sensorFlags & KinectInterop.FrameSource.TypeDepth) == 0 || msDepthFrame != null) &&
					((sensorFlags & KinectInterop.FrameSource.TypeInfrared) == 0 || msInfraredFrame != null);

				if(!bAllSet)
				{
					// release all frames
					if(msBodyFrame != null)
					{
						msBodyFrame.Dispose();
						msBodyFrame = null;
					}

					if(msBodyIndexFrame != null)
					{
						msBodyIndexFrame.Dispose();
						msBodyIndexFrame = null;
					}

					if(msColorFrame != null)
					{
						msColorFrame.Dispose();
						msColorFrame = null;
					}

					if(msDepthFrame != null)
					{
						msDepthFrame.Dispose();
						msDepthFrame = null;
					}

					if(msInfraredFrame != null)
					{
						msInfraredFrame.Dispose();
						msInfraredFrame = null;
					}
				}
//				else
//				{
//					bool bNeedBody = (sensorFlags & KinectInterop.FrameSource.TypeBody) != 0;
//					bool bNeedBodyIndex = (sensorFlags & KinectInterop.FrameSource.TypeBodyIndex) != 0;
//					bool bNeedColor = (sensorFlags & KinectInterop.FrameSource.TypeColor) != 0;
//					bool bNeedDepth = (sensorFlags & KinectInterop.FrameSource.TypeDepth) != 0;
//					bool bNeedInfrared = (sensorFlags & KinectInterop.FrameSource.TypeInfrared) != 0;
//
//					bAllSet = true;
//				}
			}

			return (multiSourceFrame != null);
		}
		
		return false;
	}

	public void FreeMultiSourceFrame (KinectInterop.SensorData sensorData)
	{
		// release all frames
		if(msBodyFrame != null)
		{
			msBodyFrame.Dispose();
			msBodyFrame = null;
		}
		
		if(msBodyIndexFrame != null)
		{
			msBodyIndexFrame.Dispose();
			msBodyIndexFrame = null;
		}
		
		if(msColorFrame != null)
		{
			msColorFrame.Dispose();
			msColorFrame = null;
		}
		
		if(msDepthFrame != null)
		{
			msDepthFrame.Dispose();
			msDepthFrame = null;
		}
		
		if(msInfraredFrame != null)
		{
			msInfraredFrame.Dispose();
			msInfraredFrame = null;
		}

		if(multiSourceFrame != null)
		{
			multiSourceFrame = null;
		}
	}

	public bool PollBodyFrame (KinectInterop.SensorData sensorData, ref KinectInterop.BodyFrameData bodyFrame, 
	                           ref Matrix4x4 kinectToWorld, bool bIgnoreJointZ)
	{
		bool bNewFrame = false;
		
		if((multiSourceFrameReader != null && multiSourceFrame != null) || 
		   bodyFrameReader != null)
		{
			BodyFrame frame = multiSourceFrame != null ? msBodyFrame : 
				bodyFrameReader.AcquireLatestFrame();

			if(frame != null)
			{
				frame.GetAndRefreshBodyData(bodyData);

				bodyFrame.liPreviousTime = bodyFrame.liRelativeTime;
				bodyFrame.liRelativeTime = frame.RelativeTime.Ticks;

				if(sensorData.hintHeightAngle)
				{
					// get the floor plane
					Windows.Kinect.Vector4 vFloorPlane = frame.FloorClipPlane;
					Vector3 floorPlane = new Vector3(vFloorPlane.X, vFloorPlane.Y, vFloorPlane.Z);

					sensorData.sensorRotDetected = Quaternion.FromToRotation(floorPlane, Vector3.up);
					sensorData.sensorHgtDetected = vFloorPlane.W;
				}

				frame.Dispose();
				frame = null;
				
				for(int i = 0; i < sensorData.bodyCount; i++)
				{
					Body body = bodyData[i];
					
					if (body == null)
					{
						bodyFrame.bodyData[i].bIsTracked = 0;
						continue;
					}
					
					bodyFrame.bodyData[i].bIsTracked = (short)(body.IsTracked ? 1 : 0);
					
					if(body.IsTracked)
					{
						// transfer body and joints data
						bodyFrame.bodyData[i].liTrackingID = (long)body.TrackingId;

						// cache the body joints (following the advice of Brian Chasalow)
						Dictionary<Windows.Kinect.JointType, Windows.Kinect.Joint> bodyJoints = body.Joints;

						// calculate the inter-frame time
						float frameTime = 0f;
						if(bodyFrame.bTurnAnalisys && bodyFrame.liPreviousTime > 0)
						{
							frameTime = (float)(bodyFrame.liRelativeTime - bodyFrame.liPreviousTime) / 100000000000;
						}

						for(int j = 0; j < sensorData.jointCount; j++)
						{
							Windows.Kinect.Joint joint = bodyJoints[(Windows.Kinect.JointType)j];
							KinectInterop.JointData jointData = bodyFrame.bodyData[i].joint[j];
							
							//jointData.jointType = (KinectInterop.JointType)j;
							jointData.trackingState = (KinectInterop.TrackingState)joint.TrackingState;

							if((int)joint.TrackingState != (int)TrackingState.NotTracked)
							{
								float jPosZ = (bIgnoreJointZ && j > 0) ? bodyFrame.bodyData[i].joint[0].kinectPos.z : joint.Position.Z;
								jointData.kinectPos = new Vector3(joint.Position.X, joint.Position.Y, jPosZ);
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

						if(bodyFrame.bTurnAnalisys && bodyFrame.liPreviousTime > 0)
						{
							for(int j = 0; j < sensorData.jointCount; j++)
							{
								KinectInterop.JointData jointData = bodyFrame.bodyData[i].joint[j];

								int p = (int)GetParentJoint((KinectInterop.JointType)j);
								Vector3 parentPos = bodyFrame.bodyData[i].joint[p].position;
								
								jointData.posRel = jointData.position - parentPos;
								jointData.posDrv = frameTime > 0f ? (jointData.position - jointData.posPrev) / frameTime : Vector3.zero;
								jointData.posPrev = jointData.position;

								bodyFrame.bodyData[i].joint[j] = jointData;
							}
						}
						
						// tranfer hand states
						bodyFrame.bodyData[i].leftHandState = (KinectInterop.HandState)body.HandLeftState;
						bodyFrame.bodyData[i].leftHandConfidence = (KinectInterop.TrackingConfidence)body.HandLeftConfidence;
						
						bodyFrame.bodyData[i].rightHandState = (KinectInterop.HandState)body.HandRightState;
						bodyFrame.bodyData[i].rightHandConfidence = (KinectInterop.TrackingConfidence)body.HandRightConfidence;
					}
				}
				
				bNewFrame = true;
			}
		}
		
		return bNewFrame;
	}

	public bool PollColorFrame (KinectInterop.SensorData sensorData)
	{
		bool bNewFrame = false;
		
		if((multiSourceFrameReader != null && multiSourceFrame != null) ||
		   colorFrameReader != null) 
		{
			ColorFrame colorFrame = multiSourceFrame != null ? msColorFrame :
				colorFrameReader.AcquireLatestFrame();
			
			if(colorFrame != null)
			{
				var pColorData = GCHandle.Alloc(sensorData.colorImage, GCHandleType.Pinned);
				colorFrame.CopyConvertedFrameDataToIntPtr(pColorData.AddrOfPinnedObject(), (uint)sensorData.colorImage.Length, ColorImageFormat.Rgba);
				pColorData.Free();

				sensorData.lastColorFrameTime = colorFrame.RelativeTime.Ticks;
				
				colorFrame.Dispose();
				colorFrame = null;
				
				bNewFrame = true;
			}
		}
		
		return bNewFrame;
	}

	public bool PollDepthFrame (KinectInterop.SensorData sensorData)
	{
		bool bNewFrame = false;
		
		if((multiSourceFrameReader != null && multiSourceFrame != null) ||
		   depthFrameReader != null)
		{
			DepthFrame depthFrame = multiSourceFrame != null ? msDepthFrame :
				depthFrameReader.AcquireLatestFrame();
			
			if(depthFrame != null)
			{
				var pDepthData = GCHandle.Alloc(sensorData.depthImage, GCHandleType.Pinned);
				depthFrame.CopyFrameDataToIntPtr(pDepthData.AddrOfPinnedObject(), (uint)sensorData.depthImage.Length * sizeof(ushort));
				pDepthData.Free();
				
				sensorData.lastDepthFrameTime = depthFrame.RelativeTime.Ticks;
				
				depthFrame.Dispose();
				depthFrame = null;
				
				bNewFrame = true;
			}
			
			if((multiSourceFrameReader != null && multiSourceFrame != null) ||
			   bodyIndexFrameReader != null)
			{
				BodyIndexFrame bodyIndexFrame = multiSourceFrame != null ? msBodyIndexFrame : 
					bodyIndexFrameReader.AcquireLatestFrame();
				
				if(bodyIndexFrame != null)
				{
					var pBodyIndexData = GCHandle.Alloc(sensorData.bodyIndexImage, GCHandleType.Pinned);
					bodyIndexFrame.CopyFrameDataToIntPtr(pBodyIndexData.AddrOfPinnedObject(), (uint)sensorData.bodyIndexImage.Length);
					pBodyIndexData.Free();
					
					sensorData.lastBodyIndexFrameTime = bodyIndexFrame.RelativeTime.Ticks;
					
					bodyIndexFrame.Dispose();
					bodyIndexFrame = null;
					
					bNewFrame = true;
				}
			}
		}
		
		return bNewFrame;
	}

	public bool PollInfraredFrame (KinectInterop.SensorData sensorData)
	{
		bool bNewFrame = false;
		
		if((multiSourceFrameReader != null && multiSourceFrame != null) ||
		   infraredFrameReader != null)
		{
			InfraredFrame infraredFrame = multiSourceFrame != null ? msInfraredFrame : 
				infraredFrameReader.AcquireLatestFrame();
			
			if(infraredFrame != null)
			{
				var pInfraredData = GCHandle.Alloc(sensorData.infraredImage, GCHandleType.Pinned);
				infraredFrame.CopyFrameDataToIntPtr(pInfraredData.AddrOfPinnedObject(), (uint)sensorData.infraredImage.Length * sizeof(ushort));
				pInfraredData.Free();
				
				sensorData.lastInfraredFrameTime = infraredFrame.RelativeTime.Ticks;
				
				infraredFrame.Dispose();
				infraredFrame = null;
				
				bNewFrame = true;
			}
		}
		
		return bNewFrame;
	}

	public void FixJointOrientations(KinectInterop.SensorData sensorData, ref KinectInterop.BodyData bodyData)
	{
		// no fixes are needed
	}

	public bool IsBodyTurned(ref KinectInterop.BodyData bodyData)
	{
		//face = On: Face (357.0/1.0)
		//face = Off
		//|   Head_px <= -0.02
		//|   |   Neck_dx <= 0.08: Face (46.0/1.0)
		//|   |   Neck_dx > 0.08: Back (3.0)
		//|   Head_px > -0.02
		//|   |   SpineShoulder_px <= -0.02: Face (4.0)
		//|   |   SpineShoulder_px > -0.02: Back (64.0/1.0)
		
		bool bBodyTurned = false;

		if(bFaceTrackingInited)
		{
			bool bFaceOn = IsFaceTracked(bodyData.liTrackingID);
			
			if(bFaceOn)
			{
				bBodyTurned = false;
			}
			else
			{
				// face = Off
				if(bodyData.joint[(int)KinectInterop.JointType.Head].posRel.x <= -0.02f)
				{
					bBodyTurned = (bodyData.joint[(int)KinectInterop.JointType.Neck].posDrv.x > 0.08f);
				}
				else
				{
					// Head_px > -0.02
					bBodyTurned = (bodyData.joint[(int)KinectInterop.JointType.SpineShoulder].posRel.x > -0.02f);
				}
			}
		}

		return bBodyTurned;
	}

	public Vector2 MapSpacePointToDepthCoords (KinectInterop.SensorData sensorData, Vector3 spacePos)
	{
		Vector2 vPoint = Vector2.zero;
		
		if(coordMapper != null)
		{
			CameraSpacePoint camPoint = new CameraSpacePoint();
			camPoint.X = spacePos.x;
			camPoint.Y = spacePos.y;
			camPoint.Z = spacePos.z;
			
			CameraSpacePoint[] camPoints = new CameraSpacePoint[1];
			camPoints[0] = camPoint;
			
			DepthSpacePoint[] depthPoints = new DepthSpacePoint[1];
			coordMapper.MapCameraPointsToDepthSpace(camPoints, depthPoints);
			
			DepthSpacePoint depthPoint = depthPoints[0];
			
			if(depthPoint.X >= 0 && depthPoint.X < sensorData.depthImageWidth &&
			   depthPoint.Y >= 0 && depthPoint.Y < sensorData.depthImageHeight)
			{
				vPoint.x = depthPoint.X;
				vPoint.y = depthPoint.Y;
			}
		}
		
		return vPoint;
	}

	public Vector3 MapDepthPointToSpaceCoords (KinectInterop.SensorData sensorData, Vector2 depthPos, ushort depthVal)
	{
		Vector3 vPoint = Vector3.zero;
		
		if(coordMapper != null && depthPos != Vector2.zero)
		{
			DepthSpacePoint depthPoint = new DepthSpacePoint();
			depthPoint.X = depthPos.x;
			depthPoint.Y = depthPos.y;
			
			DepthSpacePoint[] depthPoints = new DepthSpacePoint[1];
			depthPoints[0] = depthPoint;
			
			ushort[] depthVals = new ushort[1];
			depthVals[0] = depthVal;
			
			CameraSpacePoint[] camPoints = new CameraSpacePoint[1];
			coordMapper.MapDepthPointsToCameraSpace(depthPoints, depthVals, camPoints);
			
			CameraSpacePoint camPoint = camPoints[0];
			vPoint.x = camPoint.X;
			vPoint.y = camPoint.Y;
			vPoint.z = camPoint.Z;
		}
		
		return vPoint;
	}

	public Vector2 MapDepthPointToColorCoords (KinectInterop.SensorData sensorData, Vector2 depthPos, ushort depthVal)
	{
		Vector2 vPoint = Vector2.zero;
		
		if(coordMapper != null && depthPos != Vector2.zero)
		{
			DepthSpacePoint depthPoint = new DepthSpacePoint();
			depthPoint.X = depthPos.x;
			depthPoint.Y = depthPos.y;
			
			DepthSpacePoint[] depthPoints = new DepthSpacePoint[1];
			depthPoints[0] = depthPoint;
			
			ushort[] depthVals = new ushort[1];
			depthVals[0] = depthVal;
			
			ColorSpacePoint[] colPoints = new ColorSpacePoint[1];
			coordMapper.MapDepthPointsToColorSpace(depthPoints, depthVals, colPoints);
			
			ColorSpacePoint colPoint = colPoints[0];
			vPoint.x = colPoint.X;
			vPoint.y = colPoint.Y;
		}
		
		return vPoint;
	}

	public bool MapDepthFrameToColorCoords (KinectInterop.SensorData sensorData, ref Vector2[] vColorCoords)
	{
		if(coordMapper != null && sensorData.colorImage != null && sensorData.depthImage != null)
		{
			var pDepthData = GCHandle.Alloc(sensorData.depthImage, GCHandleType.Pinned);
			var pColorCoordsData = GCHandle.Alloc(vColorCoords, GCHandleType.Pinned);
			
			coordMapper.MapDepthFrameToColorSpaceUsingIntPtr(
				pDepthData.AddrOfPinnedObject(), 
				sensorData.depthImage.Length * sizeof(ushort),
				pColorCoordsData.AddrOfPinnedObject(), 
				(uint)vColorCoords.Length);
			
			pColorCoordsData.Free();
			pDepthData.Free();
			
			return true;
		}
		
		return false;
	}

	public bool MapColorFrameToDepthCoords (KinectInterop.SensorData sensorData, ref Vector2[] vDepthCoords)
	{
		if(coordMapper != null && sensorData.colorImage != null && sensorData.depthImage != null)
		{
			var pDepthData = GCHandle.Alloc(sensorData.depthImage, GCHandleType.Pinned);
			var pDepthCoordsData = GCHandle.Alloc(vDepthCoords, GCHandleType.Pinned);
			
			coordMapper.MapColorFrameToDepthSpaceUsingIntPtr(
				pDepthData.AddrOfPinnedObject(), 
				(uint)sensorData.depthImage.Length * sizeof(ushort),
				pDepthCoordsData.AddrOfPinnedObject(), 
				(uint)vDepthCoords.Length);
			
			pDepthCoordsData.Free();
			pDepthData.Free();
			
			return true;
		}
		
		return false;
	}
	
	// returns the index of the given joint in joint's array or -1 if joint is not applicable
	public int GetJointIndex(KinectInterop.JointType joint)
	{
		return (int)joint;
	}
	
//	// returns the joint at given index
//	public KinectInterop.JointType GetJointAtIndex(int index)
//	{
//		return (KinectInterop.JointType)(index);
//	}
	
	// returns the parent joint of the given joint
	public KinectInterop.JointType GetParentJoint(KinectInterop.JointType joint)
	{
		switch(joint)
		{
			case KinectInterop.JointType.SpineBase:
				return KinectInterop.JointType.SpineBase;
				
			case KinectInterop.JointType.Neck:
				return KinectInterop.JointType.SpineShoulder;
				
			case KinectInterop.JointType.SpineShoulder:
				return KinectInterop.JointType.SpineMid;
				
			case KinectInterop.JointType.ShoulderLeft:
			case KinectInterop.JointType.ShoulderRight:
				return KinectInterop.JointType.SpineShoulder;
				
			case KinectInterop.JointType.HipLeft:
			case KinectInterop.JointType.HipRight:
				return KinectInterop.JointType.SpineBase;
				
			case KinectInterop.JointType.HandTipLeft:
				return KinectInterop.JointType.HandLeft;
				
			case KinectInterop.JointType.ThumbLeft:
				return KinectInterop.JointType.WristLeft;
			
			case KinectInterop.JointType.HandTipRight:
				return KinectInterop.JointType.HandRight;

			case KinectInterop.JointType.ThumbRight:
				return KinectInterop.JointType.WristRight;
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
				return KinectInterop.JointType.SpineShoulder;
			case KinectInterop.JointType.SpineShoulder:
				return KinectInterop.JointType.Neck;
			case KinectInterop.JointType.Neck:
				return KinectInterop.JointType.Head;
				
			case KinectInterop.JointType.ShoulderLeft:
				return KinectInterop.JointType.ElbowLeft;
			case KinectInterop.JointType.ElbowLeft:
				return KinectInterop.JointType.WristLeft;
			case KinectInterop.JointType.WristLeft:
				return KinectInterop.JointType.HandLeft;
			case KinectInterop.JointType.HandLeft:
				return KinectInterop.JointType.HandTipLeft;
				
			case KinectInterop.JointType.ShoulderRight:
				return KinectInterop.JointType.ElbowRight;
			case KinectInterop.JointType.ElbowRight:
				return KinectInterop.JointType.WristRight;
			case KinectInterop.JointType.WristRight:
				return KinectInterop.JointType.HandRight;
			case KinectInterop.JointType.HandRight:
				return KinectInterop.JointType.HandTipRight;
				
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
		
		return joint;  // in case of end joint - Head, HandTipLeft, HandTipRight, FootLeft, FootRight
	}
	
	public bool IsFaceTrackingAvailable(ref bool bNeedRestart)
	{
		bool bOneCopied = false, bAllCopied = true;
		string sTargetPath = ".";

		if(!KinectInterop.Is64bitArchitecture())
		{
			// 32 bit
			sTargetPath = KinectInterop.GetTargetDllPath(".", false) + "/";

			Dictionary<string, string> dictFilesToUnzip = new Dictionary<string, string>();
			dictFilesToUnzip["Kinect20.Face.dll"] = sTargetPath + "Kinect20.Face.dll";
			dictFilesToUnzip["KinectFaceUnityAddin.dll"] = sTargetPath + "KinectFaceUnityAddin.dll";
			dictFilesToUnzip["msvcp110.dll"] = sTargetPath + "msvcp110.dll";
			dictFilesToUnzip["msvcr110.dll"] = sTargetPath + "msvcr110.dll";

			KinectInterop.UnzipResourceFiles(dictFilesToUnzip, "KinectV2UnityAddin.x86.zip", ref bOneCopied, ref bAllCopied);
		}
		else
		{
			//Debug.Log("Face - x64-architecture.");
			sTargetPath = KinectInterop.GetTargetDllPath(".", true) + "/";

			Dictionary<string, string> dictFilesToUnzip = new Dictionary<string, string>();
			dictFilesToUnzip["Kinect20.Face.dll"] = sTargetPath + "Kinect20.Face.dll";
			dictFilesToUnzip["KinectFaceUnityAddin.dll"] = sTargetPath + "KinectFaceUnityAddin.dll";
			dictFilesToUnzip["msvcp110.dll"] = sTargetPath + "msvcp110.dll";
			dictFilesToUnzip["msvcr110.dll"] = sTargetPath + "msvcr110.dll";
			
			KinectInterop.UnzipResourceFiles(dictFilesToUnzip, "KinectV2UnityAddin.x64.zip", ref bOneCopied, ref bAllCopied);
		}

		KinectInterop.UnzipResourceDirectory(sTargetPath, "NuiDatabase.zip", sTargetPath + "NuiDatabase");
		
		bNeedRestart = (bOneCopied && bAllCopied);
		
		return true;
	}
	
	public bool InitFaceTracking(bool bUseFaceModel, bool bDrawFaceRect)
	{
		isDrawFaceRect = bDrawFaceRect;

//		// load the native dlls to make sure libraries are loaded (after previous finish-unload)
//		KinectInterop.LoadNativeLib("Kinect20.Face.dll");
//		KinectInterop.LoadNativeLib("KinectFaceUnityAddin.dll");

		// specify the required face frame results
		FaceFrameFeatures faceFrameFeatures =
			FaceFrameFeatures.BoundingBoxInColorSpace
				//| FaceFrameFeatures.BoundingBoxInInfraredSpace
				| FaceFrameFeatures.PointsInColorSpace
				//| FaceFrameFeatures.PointsInInfraredSpace
				| FaceFrameFeatures.RotationOrientation
				//| FaceFrameFeatures.FaceEngagement
				//| FaceFrameFeatures.Glasses
				//| FaceFrameFeatures.Happy
				//| FaceFrameFeatures.LeftEyeClosed
				//| FaceFrameFeatures.RightEyeClosed
				//| FaceFrameFeatures.LookingAway
				//| FaceFrameFeatures.MouthMoved
				//| FaceFrameFeatures.MouthOpen
				;
		
		// create a face frame source + reader to track each face in the FOV
		faceFrameSources = new FaceFrameSource[this.bodyCount];
		faceFrameReaders = new FaceFrameReader[this.bodyCount];

		if(bUseFaceModel)
		{
			hdFaceFrameSources = new HighDefinitionFaceFrameSource[this.bodyCount];
			hdFaceFrameReaders = new HighDefinitionFaceFrameReader[this.bodyCount];

			hdFaceModels = new FaceModel[this.bodyCount];
			hdFaceAlignments = new FaceAlignment[this.bodyCount];
		}

		for (int i = 0; i < bodyCount; i++)
		{
			// create the face frame source with the required face frame features and an initial tracking Id of 0
			faceFrameSources[i] = FaceFrameSource.Create(this.kinectSensor, 0, faceFrameFeatures);
			
			// open the corresponding reader
			faceFrameReaders[i] = faceFrameSources[i].OpenReader();

			if(bUseFaceModel)
			{
				///////// HD Face
				hdFaceFrameSources[i] = HighDefinitionFaceFrameSource.Create(this.kinectSensor);
				hdFaceFrameReaders[i] = hdFaceFrameSources[i].OpenReader();

				hdFaceModels[i] = FaceModel.Create();
				hdFaceAlignments[i] = FaceAlignment.Create();
			}
		}
		
		// allocate storage to store face frame results for each face in the FOV
		faceFrameResults = new FaceFrameResult[this.bodyCount];

//		FrameDescription frameDescription = this.kinectSensor.ColorFrameSource.FrameDescription;
//		faceDisplayWidth = frameDescription.Width;
//		faceDisplayHeight = frameDescription.Height;

		bFaceTrackingInited = true;

		return bFaceTrackingInited;
	}
	
	public void FinishFaceTracking()
	{
		if(faceFrameReaders != null)
		{
			for (int i = 0; i < faceFrameReaders.Length; i++)
			{
				if (faceFrameReaders[i] != null)
				{
					faceFrameReaders[i].Dispose();
					faceFrameReaders[i] = null;
				}
			}
		}

		if(faceFrameSources != null)
		{
			for (int i = 0; i < faceFrameSources.Length; i++)
			{
				faceFrameSources[i] = null;
			}
		}

		///////// HD Face
		if(hdFaceFrameSources != null)
		{
			for (int i = 0; i < hdFaceAlignments.Length; i++)
			{
				hdFaceAlignments[i] = null;
			}

			for (int i = 0; i < hdFaceModels.Length; i++)
			{
				if (hdFaceModels[i] != null)
				{
					hdFaceModels[i].Dispose();
					hdFaceModels[i] = null;
				}
			}
			
			for (int i = 0; i < hdFaceFrameReaders.Length; i++)
			{
				if (hdFaceFrameReaders[i] != null)
				{
					hdFaceFrameReaders[i].Dispose();
					hdFaceFrameReaders[i] = null;
				}
			}

			for (int i = 0; i < hdFaceFrameSources.Length; i++)
			{
				//hdFaceFrameSources[i].Dispose(true);
				hdFaceFrameSources[i] = null;
			}
		}
		
		bFaceTrackingInited = false;

//		// unload the native dlls to prevent hd-face-wrapper's memory leaks
//		KinectInterop.DeleteNativeLib("KinectFaceUnityAddin.dll", true);
//		KinectInterop.DeleteNativeLib("Kinect20.Face.dll", true);

	}
	
	public bool UpdateFaceTracking()
	{
		if(bodyData == null || faceFrameSources == null || faceFrameReaders == null)
			return false;

		for(int i = 0; i < this.bodyCount; i++)
		{
			if(faceFrameSources[i] != null)
			{
				if(!faceFrameSources[i].IsTrackingIdValid)
				{
					faceFrameSources[i].TrackingId = 0;
				}
				
				if(bodyData[i] != null && bodyData[i].IsTracked)
				{
					faceFrameSources[i].TrackingId = bodyData[i].TrackingId;
				}
			}

			if (faceFrameReaders[i] != null) 
			{
				FaceFrame faceFrame = faceFrameReaders[i].AcquireLatestFrame();
				
				if (faceFrame != null)
				{
					int index = GetFaceSourceIndex(faceFrame.FaceFrameSource);
					
					if(ValidateFaceBox(faceFrame.FaceFrameResult))
					{
						faceFrameResults[index] = faceFrame.FaceFrameResult;
					}
					else
					{
						faceFrameResults[index] = null;
					}
					
					faceFrame.Dispose();
					faceFrame = null;
				}
			}

			///////// HD Face
			if(hdFaceFrameSources != null && hdFaceFrameSources[i] != null)
			{
				if(!hdFaceFrameSources[i].IsTrackingIdValid)
				{
					hdFaceFrameSources[i].TrackingId = 0;
				}

				if(bodyData[i] != null && bodyData[i].IsTracked)
				{
					hdFaceFrameSources[i].TrackingId = bodyData[i].TrackingId;
				}
			}
			
			if(hdFaceFrameReaders != null && hdFaceFrameReaders[i] != null)
			{
				HighDefinitionFaceFrame hdFaceFrame = hdFaceFrameReaders[i].AcquireLatestFrame();
				
				if(hdFaceFrame != null)
				{
					if(hdFaceFrame.IsFaceTracked && (hdFaceAlignments[i] != null))
					{
						hdFaceFrame.GetAndRefreshFaceAlignmentResult(hdFaceAlignments[i]);
					}
					
					hdFaceFrame.Dispose();
					hdFaceFrame = null;
				}
			}

		}

		return true;
	}
	
	private int GetFaceSourceIndex(FaceFrameSource faceFrameSource)
	{
		int index = -1;
		
		for (int i = 0; i < this.bodyCount; i++)
		{
			if (this.faceFrameSources[i] == faceFrameSource)
			{
				index = i;
				break;
			}
		}
		
		return index;
	}
	
	private bool ValidateFaceBox(FaceFrameResult faceResult)
	{
		bool isFaceValid = faceResult != null;
		
		if (isFaceValid)
		{
			var faceBox = faceResult.FaceBoundingBoxInColorSpace;
			//if (faceBox != null)
			{
				// check if we have a valid rectangle within the bounds of the screen space
				isFaceValid = (faceBox.Right - faceBox.Left) > 0 &&
					(faceBox.Bottom - faceBox.Top) > 0; // &&
						//faceBox.Right <= this.faceDisplayWidth &&
						//faceBox.Bottom <= this.faceDisplayHeight;
			}
		}
		
		return isFaceValid;
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
		for (int i = 0; i < this.bodyCount; i++)
		{
			if(faceFrameSources != null && faceFrameSources[i] != null && faceFrameSources[i].TrackingId == (ulong)userId)
			{
				if(faceFrameResults != null && faceFrameResults[i] != null)
				{
					return true;
				}
			}
		}

		return false;
	}

	public bool GetFaceRect(long userId, ref Rect faceRect)
	{
		for (int i = 0; i < this.bodyCount; i++)
		{
			if(faceFrameSources != null && faceFrameSources[i] != null && faceFrameSources[i].TrackingId == (ulong)userId)
			{
				if(faceFrameResults != null && faceFrameResults[i] != null)
				{
					var faceBox = faceFrameResults[i].FaceBoundingBoxInColorSpace;

					//if (faceBox != null)
					{
						faceRect.x = faceBox.Left;
						faceRect.y = faceBox.Top;
						faceRect.width = faceBox.Right - faceBox.Left;
						faceRect.height = faceBox.Bottom - faceBox.Top;
						
						return true;
					}
				}
			}
		}
		
		return false;
	}
	
	public void VisualizeFaceTrackerOnColorTex(Texture2D texColor)
	{
		if(bFaceTrackingInited)
		{
			for (int i = 0; i < this.bodyCount; i++)
			{
				if(faceFrameSources != null && faceFrameSources[i] != null && faceFrameSources[i].IsTrackingIdValid)
				{
					if(faceFrameResults != null && faceFrameResults[i] != null)
					{
						var faceBox = faceFrameResults[i].FaceBoundingBoxInColorSpace;
						
						//if (faceBox != null)
						{
							UnityEngine.Color color = UnityEngine.Color.magenta;
							Vector2 pt1, pt2;
							
							// bottom
							pt1.x = faceBox.Left; pt1.y = faceBox.Top;
							pt2.x = faceBox.Right; pt2.y = pt1.y;
							DrawLine(texColor, pt1, pt2, color);
							
							// right
							pt1.x = pt2.x; pt1.y = pt2.y;
							pt2.x = pt1.x; pt2.y = faceBox.Bottom;
							DrawLine(texColor, pt1, pt2, color);
							
							// top
							pt1.x = pt2.x; pt1.y = pt2.y;
							pt2.x = faceBox.Left; pt2.y = pt1.y;
							DrawLine(texColor, pt1, pt2, color);
							
							// left
							pt1.x = pt2.x; pt1.y = pt2.y;
							pt2.x = pt1.x; pt2.y = faceBox.Top;
							DrawLine(texColor, pt1, pt2, color);
						}
					}
				}
			}
		}
	}
	
	private void DrawLine(Texture2D a_Texture, Vector2 ptStart, Vector2 ptEnd, UnityEngine.Color a_Color)
	{
		KinectInterop.DrawLine(a_Texture, (int)ptStart.x, (int)ptStart.y, (int)ptEnd.x, (int)ptEnd.y, a_Color);
	}
	
	public bool GetHeadPosition(long userId, ref Vector3 headPos)
	{
		for (int i = 0; i < this.bodyCount; i++)
		{
			if(bodyData[i].TrackingId == (ulong)userId && bodyData[i].IsTracked)
			{
				CameraSpacePoint vHeadPos = bodyData[i].Joints[Windows.Kinect.JointType.Head].Position;

				if(vHeadPos.Z > 0f)
				{
					headPos.x = vHeadPos.X;
					headPos.y = vHeadPos.Y;
					headPos.z = vHeadPos.Z;
					
					return true;
				}
			}
		}
		
		return false;
	}
	
	public bool GetHeadRotation(long userId, ref Quaternion headRot)
	{
		for (int i = 0; i < this.bodyCount; i++)
		{
			if(faceFrameSources != null && faceFrameSources[i] != null && faceFrameSources[i].TrackingId == (ulong)userId)
			{
				if(faceFrameResults != null && faceFrameResults[i] != null)
				{
					Windows.Kinect.Vector4 vHeadRot = faceFrameResults[i].FaceRotationQuaternion;

					if(vHeadRot.W > 0f)
					{
						headRot = new Quaternion(vHeadRot.X, vHeadRot.Y, vHeadRot.Z, vHeadRot.W);
						return true;
					}
//					else
//					{
//						Debug.Log(string.Format("Bad rotation: ({0:F2}, {1:F2}, {2:F2}, {3:F2}})", vHeadRot.X, vHeadRot.Y, vHeadRot.Z, vHeadRot.W));
//						return false;
//					}

				}
			}
		}
		
		return false;
	}
	
	public bool GetAnimUnits(long userId, ref Dictionary<KinectInterop.FaceShapeAnimations, float> dictAU)
	{
		for (int i = 0; i < this.bodyCount; i++)
		{
			if(hdFaceFrameSources != null && hdFaceFrameSources[i] != null && hdFaceFrameSources[i].TrackingId == (ulong)userId)
			{
				if(hdFaceAlignments != null && hdFaceAlignments[i] != null)
				{
					foreach(Microsoft.Kinect.Face.FaceShapeAnimations akey in hdFaceAlignments[i].AnimationUnits.Keys)
					{
						dictAU[(KinectInterop.FaceShapeAnimations)akey] = hdFaceAlignments[i].AnimationUnits[akey];
					}

					return true;
				}
			}
		}
		
		return false;
	}
	
	public bool GetShapeUnits(long userId, ref Dictionary<KinectInterop.FaceShapeDeformations, float> dictSU)
	{
		for (int i = 0; i < this.bodyCount; i++)
		{
			if(hdFaceFrameSources != null && hdFaceFrameSources[i] != null && hdFaceFrameSources[i].TrackingId == (ulong)userId)
			{
				if(hdFaceModels != null && hdFaceModels[i] != null)
				{
					foreach(Microsoft.Kinect.Face.FaceShapeDeformations skey in hdFaceModels[i].FaceShapeDeformations.Keys)
					{
						dictSU[(KinectInterop.FaceShapeDeformations)skey] = hdFaceModels[i].FaceShapeDeformations[skey];
					}
					
					return true;
				}
			}
		}
		
		return false;
	}
	
	public int GetFaceModelVerticesCount(long userId)
	{
		for (int i = 0; i < this.bodyCount; i++)
		{
			if(hdFaceFrameSources != null && hdFaceFrameSources[i] != null && (hdFaceFrameSources[i].TrackingId == (ulong)userId || userId == 0))
			{
				if(hdFaceModels != null && hdFaceModels[i] != null)
				{
					var vertices = hdFaceModels[i].CalculateVerticesForAlignment(hdFaceAlignments[i]);
					int verticesCount = vertices.Count;

					return verticesCount;
				}
			}
		}
		
		return 0;
	}
	
	public bool GetFaceModelVertices(long userId, ref Vector3[] avVertices)
	{
		for (int i = 0; i < this.bodyCount; i++)
		{
			if(hdFaceFrameSources != null && hdFaceFrameSources[i] != null && (hdFaceFrameSources[i].TrackingId == (ulong)userId || userId == 0))
			{
				if(hdFaceModels != null && hdFaceModels[i] != null)
				{
					var vertices = hdFaceModels[i].CalculateVerticesForAlignment(hdFaceAlignments[i]);
					int verticesCount = vertices.Count;

					if(avVertices.Length == verticesCount)
					{
						for(int v = 0; v < verticesCount; v++)
						{
							avVertices[v].x = vertices[v].X;
							avVertices[v].y = vertices[v].Y;
							avVertices[v].z = vertices[v].Z;  // -vertices[v].Z;
						}
					}

					return true;
				}
			}
		}
		
		return false;
	}
	
	public int GetFaceModelTrianglesCount()
	{
		var triangleIndices = FaceModel.TriangleIndices;
		int triangleLength = triangleIndices.Count;

		return triangleLength;
	}
	
	public bool GetFaceModelTriangles(bool bMirrored, ref int[] avTriangles)
	{
		var triangleIndices = FaceModel.TriangleIndices;
		int triangleLength = triangleIndices.Count;

		if(avTriangles.Length >= triangleLength)
		{
			for(int i = 0; i < triangleLength; i += 3)
			{
				//avTriangles[i] = (int)triangleIndices[i];
				avTriangles[i] = (int)triangleIndices[i + 2];
				avTriangles[i + 1] = (int)triangleIndices[i + 1];
				avTriangles[i + 2] = (int)triangleIndices[i];
			}

			if(bMirrored)
			{
				Array.Reverse(avTriangles);
			}

			return true;
		}

		return false;
	}
	
	public bool IsSpeechRecognitionAvailable(ref bool bNeedRestart)
	{
		bool bOneCopied = false, bAllCopied = true;
		
		if(!KinectInterop.Is64bitArchitecture())
		{
			//Debug.Log("Speech - x32-architecture.");
			string sTargetPath = KinectInterop.GetTargetDllPath(".", false) + "/";

			Dictionary<string, string> dictFilesToUnzip = new Dictionary<string, string>();
			dictFilesToUnzip["Kinect2SpeechWrapper.dll"] = sTargetPath + "Kinect2SpeechWrapper.dll";
			dictFilesToUnzip["msvcp110.dll"] = sTargetPath + "msvcp110.dll";
			dictFilesToUnzip["msvcr110.dll"] = sTargetPath + "msvcr110.dll";
			
			KinectInterop.UnzipResourceFiles(dictFilesToUnzip, "KinectV2UnityAddin.x86.zip", ref bOneCopied, ref bAllCopied);
		}
		else
		{
			//Debug.Log("Face - x64-architecture.");
			string sTargetPath = KinectInterop.GetTargetDllPath(".", true) + "/";

			Dictionary<string, string> dictFilesToUnzip = new Dictionary<string, string>();
			dictFilesToUnzip["Kinect2SpeechWrapper.dll"] = sTargetPath + "Kinect2SpeechWrapper.dll";
			dictFilesToUnzip["msvcp110.dll"] = sTargetPath + "msvcp110.dll";
			dictFilesToUnzip["msvcr110.dll"] = sTargetPath + "msvcr110.dll";
			
			KinectInterop.UnzipResourceFiles(dictFilesToUnzip, "KinectV2UnityAddin.x64.zip", ref bOneCopied, ref bAllCopied);
		}
		
		bNeedRestart = (bOneCopied && bAllCopied);
		
		return true;
	}
	
	public int InitSpeechRecognition(string sRecoCriteria, bool bUseKinect, bool bAdaptationOff)
	{
//		if(kinectSensor != null)
//		{
//			float fWaitTime = Time.realtimeSinceStartup + 5f;
//
//			while(!kinectSensor.IsAvailable && Time.realtimeSinceStartup < fWaitTime)
//			{
//				// wait
//			}
//		}
		
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
		bBackgroundRemovalInited = KinectInterop.IsOpenCvAvailable(ref bNeedRestart);
		return bBackgroundRemovalInited;
	}
	
	public bool InitBackgroundRemoval(KinectInterop.SensorData sensorData, bool isHiResPrefered)
	{
		return KinectInterop.InitBackgroundRemoval(sensorData, isHiResPrefered);
	}
	
	public void FinishBackgroundRemoval(KinectInterop.SensorData sensorData)
	{
		KinectInterop.FinishBackgroundRemoval(sensorData);
		bBackgroundRemovalInited = false;
	}
	
	public bool UpdateBackgroundRemoval(KinectInterop.SensorData sensorData, bool isHiResPrefered, Color32 defaultColor)
	{
		return KinectInterop.UpdateBackgroundRemoval(sensorData, isHiResPrefered, defaultColor);
	}

	public bool IsBackgroundRemovalActive()
	{
		return bBackgroundRemovalInited;
	}

	public bool IsBRHiResSupported()
	{
		return true;
	}
	
	public Rect GetForegroundFrameRect(KinectInterop.SensorData sensorData, bool isHiResPrefered)
	{
		return KinectInterop.GetForegroundFrameRect(sensorData, isHiResPrefered);
	}
	
	public int GetForegroundFrameLength(KinectInterop.SensorData sensorData, bool isHiResPrefered)
	{
		return KinectInterop.GetForegroundFrameLength(sensorData, isHiResPrefered);
	}
	
	public bool PollForegroundFrame(KinectInterop.SensorData sensorData, bool isHiResPrefered, Color32 defaultColor, bool bLimitedUsers, ICollection<int> alTrackedIndexes, ref byte[] foregroundImage)
	{
		return KinectInterop.PollForegroundFrame(sensorData, isHiResPrefered, defaultColor, bLimitedUsers, alTrackedIndexes, ref foregroundImage);
	}
	
}
