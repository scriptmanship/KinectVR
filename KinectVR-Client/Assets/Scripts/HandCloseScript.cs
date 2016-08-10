using UnityEngine;
using System.Collections;

public class HandCloseScript : MonoBehaviour {
	public Transform thumb;
	public Transform thumb1;
	public Transform index;
	public Transform index1;
	public Transform middle;
	public Transform middle1;
	public Transform pinky;
	public Transform pinky1;
	public Transform ring;
	public Transform ring1;

	private Vector3 index_start;
	private Vector3 index_end;

	private Vector3 index1_start;
	private Vector3 index1_end;

	private Vector3 middle_start;
	private Vector3 middle_end;

	private Vector3 middle1_start;
	private Vector3 middle1_end;

	private Vector3 pinky_start;
	private Vector3 pinky_end;

	private Vector3 pinky1_start;
	private Vector3 pinky1_end;

	private Vector3 ring_start;
	private Vector3 ring_end;

	private Vector3 ring1_start;
	private Vector3 ring1_end;

	public bool isLeft;

	private Vector3 thumb_start;
	private Vector3 thumb_end;
	private Vector3 thumb1_start;
	private Vector3 thumb1_end;

	public bool thumbBent;
	public bool fingersBent;
	// Use this for initialization
	void Start () {
		thumb_start = thumb.localEulerAngles;
		thumb1_start = thumb1.localEulerAngles;

		index_start = index.localEulerAngles;
		index1_start = index1.localEulerAngles;

		middle_start = middle.localEulerAngles;
		middle1_start = middle1.localEulerAngles;

		pinky_start  = pinky.localEulerAngles;
		pinky1_start = pinky1.localEulerAngles;

		ring_start = ring.localEulerAngles;
		ring1_start = ring1.localEulerAngles;


		index_end = new Vector3 (80f, 9f, 0);
		index1_end = new Vector3 (80f, 9f, 0);

		middle_end = new Vector3 (80f, 9f, 0);
		middle1_end = new Vector3 (80f, 9f, 0);

		pinky_end = new Vector3 (80f, 9f, 0);
		pinky1_end = new Vector3 (80f, 9f, 0);

		ring_end = new Vector3 (80f, 9f, 0);
		ring1_end = new Vector3 (80f, 9f, 0);

		//thumb_end = new Vector3 (39.796f,328.51f, 336.81f);
		if (isLeft) {
			thumb_end = thumb_start - new Vector3 (0f, 0f, 330f);
			thumb1_end = new Vector3 (0, 0, 35f);
		} else {
			thumb_end = thumb_start + new Vector3 (0f, 0f, 330f);
			thumb1_end = new Vector3 (0, 0, 330f);
		}

	}

	private float speed = 30f;
	// Update is called once per frame
	void FixedUpdate () {
		if (thumbBent) {
			thumb.localEulerAngles = Vector3.Lerp(thumb.localEulerAngles,thumb_end, Time.deltaTime*speed);
			thumb1.localEulerAngles = Vector3.Lerp(thumb1.localEulerAngles,thumb1_end, Time.deltaTime*speed);
		} else {
			thumb.localEulerAngles = Vector3.Lerp(thumb.localEulerAngles,thumb_start, Time.deltaTime*speed);
			thumb1.localEulerAngles = Vector3.Lerp(thumb1.localEulerAngles,thumb1_start, Time.deltaTime*speed);

		}

		if (fingersBent) {
			index.localEulerAngles = Vector3.Lerp(index.localEulerAngles,index_end, Time.deltaTime*speed);
			index1.localEulerAngles = Vector3.Lerp(index1.localEulerAngles,index1_end, Time.deltaTime*speed);

			middle.localEulerAngles = Vector3.Lerp(middle.localEulerAngles,middle_end, Time.deltaTime*speed);
			middle1.localEulerAngles = Vector3.Lerp(middle1.localEulerAngles,middle1_end, Time.deltaTime*speed);

			pinky.localEulerAngles = Vector3.Lerp(pinky.localEulerAngles,pinky_end, Time.deltaTime*speed);
			pinky1.localEulerAngles = Vector3.Lerp(pinky1.localEulerAngles,pinky1_end, Time.deltaTime*speed);

			ring.localEulerAngles = Vector3.Lerp(ring.localEulerAngles,ring_end, Time.deltaTime*speed);
			ring1.localEulerAngles = Vector3.Lerp(ring1.localEulerAngles,ring1_end, Time.deltaTime*speed);
		} else {
			index.localEulerAngles = Vector3.Lerp(index.localEulerAngles,index_start, Time.deltaTime*speed);
			index1.localEulerAngles = Vector3.Lerp(index1.localEulerAngles,index1_start, Time.deltaTime*speed);

			middle.localEulerAngles = Vector3.Lerp(middle.localEulerAngles,middle_start, Time.deltaTime*speed);
			middle1.localEulerAngles = Vector3.Lerp(middle1.localEulerAngles,middle1_start, Time.deltaTime*speed);

			pinky.localEulerAngles = Vector3.Lerp(pinky.localEulerAngles,pinky_start, Time.deltaTime*speed);
			pinky1.localEulerAngles = Vector3.Lerp(pinky1.localEulerAngles,pinky1_start, Time.deltaTime*speed);

			ring.localEulerAngles = Vector3.Lerp(ring.localEulerAngles,ring_start, Time.deltaTime*speed);
			ring1.localEulerAngles = Vector3.Lerp(ring1.localEulerAngles,ring1_start, Time.deltaTime*speed);
		}
	
	}
}
