using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]
public class AvatarScaler : MonoBehaviour 
{
	[Tooltip("Index of the player, tracked by this component. 0 means the 1st player, 1 - the 2nd one, 2 - the 3rd one, etc.")]
	public int playerIndex = 0;

	[Tooltip("Whether the avatar is facing the player or not.")]
	public bool mirroredAvatar = false;

	[Tooltip("Whole body scale factor (including arms and legs) that might be used for body-scale fine tuning.")]
	public float bodyScaleFactor = 1.03f;
	[Tooltip("Additional scale factor for arms that might be used for arms-scale fine tuning.")]
	public float armScaleFactor = 1.0f;
	[Tooltip("Additional scale factor for legs that might be used for legs-scale fine tuning.")]
	public float legScaleFactor = 0.9f;
	
	[Tooltip("Whether the scale is updated continuously or just after the calibration pose.")]
	public bool continuousScaling = false;
	
	[Tooltip("Scale smoothing factor used in case of continuous scaling.")]
	public float smoothFactor = 5f;

	[Tooltip("GUI-Text to display the avatar-scaler debug messages.")]
	public GUIText debugText;
	
	// used by the controller to store current userId
	[System.NonSerialized]
	public long currentUserId = 0;

	// model transforms for scaling
	private Transform bodyScaleTransform;
	private Transform leftShoulderScaleTransform;
	private Transform leftElbowScaleTransform;
	private Transform rightShoulderScaleTransform;
	private Transform rightElbowScaleTransform;
	private Transform leftHipScaleTransform;
	private Transform leftKneeScaleTransform;
	private Transform rightHipScaleTransform;
	private Transform rightKneeScaleTransform;

	private Vector3 modelBodyScale = Vector3.one;
	private Vector3 modelLeftShoulderScale = Vector3.one;
	private Vector3 modelLeftElbowScale = Vector3.one;
	private Vector3 modelRightShoulderScale = Vector3.one; 
	private Vector3 modelRightElbowScale = Vector3.one; 
	private Vector3 modelLeftHipScale = Vector3.one; 
	private Vector3 modelLeftKneeScale = Vector3.one; 
	private Vector3 modelRightHipScale = Vector3.one;
	private Vector3 modelRightKneeScale = Vector3.one;
	
	// model bone sizes and original scales
	private float modelBodyHeight = 0f;
	private float modelLeftUpperArmLength = 0f;
	private float modelLeftLowerArmLength = 0f;
	private float modelRightUpperArmLength = 0f;
	private float modelRightLowerArmLength = 0f;
	private float modelLeftUpperLegLength = 0f;
	private float modelLeftLowerLegLength = 0f;
	private float modelRightUpperLegLength = 0f;
	private float modelRightLowerLegLength = 0f;
	
	// user bone sizes
	private float bodyHeight = 0f;
	private float leftUpperArmLength = 0f; 
	private float leftLowerArmLength = 0f; 
	private float rightUpperArmLength = 0f;
	private float rightLowerArmLength = 0f;
	private float leftUpperLegLength = 0f; 
	private float leftLowerLegLength = 0f; 
	private float rightUpperLegLength = 0f;
	private float rightLowerLegLength = 0f;

	// user bone scale factors
	private float fScaleBody = 0f;
	private float fScaleLeftUpperArm = 0f;
	private float fScaleLeftLowerArm = 0f;
	private float fScaleRightUpperArm = 0f;
	private float fScaleRightLowerArm = 0f;
	private float fScaleLeftUpperLeg = 0f;
	private float fScaleLeftLowerLeg = 0f;
	private float fScaleRightUpperLeg = 0f;
	private float fScaleRightLowerLeg = 0f;


