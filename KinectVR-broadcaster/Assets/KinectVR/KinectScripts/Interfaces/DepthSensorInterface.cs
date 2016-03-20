using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public interface DepthSensorInterface
{
	// returns the depth sensor platform
	KinectInterop.DepthSensorPlatform GetSensorPlatform();

	// initializes libraries and resources needed by this sensor interface
	// returns true if the resources are successfully initialized, false otherwise
	bool InitSensorInterface(bool bCopyLibs, ref bool bNeedRestart);

	// releases the resources and libraries used by this interface
	void FreeSensorInterface(bool bDeleteLibs);

	// checks if there is available sensor on this interface
	// returns true if there are available sensors on this interface, false otherwise
	bool IsSensorAvailable();
	
	// returns the number of available sensors, controlled by this interface
	int GetSensorsCount();

	// opens the default sensor and inits needed resources. returns new sensor-data object
	KinectInterop.SensorData OpenDefaultSensor(KinectInterop.FrameSource dwFlags, float sensorAngle, bool bUseMultiSource);

	// closes the sensor and frees used resources
	void CloseSensor(KinectInterop.SensorData sensorData);

	// this method is invoked periodically to update sensor data, if needed
	// returns true if update is successful, false otherwise
	bool UpdateSensorData(KinectInterop.SensorData sensorData);

	// gets next multi source frame, if one is available
	// returns true if there is a new multi-source frame, false otherwise
	bool GetMultiSourceFrame(KinectInterop.SensorData sensorData);

	// frees the resources taken by the last multi-source frame
	void FreeMultiSourceFrame(KinectInterop.SensorData sensorData);

	// polls for new body/skeleton frame. must fill in all needed body and joints' elements (tracking state and position)
	// returns true if new body frame is available, false otherwise
	bool PollBodyFrame(KinectInterop.SensorData sensorData, ref KinectInterop.BodyFrameData bodyFrame, ref Matrix4x4 kinectToWorld, bool bIgnoreJointZ);

	// polls for new color frame data
	// returns true if new color frame is available, false otherwise
	bool PollColorFrame(KinectInterop.SensorData sensorData);

	// polls for new depth and body index frame data
	// returns true if new depth or body index frame is available, false otherwise
	bool PollDepthFrame(KinectInterop.SensorData sensorData);

	// polls for new infrared frame data
	// returns true if new infrared frame is available, false otherwise
	bool PollInfraredFrame(KinectInterop.SensorData sensorData);

	// performs sensor-specific fixes of joint positions and orientations
	void FixJointOrientations(KinectInterop.SensorData sensorData, ref KinectInterop.BodyData bodyData);

	// checks if the given body is turned around or not
	bool IsBodyTurned(ref KinectInterop.BodyData bodyData);

	// returns depth frame coordinates for the given 3d space point
	Vector2 MapSpacePointToDepthCoords(KinectInterop.SensorData sensorData, Vector3 spacePos);

	// returns 3d Kinect-space coordinates for the given depth frame point
	Vector3 MapDepthPointToSpaceCoords(KinectInterop.SensorData sensorData, Vector2 depthPos, ushort depthVal);

	// returns color-space coordinates for the given depth point
	Vector2 MapDepthPointToColorCoords(KinectInterop.SensorData sensorData, Vector2 depthPos, ushort depthVal);

	// estimates all color-space coordinates for the current depth frame
	// returns true on success, false otherwise
	bool MapDepthFrameToColorCoords(KinectInterop.SensorData sensorData, ref Vector2[] vColorCoords);

	// estimates all depth-space coordinates for the current color frame
	// returns true on success, false otherwise
	bool MapColorFrameToDepthCoords (KinectInterop.SensorData sensorData, ref Vector2[] vDepthCoords);

	// returns the index of the given joint in joint's array
	int GetJointIndex(KinectInterop.JointType joint);
	
//	// returns the joint at given index
//	KinectInterop.JointType GetJointAtIndex(int index);
	
	// returns the parent joint of the given joint
	KinectInterop.JointType GetParentJoint(KinectInterop.JointType joint);
	
	// returns the next joint in the hierarchy, as to the given joint
	KinectInterop.JointType GetNextJoint(KinectInterop.JointType joint);

	// returns true if the face tracking is supported by this interface, false otherwise
	bool IsFaceTrackingAvailable(ref bool bNeedRestart);

	// initializes libraries and resources needed by the face tracking subsystem
	bool InitFaceTracking(bool bUseFaceModel, bool bDrawFaceRect);

	// releases the resources and libraries used by the face tracking subsystem
	void FinishFaceTracking();

	// this method gets invoked periodically to update the face tracking state
	// returns true if update is successful, false otherwise
	bool UpdateFaceTracking();

	// returns true if face tracking is initialized, false otherwise
	bool IsFaceTrackingActive();

	// returns true if face rectangle(s) must be drawn in color map, false otherwise
	bool IsDrawFaceRect();

	// returns true if the face of the specified user is being tracked at the moment, false otherwise
	bool IsFaceTracked(long userId);

	// gets the face rectangle in color coordinates. returns true on success, false otherwise
	bool GetFaceRect(long userId, ref Rect faceRect);

	// visualizes face tracker debug information
	void VisualizeFaceTrackerOnColorTex(Texture2D texColor);

	// gets the head position of the specified user. returns true on success, false otherwise
	bool GetHeadPosition(long userId, ref Vector3 headPos);
	
	// gets the head rotation of the specified user. returns true on success, false otherwise
	bool GetHeadRotation(long userId, ref Quaternion headRot);

	// gets the AU values for the specified user. returns true on success, false otherwise
	bool GetAnimUnits(long userId, ref Dictionary<KinectInterop.FaceShapeAnimations, float> afAU);

	// gets the SU values for the specified user. returns true on success, false otherwise
	bool GetShapeUnits(long userId, ref Dictionary<KinectInterop.FaceShapeDeformations, float> afSU);

	// returns the length of model's vertices array for the specified user
	int GetFaceModelVerticesCount(long userId);

	// gets the model vertices for the specified user. returns true on success, false otherwise
	bool GetFaceModelVertices(long userId, ref Vector3[] avVertices);
	
	// returns the length of model's triangles array
	int GetFaceModelTrianglesCount();
	
	// gets the model triangle indices. returns true on success, false otherwise
	bool GetFaceModelTriangles(bool bMirrored, ref int[] avTriangles);

	// returns true if the face tracking is supported by this interface, false otherwise
	bool IsSpeechRecognitionAvailable(ref bool bNeedRestart);

	// initializes libraries and resources needed by the speech recognition subsystem
	int InitSpeechRecognition(string sRecoCriteria, bool bUseKinect, bool bAdaptationOff);

	// releases the resources and libraries used by the speech recognition subsystem
	void FinishSpeechRecognition();
	
	// this method gets invoked periodically to update the speech recognition state
	// returns true if update is successful, false otherwise
	int UpdateSpeechRecognition();

	// loads new grammar file with the specified language code
	int LoadSpeechGrammar(string sFileName, short iLangCode, bool bDynamic);

	// adds a phrase to the from-rule in dynamic grammar. if the to-rule is empty, this means end of the phrase recognition
	int AddGrammarPhrase(string sFromRule, string sToRule, string sPhrase, bool bClearRulePhrases, bool bCommitGrammar);

	// sets the required confidence of the recognized phrases (must be between 0.0f and 1.0f)
	void SetSpeechConfidence(float fConfidence);

	// returns true if speech start has been detected, false otherwise
	bool IsSpeechStarted();

	// returns true if speech end has been detected, false otherwise
	bool IsSpeechEnded();

	// returns true if a grammar phrase has been recognized, false otherwise
	bool IsPhraseRecognized();

	// returns the confidence of the currently recognized phrase, in range [0, 1]
	float GetPhraseConfidence();

	// returns the tag of the recognized grammar phrase, empty string if no phrase is recognized at the moment
	string GetRecognizedPhraseTag();

	// clears the currently recognized grammar phrase (prepares SR system for next phrase recognition)
	void ClearRecognizedPhrase();

	// returns true if the background removal is supported by this interface, false otherwise
	bool IsBackgroundRemovalAvailable(ref bool bNeedRestart);
	
	// initializes libraries and resources needed by the background removal subsystem
	bool InitBackgroundRemoval(KinectInterop.SensorData sensorData, bool isHiResPrefered);
	
	// releases the resources and libraries used by the background removal subsystem
	void FinishBackgroundRemoval(KinectInterop.SensorData sensorData);
	
	// this method gets invoked periodically to update the background removal
	// returns true if update is successful, false otherwise
	bool UpdateBackgroundRemoval(KinectInterop.SensorData sensorData, bool isHiResPrefered, Color32 defaultColor);
	
	// returns true if background removal is initialized, false otherwise
	bool IsBackgroundRemovalActive();

	// returns true if BR-manager supports high resolution background removal
	bool IsBRHiResSupported();

	// returns the rectange of the foreground frame
	Rect GetForegroundFrameRect(KinectInterop.SensorData sensorData, bool isHiResPrefered);
	
	// returns the length of the foreground frame in bytes
	int GetForegroundFrameLength(KinectInterop.SensorData sensorData, bool isHiResPrefered);
	
	// polls for new foreground frame data
	// returns true if foreground frame is available, false otherwise
	bool PollForegroundFrame(KinectInterop.SensorData sensorData, bool isHiResPrefered, Color32 defaultColor, bool bLimitedUsers, ICollection<int> alTrackedIndexes, ref byte[] foregroundImage);
	
}
