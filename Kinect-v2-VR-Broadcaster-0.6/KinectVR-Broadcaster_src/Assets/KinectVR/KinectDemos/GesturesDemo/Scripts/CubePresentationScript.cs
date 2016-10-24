using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CubePresentationScript : MonoBehaviour 
{
	[Tooltip("Whether the presentation slides can be changed with gestures (SwipeLeft & SwipeRight).")]
	public bool slideChangeWithGestures = true;
	[Tooltip("Whether the presentation slides can be changed with keys (PgDown & PgUp).")]
	public bool slideChangeWithKeys = true;
	[Tooltip("Speed of spinning, when presentation slides change.")]
	public int spinSpeed = 5;

	[Tooltip("List of the presentation slides.")]
	public List<Texture> slideTextures;
	[Tooltip("List of the side planes, comprising the presentation cube.")]
	public List<GameObject> cubeSides;
	

	//private int maxSides = 0;
	private int maxTextures = 0;
	private int side = 0;
	private int tex = 0;
	private bool isSpinning = false;

	private int[] hsides = { 0, 1, 2, 3 };  // left, front, right, back
	private int[] vsides = { 4, 1, 5, 3};  // up, front, down, back
	
	private CubeGestureListener gestureListener;
	private Quaternion initialRotation;
	private int stepsToGo = 0;
	//private float nextStepTime = 0f;
	private Vector3 rotationStep;


	void Start() 
	{
		// hide mouse cursor
		Cursor.visible = false;
		
		// calculate max slides and textures
		//maxSides = cubeSides.Count;
		maxTextures = slideTextures.Count;
		
		initialRotation = transform.rotation;
		isSpinning = false;
		
		tex = 0;
		side = hsides[1];
		
		if(side < cubeSides.Count && cubeSides[side] && cubeSides[side].GetComponent<Renderer>())
		{
			cubeSides[side].GetComponent<Renderer>().material.mainTexture = slideTextures[tex];
		}
		
		// get the gestures listener
		gestureListener = CubeGestureListener.Instance;
	}
	
	void Update() 
	{
		// dont run Update() if there is no gesture listener
		if(!gestureListener)
			return;
		
		if(!isSpinning)
		{
			if(slideChangeWithKeys)
			{
				if(Input.GetKeyDown(KeyCode.PageDown))
					RotateLeft();
				else if(Input.GetKeyDown(KeyCode.PageUp))
					RotateRight();
			}
			
			if(slideChangeWithGestures && gestureListener)
			{
				if(gestureListener.IsSwipeLeft())
					RotateLeft();
				else if(gestureListener.IsSwipeRight())
					RotateRight();
				else if(gestureListener.IsSwipeUp())
					RotateUp();
			}
		}
		else
		{
			// spin the presentation
			if(stepsToGo > 0)
			{
				//if(Time.realtimeSinceStartup >= nextStepTime)
				{
					transform.Rotate(rotationStep, Space.World);
					stepsToGo--;
					//nextStepTime = Time.realtimeSinceStartup + Time.deltaTime;
				}
			}
			else
			{
				transform.rotation = Quaternion.Euler(rotationStep * 90f / spinSpeed) * initialRotation;
				isSpinning = false;
			}
		}
	}
	
	// rotates cube left
	private void RotateLeft()
	{
		// set the next texture slide
		tex = (tex + 1) % maxTextures;
		
		// rotate hsides left
		SetSides(ref hsides, hsides[1], hsides[2], hsides[3], hsides[0]);
		SetSides(ref vsides, -1, hsides[1], -1, hsides[3]);
		side = hsides[1];

		// set the slide on the selected side
		if(side < cubeSides.Count && cubeSides[side] && cubeSides[side].GetComponent<Renderer>())
		{
			cubeSides[side].GetComponent<Renderer>().material.mainTexture = slideTextures[tex];
		}
		
		// rotate the presentation
		isSpinning = true;
		initialRotation = transform.rotation;
		rotationStep = new Vector3(0, 1 * spinSpeed, 0);
		stepsToGo = 90 / spinSpeed;
		//nextStepTime = 0f;
	}
	
	// rotates cube right
	private void RotateRight()
	{
		// set the previous texture slide
		if(tex <= 0)
			tex = maxTextures - 1;
		else
			tex -= 1;
		
		// rotate hsides right
		SetSides(ref hsides, hsides[3], hsides[0], hsides[1], hsides[2]);
		SetSides(ref vsides, -1, hsides[1], -1, hsides[3]);
		side = hsides[1];

		// set the slide on the selected side
		if(side < cubeSides.Count && cubeSides[side] && cubeSides[side].GetComponent<Renderer>())
		{
			cubeSides[side].GetComponent<Renderer>().material.mainTexture = slideTextures[tex];
		}
		
		// rotate the presentation
		isSpinning = true;
		initialRotation = transform.rotation;
		rotationStep = new Vector3(0, -1 * spinSpeed, 0);
		stepsToGo = 90 / spinSpeed;
		//nextStepTime = 0f;
	}

	// rotates cube up
	private void RotateUp()
	{
		// set the next texture slide
		tex = (tex + 1) % maxTextures;

		// rotate vsides up
		SetSides(ref vsides, vsides[1], vsides[2], vsides[3], vsides[0]);
		SetSides(ref hsides, -1, vsides[1], -1, vsides[3]);
		side = hsides[1];

		// set the slide on the selected side
		if(side < cubeSides.Count && cubeSides[side] && cubeSides[side].GetComponent<Renderer>())
		{
			cubeSides[side].GetComponent<Renderer>().material.mainTexture = slideTextures[tex];
		}

		// rotate the presentation
		isSpinning = true;
		initialRotation = transform.rotation;	
		rotationStep = new Vector3(1 * spinSpeed, 0, 0);
		stepsToGo = 90 / spinSpeed;
		//nextStepTime = 0f;
	}

	// sets values of sides' array
	private void SetSides(ref int[] sides, int v0, int v1, int v2, int v3)
	{
		if(v0 >= 0)
		{
			sides[0] = v0;
		}

		if(v1 >= 0)
		{
			sides[1] = v1;
		}

		if(v2 >= 0)
		{
			sides[2] = v2;
		}

		if(v3 >= 0)
		{
			sides[3] = v3;
		}
	}

}