	public void Start () 
	{
		// calculate model body sizes;
		Animator animatorComponent = GetComponent<Animator>();

		if(animatorComponent)
		{
			// get scale transforms
			bodyScaleTransform = animatorComponent.GetBoneTransform(HumanBodyBones.Hips);

			leftShoulderScaleTransform = animatorComponent.GetBoneTransform(HumanBodyBones.LeftUpperArm);
			leftElbowScaleTransform = animatorComponent.GetBoneTransform(HumanBodyBones.LeftLowerArm);
			rightShoulderScaleTransform = animatorComponent.GetBoneTransform(HumanBodyBones.RightUpperArm);
			rightElbowScaleTransform = animatorComponent.GetBoneTransform(HumanBodyBones.RightLowerArm);

			leftHipScaleTransform = animatorComponent.GetBoneTransform(HumanBodyBones.LeftUpperLeg);
			leftKneeScaleTransform = animatorComponent.GetBoneTransform(HumanBodyBones.LeftLowerLeg);
			rightHipScaleTransform = animatorComponent.GetBoneTransform(HumanBodyBones.RightUpperLeg);
			rightKneeScaleTransform = animatorComponent.GetBoneTransform(HumanBodyBones.RightLowerLeg);

			// get model bone scales
			modelBodyScale = bodyScaleTransform ? bodyScaleTransform.localScale : Vector3.one;

			modelLeftShoulderScale = leftShoulderScaleTransform ? leftShoulderScaleTransform.localScale : Vector3.one;
			modelLeftElbowScale = leftElbowScaleTransform ? leftElbowScaleTransform.localScale : Vector3.one;
			modelRightShoulderScale = rightShoulderScaleTransform ? rightShoulderScaleTransform.localScale : Vector3.one;
			modelRightElbowScale = rightElbowScaleTransform ? rightElbowScaleTransform.localScale : Vector3.one;

			modelLeftHipScale = leftHipScaleTransform ? leftHipScaleTransform.localScale : Vector3.one;
			modelLeftKneeScale = leftKneeScaleTransform ? leftKneeScaleTransform.localScale : Vector3.one;
			modelRightHipScale = rightHipScaleTransform ? rightHipScaleTransform.localScale : Vector3.one;
			modelRightKneeScale = rightKneeScaleTransform ? rightKneeScaleTransform.localScale : Vector3.one;

			GetModelBodyHeight(animatorComponent, ref modelBodyHeight);

			GetModelBoneLength(animatorComponent, HumanBodyBones.LeftUpperArm, HumanBodyBones.LeftLowerArm, ref modelLeftUpperArmLength);
			GetModelBoneLength(animatorComponent, HumanBodyBones.LeftLowerArm, HumanBodyBones.LeftHand, ref modelLeftLowerArmLength);
			GetModelBoneLength(animatorComponent, HumanBodyBones.RightUpperArm, HumanBodyBones.RightLowerArm, ref modelRightUpperArmLength);
			GetModelBoneLength(animatorComponent, HumanBodyBones.RightLowerArm, HumanBodyBones.RightHand, ref modelRightLowerArmLength);

			GetModelBoneLength(animatorComponent, HumanBodyBones.LeftUpperLeg, HumanBodyBones.LeftLowerLeg, ref modelLeftUpperLegLength);
			GetModelBoneLength(animatorComponent, HumanBodyBones.LeftLowerLeg, HumanBodyBones.LeftFoot, ref modelLeftLowerLegLength);
			GetModelBoneLength(animatorComponent, HumanBodyBones.RightUpperLeg, HumanBodyBones.RightLowerLeg, ref modelRightUpperLegLength);
			GetModelBoneLength(animatorComponent, HumanBodyBones.RightLowerLeg, HumanBodyBones.RightFoot, ref modelRightLowerLegLength);
		}

//		// copy the mirroring-flag from the avatar controller component
//		AvatarController avatarController = GetComponent<AvatarController>();
//
//		if(avatarController)
//		{
//			playerIndex = avatarController.playerIndex;
//			mirroredAvatar = avatarController.mirroredMovement;
//		}

	}
	
	void Update () 
	{
		if(continuousScaling)
		{
			GetUserBodySize(true, true, true);
			ScaleAvatar(smoothFactor);
		}
	}

	// gets the the actual sizes of the user bones
	public void GetUserBodySize(bool bBody, bool bArms, bool bLegs)
	{
		KinectManager manager = KinectManager.Instance;
		if(manager == null)
			return;
		
		if(bBody)
		{
			GetUserBodyHeight(manager, bodyScaleFactor, ref bodyHeight);
		}
		
		if(bArms)
		{
			GetUserBoneLength(manager, KinectInterop.JointType.ShoulderLeft, KinectInterop.JointType.ElbowLeft, armScaleFactor, ref leftUpperArmLength);
			GetUserBoneLength(manager, KinectInterop.JointType.ElbowLeft, KinectInterop.JointType.WristLeft, armScaleFactor, ref leftLowerArmLength);
			GetUserBoneLength(manager, KinectInterop.JointType.ShoulderRight, KinectInterop.JointType.ElbowRight, armScaleFactor, ref rightUpperArmLength);
			GetUserBoneLength(manager, KinectInterop.JointType.ElbowRight, KinectInterop.JointType.WristRight, armScaleFactor, ref rightLowerArmLength);

			EqualizeBoneLength(ref leftUpperArmLength, ref rightUpperArmLength);
			EqualizeBoneLength(ref leftLowerArmLength, ref rightLowerArmLength);
		}
		
		if(bLegs)
		{
			GetUserBoneLength(manager, KinectInterop.JointType.HipLeft, KinectInterop.JointType.KneeLeft, legScaleFactor, ref leftUpperLegLength);
			GetUserBoneLength(manager, KinectInterop.JointType.KneeLeft, KinectInterop.JointType.AnkleLeft, legScaleFactor, ref leftLowerLegLength);
			GetUserBoneLength(manager, KinectInterop.JointType.HipRight, KinectInterop.JointType.KneeRight, legScaleFactor, ref rightUpperLegLength);
			GetUserBoneLength(manager, KinectInterop.JointType.KneeRight, KinectInterop.JointType.AnkleRight, legScaleFactor, ref rightLowerLegLength);
			
			EqualizeBoneLength(ref leftUpperLegLength, ref rightUpperLegLength);
			EqualizeBoneLength(ref leftLowerLegLength, ref rightLowerLegLength);
		}
	}
	
