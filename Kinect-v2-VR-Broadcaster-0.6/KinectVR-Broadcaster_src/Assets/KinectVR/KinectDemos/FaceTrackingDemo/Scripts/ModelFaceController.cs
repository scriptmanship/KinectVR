using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ModelFaceController : MonoBehaviour 
{
	public enum AxisEnum { X, Y, Z };
	
	[Tooltip("Transform of the joint, used to move and rotate the head.")]
	public Transform HeadTransform;

	[Tooltip("Whether the model's head is facing the player or not.")]
	public bool mirroredHeadMovement = true;

	[Tooltip("Smooth factor used for head movement and head-joint rotations.")]
	public float smoothFactor = 5f;
	
	// Upper Lip Left
	[Tooltip("Left upper lip joint.")]
	public Transform UpperLipLeft;
	[Tooltip("Left upper lip axis of rotation.")]
	public AxisEnum UpperLipLeftAxis;
	[Tooltip("Maximum up-value for the left upper lip, down-value is the opposite one.")]
	public float UpperLipLeftUp;

	// Upper Lip Right
	[Tooltip("Right upper lip joint.")]
	public Transform UpperLipRight;
	[Tooltip("Right upper lip axis of rotation.")]
	public AxisEnum UpperLipRightAxis;
	[Tooltip("Maximum up-value for the right upper lip, down-value is the opposite one.")]
	public float UpperLipRightUp;

	// Jaw
	[Tooltip("Jaw (mouth) joint.")]
	public Transform Jaw;
	[Tooltip("Jaw axis of rotation.")]
	public AxisEnum JawAxis;
	[Tooltip("Maximum down-value for the jaw, up-value is the opposite one.")]
	public float JawDown;
	
	// Lip Left
	[Tooltip("Left lip joint.")]
	public Transform LipLeft;
	[Tooltip("Left lip axis of rotation.")]
	public AxisEnum LipLeftAxis;
	[Tooltip("Maximum stretched-value for the left lip, rounded-value is the opposite one.")]
	public float LipLeftStretched;

	// Lip Right
	[Tooltip("Right lip joint.")]
	public Transform LipRight;
	[Tooltip("Right lip axis of rotation.")]
	public AxisEnum LipRightAxis;
	[Tooltip("Maximum stretched-value for the right lip, rounded-value is the opposite one.")]
	public float LipRightStretched;

	// Eyebrow Left
	[Tooltip("Left eyebrow joint.")]
	public Transform EyebrowLeft;
	[Tooltip("Left eyebrow axis of rotation.")]
	public AxisEnum EyebrowLeftAxis;
	[Tooltip("Maximum lowered-value for the left eyebrow, raised-value is the opposite one.")]
	public float EyebrowLeftLowered;

	// Eyebrow Right
	[Tooltip("Right eyebrow joint.")]
	public Transform EyebrowRight;
	[Tooltip("Right eyebrow axis of rotation.")]
	public AxisEnum EyebrowRightAxis;
	[Tooltip("Maximum lowered-value for the right eyebrow, raised-value is the opposite one.")]
	public float EyebrowRightLowered;
	
	// Lip Corner Left
	[Tooltip("Left lip-corner joint.")]
	public Transform LipCornerLeft;
	[Tooltip("Left lip-corner axis of rotation.")]
	public AxisEnum LipCornerLeftAxis;
	[Tooltip("Maximum depressed-value for the left lip-corner, smile-value is the opposite one.")]
	public float LipCornerLeftDepressed;

	// Lip Corner Right
	[Tooltip("Right lip-corner joint.")]
	public Transform LipCornerRight;
	[Tooltip("Right lip-corner axis of rotation.")]
	public AxisEnum LipCornerRightAxis;
	[Tooltip("Maximum depressed-value for the right lip-corner, smile-value is the opposite one.")]
	public float LipCornerRightDepressed;

	// Upper Eyelid Left
	[Tooltip("Left upper eyelid joint.")]
	public Transform UpperEyelidLeft;
	[Tooltip("Left upper eyelid axis of rotation.")]
	public AxisEnum UpperEyelidLeftAxis;
	[Tooltip("Maximum lowered-value for the left upper eyelid, raised-value is the opposite one.")]
	public float UpperEyelidLeftLowered;

	// Upper Eyelid Right
	[Tooltip("Right upper eyelid joint.")]
	public Transform UpperEyelidRight;
	[Tooltip("Right upper eyelid axis of rotation.")]
	public AxisEnum UpperEyelidRightAxis;
	[Tooltip("Maximum lowered-value for the right upper eyelid, raised-value is the opposite one.")]
	public float UpperEyelidRightLowered;
	
	// Lower Eyelid Left
	[Tooltip("Left lower eyelid joint.")]
	public Transform LowerEyelidLeft;
	[Tooltip("Left lower eyelid axis of rotation.")]
	public AxisEnum LowerEyelidLeftAxis;
	[Tooltip("Maximum raised-value for the left lower eyelid, lowered-value is the opposite one.")]
	public float LowerEyelidLeftRaised;

	// Lower Eyelid Right
	[Tooltip("Right lower eyelid joint.")]
	public Transform LowerEyelidRight;
	[Tooltip("Right lower eyelid axis of rotation.")]
	public AxisEnum LowerEyelidRightAxis;
	[Tooltip("Maximum raised-value for the right lower eyelid, lowered-value is the opposite one.")]
	public float LowerEyelidRightRaised;
	
	
	private FacetrackingManager manager;
	private KinectInterop.DepthSensorPlatform platform;
	
	private Vector3 HeadInitialPosition;
	private Quaternion HeadInitialRotation;
	
	private float UpperLipLeftNeutral;
	private float UpperLipRightNeutral;
	private float JawNeutral;
	private float LipLeftNeutral;
	private float LipRightNeutral;
	private float EyebrowLeftNeutral;
	private float EyebrowRightNeutral;
	private float LipCornerLeftNeutral;
	private float LipCornerRightNeutral;
	private float UpperEyelidLeftNeutral;
	private float UpperEyelidRightNeutral;
	private float LowerEyelidLeftNeutral;
	private float LowerEyelidRightNeutral;

	
	void Start()
	{
		if(HeadTransform != null)
		{
			HeadInitialPosition = HeadTransform.localPosition;
			//HeadInitialPosition.z = 0;
			HeadInitialRotation = HeadTransform.localRotation;
		}
		
		UpperLipLeftNeutral = GetJointRotation(UpperLipLeft, UpperLipLeftAxis);
		UpperLipRightNeutral = GetJointRotation(UpperLipRight, UpperLipRightAxis);
		
		JawNeutral = GetJointRotation(Jaw, JawAxis);
		
		LipLeftNeutral = GetJointRotation(LipLeft, LipLeftAxis);
		LipRightNeutral = GetJointRotation(LipRight, LipRightAxis);
		
		EyebrowLeftNeutral = GetJointRotation(EyebrowLeft, EyebrowLeftAxis);
		EyebrowRightNeutral = GetJointRotation(EyebrowRight, EyebrowRightAxis);
		
		LipCornerLeftNeutral = GetJointRotation(LipCornerLeft, LipCornerLeftAxis);
		LipCornerRightNeutral = GetJointRotation(LipCornerRight, LipCornerRightAxis);
		
		UpperEyelidLeftNeutral = GetJointRotation(UpperEyelidLeft, UpperEyelidLeftAxis);
		UpperEyelidRightNeutral = GetJointRotation(UpperEyelidRight, UpperEyelidRightAxis);

		LowerEyelidLeftNeutral = GetJointRotation(LowerEyelidLeft, LowerEyelidLeftAxis);
		LowerEyelidRightNeutral = GetJointRotation(LowerEyelidRight, LowerEyelidRightAxis);

		KinectManager kinectManager = KinectManager.Instance;
		if(kinectManager && kinectManager.IsInitialized())
		{
			platform = kinectManager.GetSensorPlatform();
		}
	}
	
	void Update() 
	{
		// get the face-tracking manager instance
		if(manager == null)
		{
			manager = FacetrackingManager.Instance;
		}

		if(manager && manager.IsTrackingFace())
		{
			// set head position & rotation
			if(HeadTransform != null)
			{
				// head position
				Vector3 newPosition = HeadInitialPosition + manager.GetHeadPosition(mirroredHeadMovement);

				if(smoothFactor != 0f)
					HeadTransform.localPosition = Vector3.Lerp(HeadTransform.localPosition, newPosition, smoothFactor * Time.deltaTime);
				else
					HeadTransform.localPosition = newPosition;

				// head rotation
				Quaternion newRotation = HeadInitialRotation * manager.GetHeadRotation(mirroredHeadMovement);

				if(smoothFactor != 0f)
					HeadTransform.localRotation = Quaternion.Slerp(HeadTransform.localRotation, newRotation, smoothFactor * Time.deltaTime);
				else
					HeadTransform.localRotation = newRotation;
			}
			
			// apply animation units

			// AU0 - Upper Lip Raiser
			// 0=neutral, covering teeth; 1=showing teeth fully; -1=maximal possible pushed down lip
			float fAU0 = manager.GetAnimUnit(KinectInterop.FaceShapeAnimations.LipPucker);
			SetJointRotation(UpperLipLeft, UpperLipLeftAxis, fAU0, UpperLipLeftNeutral, UpperLipLeftUp);
			SetJointRotation(UpperLipRight, UpperLipRightAxis, fAU0, UpperLipRightNeutral, UpperLipRightUp);
			
			// AU1 - Jaw Lowerer
			// 0=closed; 1=fully open; -1= closed, like 0
			float fAU1 = manager.GetAnimUnit(KinectInterop.FaceShapeAnimations.JawOpen);
			SetJointRotation(Jaw, JawAxis, fAU1, JawNeutral, JawDown);
			
			// AU2 – Lip Stretcher
			// 0=neutral; 1=fully stretched (joker’s smile); -1=fully rounded (kissing mouth)
			float fAU2_left = manager.GetAnimUnit(KinectInterop.FaceShapeAnimations.LipStretcherLeft);
			fAU2_left = (platform == KinectInterop.DepthSensorPlatform.KinectSDKv2) ? (fAU2_left * 2 - 1) : fAU2_left;
			SetJointRotation(LipLeft, LipLeftAxis, fAU2_left, LipLeftNeutral, LipLeftStretched);

			float fAU2_right = manager.GetAnimUnit(KinectInterop.FaceShapeAnimations.LipStretcherRight);
			fAU2_right = (platform == KinectInterop.DepthSensorPlatform.KinectSDKv2) ? (fAU2_right * 2 - 1) : fAU2_right;
			SetJointRotation(LipRight, LipRightAxis, fAU2_right, LipRightNeutral, LipRightStretched);
			
			// AU3 – Brow Lowerer
			// 0=neutral; -1=raised almost all the way; +1=fully lowered (to the limit of the eyes)
			float fAU3_left = manager.GetAnimUnit(KinectInterop.FaceShapeAnimations.LefteyebrowLowerer);
			fAU3_left = (platform == KinectInterop.DepthSensorPlatform.KinectSDKv2) ? (fAU3_left * 2 - 1) : fAU3_left;
			SetJointRotation(EyebrowLeft, EyebrowLeftAxis, fAU3_left, EyebrowLeftNeutral, EyebrowLeftLowered);

			float fAU3_right = manager.GetAnimUnit(KinectInterop.FaceShapeAnimations.RighteyebrowLowerer);
			fAU3_right = (platform == KinectInterop.DepthSensorPlatform.KinectSDKv2) ? (fAU3_right * 2 - 1) : fAU3_right;
			SetJointRotation(EyebrowRight, EyebrowRightAxis, fAU3_right, EyebrowRightNeutral, EyebrowRightLowered);
			
			// AU4 – Lip Corner Depressor
			// 0=neutral; -1=very happy smile; +1=very sad frown
			float fAU4_left = manager.GetAnimUnit(KinectInterop.FaceShapeAnimations.LipCornerDepressorLeft);
			fAU4_left = (platform == KinectInterop.DepthSensorPlatform.KinectSDKv2) ? (fAU4_left * 2) : fAU4_left;
			SetJointRotation(LipCornerLeft, LipCornerLeftAxis, fAU4_left, LipCornerLeftNeutral, LipCornerLeftDepressed);

			float fAU4_right = manager.GetAnimUnit(KinectInterop.FaceShapeAnimations.LipCornerDepressorRight);
			fAU4_right = (platform == KinectInterop.DepthSensorPlatform.KinectSDKv2) ? (fAU4_right * 2) : fAU4_right;
			SetJointRotation(LipCornerRight, LipCornerRightAxis, fAU4_right, LipCornerRightNeutral, LipCornerRightDepressed);

			// AU6, AU7 – Eyelid closed
			// 0=neutral; -1=raised; +1=fully lowered
			float fAU6_left = manager.GetAnimUnit(KinectInterop.FaceShapeAnimations.LefteyeClosed);
			fAU6_left = (platform == KinectInterop.DepthSensorPlatform.KinectSDKv2) ? (fAU6_left * 2 - 1) : fAU6_left;
			SetJointRotation(UpperEyelidLeft, UpperEyelidLeftAxis, fAU6_left, UpperEyelidLeftNeutral, UpperEyelidLeftLowered);
			SetJointRotation(LowerEyelidLeft, LowerEyelidLeftAxis, fAU6_left, LowerEyelidLeftNeutral, LowerEyelidLeftRaised);

			float fAU6_right = manager.GetAnimUnit(KinectInterop.FaceShapeAnimations.RighteyeClosed);
			fAU6_right = (platform == KinectInterop.DepthSensorPlatform.KinectSDKv2) ? (fAU6_right * 2 - 1) : fAU6_right;
			SetJointRotation(UpperEyelidRight, UpperEyelidRightAxis, fAU6_right, UpperEyelidRightNeutral, UpperEyelidRightLowered);
			SetJointRotation(LowerEyelidRight, LowerEyelidRightAxis, fAU6_right, LowerEyelidRightNeutral, LowerEyelidRightRaised);
		}
	}
	
	private float GetJointRotation(Transform joint, AxisEnum axis)
	{
		float fJointRot = 0.0f;
		
		if(joint == null)
			return fJointRot;
		
		Vector3 jointRot = joint.localRotation.eulerAngles;
		
		switch(axis)
		{
			case AxisEnum.X:
				fJointRot = jointRot.x;
				break;
			
			case AxisEnum.Y:
				fJointRot = jointRot.y;
				break;
			
			case AxisEnum.Z:
				fJointRot = jointRot.z;
				break;
		}
		
		return fJointRot;
	}
	
	private void SetJointRotation(Transform joint, AxisEnum axis, float fAU, float fMin, float fMax)
	{
		if(joint == null)
			return;
		
//		float fSign = 1.0f;
//		if(fMax < fMin)
//			fSign = -1.0f;
		
		// [-1, +1] -> [0, 1]
		//fAUnorm = (fAU + 1f) / 2f;
		float fValue = fMin + (fMax - fMin) * fAU;
		
		Vector3 jointRot = joint.localRotation.eulerAngles;
		
		switch(axis)
		{
			case AxisEnum.X:
				jointRot.x = fValue;
				break;
			
			case AxisEnum.Y:
				jointRot.y = fValue;
				break;
			
			case AxisEnum.Z:
				jointRot.z = fValue;
				break;
		}

		if(smoothFactor != 0f)
			joint.localRotation = Quaternion.Slerp(joint.localRotation, Quaternion.Euler(jointRot), smoothFactor * Time.deltaTime);
		else
			joint.localRotation = Quaternion.Euler(jointRot);
	}
	
	
}
