using UnityEngine;
using System.Collections;
using System.IO;

public class GetJointPositionDemo : MonoBehaviour 
{
	[Tooltip("The Kinect joint we want to track.")]
	public KinectInterop.JointType joint = KinectInterop.JointType.HandRight;

	[Tooltip("Current joint position in Kinect coordinates (meters).")]
	public Vector3 jointPosition;

	[Tooltip("Whether we save the joint data to a CSV file or not.")]
	public bool isSaving = false;

	[Tooltip("Path to the CSV file, we want to save the joint data to.")]
	public string saveFilePath = "joint_pos.csv";
	
	[Tooltip("How many seconds to save data to the CSV file, or 0 to save non-stop.")]
	public float secondsToSave = 0f;


	// start time of data saving to csv file
	private float saveStartTime = -1f;


	void Start()
	{
		if(isSaving && File.Exists(saveFilePath))
		{
			File.Delete(saveFilePath);
		}
	}


	void Update () 
	{
		if(isSaving)
		{
			// create the file, if needed
			if(!File.Exists(saveFilePath))
			{
				using(StreamWriter writer = File.CreateText(saveFilePath))
				{
					// csv file header
					string sLine = "time,joint,pos_x,pos_y,poz_z";
					writer.WriteLine(sLine);
				}
			}

			// check the start time
			if(saveStartTime < 0f)
			{
				saveStartTime = Time.time;
			}
		}

		// get the joint position
		KinectManager manager = KinectManager.Instance;

		if(manager && manager.IsInitialized())
		{
			if(manager.IsUserDetected())
			{
				long userId = manager.GetPrimaryUserID();

				if(manager.IsJointTracked(userId, (int)joint))
				{
					// output the joint position for easy tracking
					Vector3 jointPos = manager.GetJointPosition(userId, (int)joint);
					jointPosition = jointPos;

					if(isSaving)
					{
						if((secondsToSave == 0f) || ((Time.time - saveStartTime) <= secondsToSave))
						{
							using(StreamWriter writer = File.AppendText(saveFilePath))
							{
								string sLine = string.Format("{0:F3},{1},{2:F3},{3:F3},{4:F3}", Time.time, ((KinectInterop.JointType)joint).ToString(), jointPos.x, jointPos.y, jointPos.z);
								writer.WriteLine(sLine);
							}
						}
					}
				}
			}
		}

	}

}
