using UnityEngine;
using System.Collections;

// Require these components when using this script
[RequireComponent(typeof (Animator))]
[RequireComponent(typeof (CapsuleCollider))]
[RequireComponent(typeof (Rigidbody))]
public class BotControlScript : MonoBehaviour
{
//	[System.NonSerialized]					
//	public float lookWeight;					// the amount to transition when using head look
//	
//	[System.NonSerialized]
//	public Transform enemy;						// a transform to Lerp the camera to during head look

	[Tooltip("Overall animation speed.")]
	public float animSpeed = 1.5f;				// a public setting for overall animator animation speed

	//public float lookSmoother = 3f;				// a smoothing setting for camera motion

	[Tooltip("Whether to use the extra curves for animation or not.")]
	public bool useCurves;						// a setting for teaching purposes to show use of curves


	private Animator anim;							// a reference to the animator on the character
	private AnimatorStateInfo currentBaseState;			// a reference to the current state of the animator, used for base layer
	private AnimatorStateInfo layer2CurrentState;	// a reference to the current state of the animator, used for layer 2
	private CapsuleCollider col;					// a reference to the capsule collider of the character
	
	private SpeechManager speechManager;
	private float walkSpeed;
	private float walkDirection;
	private bool jumpNow;
	private bool waveNow;

	static int idleState = Animator.StringToHash("Base Layer.Idle");	
	static int locoState = Animator.StringToHash("Base Layer.Locomotion");			// these integers are references to our animator's states
	static int jumpState = Animator.StringToHash("Base Layer.Jump");				// and are used to check state for various actions to occur
	static int waveState = Animator.StringToHash("Layer2.Wave");
	

	void Start ()
	{
		// initialising reference variables
		anim = GetComponent<Animator>();					  
		col = GetComponent<CapsuleCollider>();

		if(anim.layerCount == 2)
		{
			anim.SetLayerWeight(1, 1);
		}
	}
	
	
	void FixedUpdate ()
	{
		// get the speech manager instance
		if(speechManager == null)
		{
			speechManager = SpeechManager.Instance;
		}

		if(speechManager != null && speechManager.IsSapiInitialized())
		{
			if(speechManager.IsPhraseRecognized())
			{
				string sPhraseTag = speechManager.GetPhraseTagRecognized();
				
				switch(sPhraseTag)
				{
					case "FORWARD":
						walkSpeed = 0.2f;
						walkDirection = 0f;
						break;
					
					case "BACK":
						walkSpeed = -0.2f;
						walkDirection = 0f;
						break;
	
					case "LEFT":
						walkDirection = -0.2f;
						if(walkSpeed == 0f)
							walkSpeed = 0.2f;
						break;
	
					case "RIGHT":
						walkDirection = 0.2f;
						if(walkSpeed == 0f)
							walkSpeed = 0.2f;
						break;
	
					case "RUN":
						walkSpeed = 0.5f;
						walkDirection = 0f;
						break;
	
					case "STOP":
						walkSpeed = 0f;
						walkDirection = 0f;
						break;

					case "JUMP":
						jumpNow = true;
						walkSpeed = 0.5f;
						walkDirection = 0f;
						break;
	
					case "HELLO":
						waveNow = true;
						walkSpeed = 0.0f;
						walkDirection = 0f;
						break;
	
				}

				speechManager.ClearPhraseRecognized();
			}
			
		}
		else
		{
			walkDirection = Input.GetAxis("Horizontal");				// setup h variable as our horizontal input axis
			walkSpeed = Input.GetAxis("Vertical");				// setup v variables as our vertical input axis
			jumpNow = Input.GetButtonDown("Jump");
			waveNow = Input.GetButtonDown("Jump");
		}
		
		anim.SetFloat("Speed", walkSpeed);					// set our animator's float parameter 'Speed' equal to the vertical input axis				
		anim.SetFloat("Direction", walkDirection); 			// set our animator's float parameter 'Direction' equal to the horizontal input axis		
		anim.speed = animSpeed;								// set the speed of our animator to the public variable 'animSpeed'
		//anim.SetLookAtWeight(lookWeight);					// set the Look At Weight - amount to use look at IK vs using the head's animation
		currentBaseState = anim.GetCurrentAnimatorStateInfo(0);	// set our currentState variable to the current state of the Base Layer (0) of animation
		
		if(anim.layerCount == 2)
		{
			layer2CurrentState = anim.GetCurrentAnimatorStateInfo(1);	// set our layer2CurrentState variable to the current state of the second Layer (1) of animation
		}

		// if we are currently in a state called Locomotion (see line 25), then allow Jump input (Space) to set the Jump bool parameter in the Animator to true
		if (currentBaseState.fullPathHash == locoState)
		{
			if(jumpNow)
			{
				jumpNow = false;
				anim.SetBool("Jump", true);
			}
		}
		
		// if we are in the jumping state... 
		else if(currentBaseState.fullPathHash == jumpState)
		{
			//  ..and not still in transition..
			if(!anim.IsInTransition(0))
			{
				if(useCurves)
					// ..set the collider height to a float curve in the clip called ColliderHeight
					col.height = anim.GetFloat("ColliderHeight");
				
				// reset the Jump bool so we can jump again, and so that the state does not loop 
				anim.SetBool("Jump", false);
			}
			
			// Raycast down from the center of the character.. 
			Ray ray = new Ray(transform.position + Vector3.up, -Vector3.up);
			RaycastHit hitInfo = new RaycastHit();
			
			if (Physics.Raycast(ray, out hitInfo))
			{
				// ..if distance to the ground is more than 1.75, use Match Target
				if (hitInfo.distance > 1.75f)
				{
					
					// MatchTarget allows us to take over animation and smoothly transition our character towards a location - the hit point from the ray.
					// Here we're telling the Root of the character to only be influenced on the Y axis (MatchTargetWeightMask) and only occur between 0.35 and 0.5
					// of the timeline of our animation clip
					anim.MatchTarget(hitInfo.point, Quaternion.identity, AvatarTarget.Root, new MatchTargetWeightMask(new Vector3(0, 1, 0), 0), 0.35f, 0.5f);
				}
			}
		}
		
		// check if we are at idle, if so, let us Wave!
		else if (currentBaseState.fullPathHash == idleState)
		{
			if(waveNow)
			{
				waveNow = false;
				anim.SetBool("Wave", true);
			}
		}
		
		// if we enter the waving state, reset the bool to let us wave again in future
		if(layer2CurrentState.fullPathHash == waveState)
		{
			anim.SetBool("Wave", false);
		}
	}

}
