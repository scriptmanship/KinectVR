using UnityEngine;
using System.Collections;

public class Rotate : MonoBehaviour {
	public float speed = 1f;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		transform.Rotate (new Vector3 (0f, 1f, 0f), speed * Time.deltaTime);
	
	}
}