	// scales the avatar as needed
	public void ScaleAvatar(float fSmooth)
	{
//		KinectManager manager = KinectManager.Instance;
//		if(!manager)
//			return;
//		
//		if(fSmooth != 0f && manager.IsUserTurnedAround(currentUserId))
//			return;

		// scale body
		SetupBoneScale(bodyScaleTransform, modelBodyScale, modelBodyHeight, 
		               bodyHeight, 0f, fSmooth, ref fScaleBody);

		// scale arms
		float fLeftUpperArmLength = !mirroredAvatar ? leftUpperArmLength : rightUpperArmLength;
		SetupBoneScale(leftShoulderScaleTransform, modelLeftShoulderScale, modelLeftUpperArmLength, 
		               fLeftUpperArmLength, fScaleBody, fSmooth, ref fScaleLeftUpperArm);

		float fLeftLowerArmLength = !mirroredAvatar ? leftLowerArmLength : rightLowerArmLength;
		SetupBoneScale(leftElbowScaleTransform, modelLeftElbowScale, modelLeftLowerArmLength, 
		               fLeftLowerArmLength, fScaleLeftUpperArm, fSmooth, ref fScaleLeftLowerArm);

		float fRightUpperArmLength = !mirroredAvatar ? rightUpperArmLength : leftUpperArmLength;
		SetupBoneScale(rightShoulderScaleTransform, modelRightShoulderScale, modelRightUpperArmLength, 
		               fRightUpperArmLength, fScaleBody, fSmooth, ref fScaleRightUpperArm);
		
		float fRightLowerArmLength = !mirroredAvatar ? rightLowerArmLength : leftLowerArmLength;
		SetupBoneScale(rightElbowScaleTransform, modelRightElbowScale, modelLeftLowerArmLength, 
		               fRightLowerArmLength, fScaleRightUpperArm, fSmooth, ref fScaleRightLowerArm);

		// scale legs
		float fLeftUpperLegLength = !mirroredAvatar ? leftUpperLegLength : rightUpperLegLength;
		SetupBoneScale(leftHipScaleTransform, modelLeftHipScale, modelLeftUpperLegLength, 
		               fLeftUpperLegLength, fScaleBody, fSmooth, ref fScaleLeftUpperLeg);
		
		float fLeftLowerLegLength = !mirroredAvatar ? leftLowerLegLength : rightLowerLegLength;
		SetupBoneScale(leftKneeScaleTransform, modelLeftKneeScale, modelLeftLowerLegLength, 
		               fLeftLowerLegLength, fScaleLeftUpperLeg, fSmooth, ref fScaleLeftLowerLeg);
		
		float fRightUpperLegLength = !mirroredAvatar ? rightUpperLegLength : leftUpperLegLength;
		SetupBoneScale(rightHipScaleTransform, modelRightHipScale, modelRightUpperLegLength, 
		               fRightUpperLegLength, fScaleBody, fSmooth, ref fScaleRightUpperLeg);
		
		float fRightLowerLegLength = !mirroredAvatar ? rightLowerLegLength : leftLowerLegLength;
		SetupBoneScale(rightKneeScaleTransform, modelRightKneeScale, modelRightLowerLegLength, 
		               fRightLowerLegLength, fScaleRightUpperLeg, fSmooth, ref fScaleRightLowerLeg);

		if(debugText != null)
		{
			string sDebug = string.Format("B: {0:F3}\nULA: {1:F3}, LLA: {2:F3}; RUA: {3:F3}, RLA: {4:F3}\nLUL: {5:F3}, LLL: {6:F3}; RUL: {7:F3}, RLL: {8:F3}",
			                              fScaleBody, fScaleLeftUpperArm, fScaleLeftLowerArm,
			                              fScaleRightUpperArm, fScaleRightLowerArm,
			                              fScaleLeftUpperLeg, fScaleLeftLowerLeg,
			                              fScaleRightUpperLeg, fScaleRightLowerLeg);
			debugText.GetComponent<GUIText>().text = sDebug;
		}
		
	}
	
