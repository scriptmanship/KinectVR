using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Kinect.Face;


public class GetFacePointsDemo : MonoBehaviour 
{
	private KinectManager manager = null;
	private Kinect2Interface k2interface = null;

	private Dictionary<FacePointType, Point> facePoints;


	// returns the face point coordinates or Vector2.zero if not found
	public Vector2 GetFacePoint(FacePointType pointType)
	{
		if(facePoints != null && facePoints.ContainsKey(pointType))
		{
			Point msPoint = facePoints[pointType];
			return new Vector2(msPoint.X, msPoint.Y);
		}

		return Vector3.zero;
	}

	void Update () 
	{
		// get reference to the Kinect2Interface
		if(k2interface == null)
		{
			manager = KinectManager.Instance;
			
			if(manager && manager.IsInitialized())
			{
				KinectInterop.SensorData sensorData = manager.GetSensorData();
				
				if(sensorData != null && sensorData.sensorInterface != null)
				{
					k2interface = (Kinect2Interface)sensorData.sensorInterface;
				}
			}
		}

		// get the face points
		if(k2interface != null && k2interface.faceFrameResults != null)
		{
			if(manager != null && manager.IsUserDetected())
			{
				ulong userId = (ulong)manager.GetPrimaryUserID();
				
				for(int i = 0; i < k2interface.faceFrameResults.Length; i++)
				{
					if(k2interface.faceFrameResults[i] != null && k2interface.faceFrameResults[i].TrackingId == userId)
					{
						facePoints = k2interface.faceFrameResults[i].FacePointsInColorSpace;
						break;
					}
				}
			}
		}

	}


}
