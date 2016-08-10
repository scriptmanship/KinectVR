using UnityEngine;
using System.Collections;

public class RandomMover : MonoBehaviour {

	float ran;
	Material mat;
	public float ytile = 20f;

	public float startrange;
	public float stoprange;

	private float restartspeed = 1f;



	private float rangerestart;
	private Vector3 startpos;
	private Vector3 bounds;




	// Use this for initialization
	void Start () {
		startpos = transform.position;
		bounds = gameObject.GetComponent<Collider> ().bounds.extents;
		Debug.Log (bounds);
		rangerestart = bounds.z / (ytile / 2f) / restartspeed ;
		Debug.Log (rangerestart);
		Debug.Log(startpos.z);
		Debug.Log(startpos.z + rangerestart);
		ran = Random.Range (startrange, stoprange);
		// fast test
		//ran = 4f;

		mat = gameObject.GetComponent<Renderer>().material;

	}

	private int dir;
	Color PingPongAlpha(Material mat){
		Color col = mat.color;
		if (dir == 0 && col.a < 0.7f) {
			col.a += 0.003f * ran;	
		} else {
			dir = 1;
		}
		if (dir == 1 && col.a > 0.2f) {
			col.a -= 0.003f * ran;
		} else {
			dir = 0;
		}
		return col;
	}

	// Update is called once per frame
	void Update () {


		mat.color = PingPongAlpha(mat);
		

		if (transform.position.z < startpos.z + rangerestart) {
			Vector3 pos = transform.position;
			pos.z += ran * Time.deltaTime;
			transform.position = pos;

		} else {
			transform.position = startpos;

		}
	
	}
}