	private bool GetModelBodyHeight(Animator animatorComponent, ref float height)
	{
		height = 0f;
		
		if(animatorComponent)
		{
			//Transform hipCenter = animatorComponent.GetBoneTransform(HumanBodyBones.Hips);

			Transform leftUpperArm = animatorComponent.GetBoneTransform(HumanBodyBones.LeftUpperArm);
			Transform rightUpperArm = animatorComponent.GetBoneTransform(HumanBodyBones.RightUpperArm);
			
			Transform leftUpperLeg = animatorComponent.GetBoneTransform(HumanBodyBones.LeftUpperLeg);
			Transform rightUpperLeg = animatorComponent.GetBoneTransform(HumanBodyBones.RightUpperLeg);
			
			if(leftUpperArm && rightUpperArm && leftUpperLeg && rightUpperLeg)
			{
				Vector3 posShoulderCenter = (leftUpperArm.position + rightUpperArm.position) / 2;
				Vector3 posHipCenter = (leftUpperLeg.position + rightUpperLeg.position) / 2;  // hipCenter.position

				height = (posShoulderCenter.y - posHipCenter.y);
				
				return true;
			}
		}
		
		return false;
	}
	
	private bool GetModelBoneLength(Animator animatorComponent, HumanBodyBones baseJoint, HumanBodyBones endJoint, ref float length)
	{
		length = 0f;
		
		if(animatorComponent)
		{
			Transform joint1 = animatorComponent.GetBoneTransform(baseJoint);
			Transform joint2 = animatorComponent.GetBoneTransform(endJoint);
			
			if(joint1 && joint2)
			{
				length = (joint2.position - joint1.position).magnitude;
				return true;
			}
		}
		
		return false;
	}
	
	private bool GetUserBodyHeight(KinectManager manager, float scaleFactor, ref float height)
	{
		height = 0f;
		
		if(manager && manager.IsJointTracked(currentUserId, (int)KinectInterop.JointType.HipLeft) &&
		   manager.IsJointTracked(currentUserId, (int)KinectInterop.JointType.HipRight) &&
		   manager.IsJointTracked(currentUserId, (int)KinectInterop.JointType.ShoulderLeft) &&
		   manager.IsJointTracked(currentUserId, (int)KinectInterop.JointType.ShoulderRight))
		{
			//Vector3 posHipCenter = manager.GetJointPosition(currentUserId, (int)KinectInterop.JointType.SpineBase);
			Vector3 posHipLeft = manager.GetJointPosition(currentUserId, (int)KinectInterop.JointType.HipLeft);
			Vector3 posHipRight = manager.GetJointPosition(currentUserId, (int)KinectInterop.JointType.HipRight);
			Vector3 posHipCenter = (posHipLeft + posHipRight) / 2;
			
			Vector3 posShoulderLeft = manager.GetJointPosition(currentUserId, (int)KinectInterop.JointType.ShoulderLeft);
			Vector3 posShoulderRight = manager.GetJointPosition(currentUserId, (int)KinectInterop.JointType.ShoulderRight);
			Vector3 posShoulderCenter = (posShoulderLeft + posShoulderRight) / 2;

			height = (posShoulderCenter.y - posHipCenter.y) * scaleFactor;
			
			return true;
		}

		return false;
	}
	
	private bool GetUserBoneLength(KinectManager manager, KinectInterop.JointType baseJoint, KinectInterop.JointType endJoint, float scaleFactor, ref float length)
	{
		length = 0f;
		
		if(manager && manager.IsJointTracked(currentUserId, (int)baseJoint) &&
		   manager.IsJointTracked(currentUserId, (int)endJoint))
		{
			Vector3 vPos1 = manager.GetJointPosition(currentUserId, (int)baseJoint);
			Vector3 vPos2 = manager.GetJointPosition(currentUserId, (int)endJoint);

			length = (vPos2 - vPos1).magnitude * scaleFactor;
			
			return true;
		}
		
		return false;
	}

	private void EqualizeBoneLength(ref float boneLen1, ref float boneLen2)
	{
		if(boneLen1 < boneLen2)
		{
			boneLen1 = boneLen2;
		}
		else
		{
			boneLen2 = boneLen1;
		}
	}
	
