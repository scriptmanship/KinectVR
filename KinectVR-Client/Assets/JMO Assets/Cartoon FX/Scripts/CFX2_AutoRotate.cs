using UnityEngine;
using System.Collections;

// Cartoon FX  - (c) 2015 Jean Moreno

// Indefinitely rotates an object at a constant speed

public class CFX2_AutoRotate : MonoBehaviour
{
	public Vector3 speed = new Vector3(0,40f,0);
	
	void Update ()
	{
		transform.Rotate(speed * Time.deltaTime);
	}
}
