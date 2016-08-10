using UnityEngine;
using System.Collections;

public class FollowTransform : MonoBehaviour {

	public Transform target;
	public Vector3 offset;
	public bool interpolation = false;
	public bool disableYFollow = false;

	public float interp = 3f;
	// Use this for initialization
	void Start () {

		Vector3 targetPos = target.position;
		Vector3 myPos = transform.position;


		offset = myPos - targetPos;
	
	}
	
	// Update is called once per frame
	void Update () {

	
		if (target) {

			Vector3 newpos = target.position;
			newpos += target.forward * offset.z;
			newpos += target.up * offset.y;
			newpos += target.right * offset.x;

			if (disableYFollow)
				newpos.y = transform.position.y;


			if (interpolation)
				transform.position = Vector3.Lerp (transform.position, newpos, Time.deltaTime * interp);
			else
				transform.position = newpos;
		}
	}
}