	private bool SetupBoneScale(Transform scaleTrans, Vector3 modelBoneScale, float modelBoneLen, float userBoneLen, float parentScale, float fSmooth, ref float boneScale)
	{
		if(modelBoneLen > 0f && userBoneLen > 0f)
		{
			boneScale = userBoneLen / modelBoneLen;
		}

		float localScale = boneScale;
		if(boneScale > 0f && parentScale > 0f)
		{
			localScale = boneScale / parentScale;
		}
		
		if(scaleTrans && localScale > 0f)
		{
			if(fSmooth != 0f)
				scaleTrans.localScale = Vector3.Lerp(scaleTrans.localScale, modelBoneScale * localScale, fSmooth * Time.deltaTime);
			else
				scaleTrans.localScale = modelBoneScale * localScale;

			return true;
		}

		return false;
	}


	public bool FixJointsBeforeScale()
	{
		Animator animatorComponent = GetComponent<Animator>();
		KinectManager manager = KinectManager.Instance;

		if(animatorComponent && modelBodyHeight > 0f && bodyHeight > 0f)
		{
			Transform hipCenter = animatorComponent.GetBoneTransform(HumanBodyBones.Hips);
			if((hipCenter.localScale - Vector3.one).magnitude > 0.01f)
				return false;

			Transform leftUpperLeg = animatorComponent.GetBoneTransform(HumanBodyBones.LeftUpperLeg);
			Transform rightUpperLeg = animatorComponent.GetBoneTransform(HumanBodyBones.RightUpperLeg);
			
			Transform leftUpperArm = animatorComponent.GetBoneTransform(HumanBodyBones.LeftUpperArm);
			Transform rightUpperArm = animatorComponent.GetBoneTransform(HumanBodyBones.RightUpperArm);
			
			if(leftUpperArm && rightUpperArm && leftUpperLeg && rightUpperLeg)
			{
				if(manager && manager.IsJointTracked(currentUserId, (int)KinectInterop.JointType.ShoulderLeft) &&
				   manager.IsJointTracked(currentUserId, (int)KinectInterop.JointType.ShoulderRight) &&
				   manager.IsJointTracked(currentUserId, (int)KinectInterop.JointType.HipLeft) &&
				   manager.IsJointTracked(currentUserId, (int)KinectInterop.JointType.HipRight))
				{
					Vector3 posHipCenter = manager.GetJointPosition(currentUserId, (int)KinectInterop.JointType.SpineBase);

					Vector3 posHipLeft = manager.GetJointPosition(currentUserId, (int)KinectInterop.JointType.HipLeft);
					Vector3 posHipRight = manager.GetJointPosition(currentUserId, (int)KinectInterop.JointType.HipRight);

					Vector3 posShoulderLeft = manager.GetJointPosition(currentUserId, (int)KinectInterop.JointType.ShoulderLeft);
					Vector3 posShoulderRight = manager.GetJointPosition(currentUserId, (int)KinectInterop.JointType.ShoulderRight);

					SetupUnscaledJoint(hipCenter, leftUpperLeg, posHipCenter, (!mirroredAvatar ? posHipLeft : posHipRight), modelBodyHeight, bodyHeight);
					SetupUnscaledJoint(hipCenter, rightUpperLeg, posHipCenter, (!mirroredAvatar ? posHipRight : posHipLeft), modelBodyHeight, bodyHeight);

					SetupUnscaledJoint(hipCenter, leftUpperArm, posHipCenter, (!mirroredAvatar ? posShoulderLeft : posShoulderRight), modelBodyHeight, bodyHeight);
					SetupUnscaledJoint(hipCenter, rightUpperArm, posHipCenter, (!mirroredAvatar ? posShoulderRight : posShoulderLeft), modelBodyHeight, bodyHeight);

					// recalculate model joints
					Start();

					return true;
				}
			}
		}
		
		return false;
	}

	private bool SetupUnscaledJoint(Transform hipCenter, Transform joint, Vector3 posHipCenter, Vector3 posJoint, float modelBoneLen, float userBoneLen)
	{
		float boneScale = 0f;

		if(modelBoneLen > 0f && userBoneLen > 0f)
		{
			boneScale = userBoneLen / modelBoneLen;
			//boneScale = 1f;
		}

		if(boneScale > 0f)
		{
			Vector3 posDiff = (posJoint - posHipCenter) / boneScale;
			posDiff.z = 0f;  // ignore difference in z

			Vector3 posJointNew = hipCenter.position + posDiff;
			joint.position = posJointNew;

			return true;
		}

		return false;
	}



}
