using UnityEngine;
using System.Collections;

public class OnlineBodyHeight : MonoBehaviour {
	public Transform spineBase;
	public string bodyName;
	public Rigidbody rb;

	// Use this for initialization
	void Start () {

		rb = gameObject.AddComponent<Rigidbody> ();
		rb.constraints = RigidbodyConstraints.FreezeRotation;
		gameObject.AddComponent<SphereCollider> ();
		gameObject.layer = LayerMask.NameToLayer ("PlayerHeight");


		transform.position = KinectWorld.s.transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 newpos = transform.position;
		Vector3 spinePos = spineBase.position;

		newpos.x = spinePos.x;
		newpos.z = spinePos.z;

		transform.position = newpos;


	
	}
}
