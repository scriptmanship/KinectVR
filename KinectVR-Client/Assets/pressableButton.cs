using UnityEngine;
using System.Collections;

public class pressableButton : MonoBehaviour {
	private Vector3 startPos;
	private Vector3 endPos;
	public pressableButton otherbut1;
	public pressableButton otherbut2;
	public bool pressed;
	AudioSource snd;

	public Transform envOn;
	public Transform envOff;

	public Transform spawnableFruits;
	// Use this for initialization
	void Start () {
		snd = GetComponent<AudioSource> ();
		startPos = transform.localPosition;
		endPos = startPos + new Vector3 (0, -0.1f, 0);
	}
	
	// Update is called once per frame
	void Update () {
		if (pressed == true) {
			transform.localPosition = Vector3.Lerp (transform.localPosition, endPos, Time.deltaTime * 5f);

		} else {
			transform.localPosition = Vector3.Lerp (transform.localPosition, startPos, Time.deltaTime * 5f);
		}
	}

	void OnTriggerEnter (){
		
		otherbut1.pressed = false;
		otherbut2.pressed = false;
		if (snd.isPlaying) {
			snd.Stop ();
		} else {
			snd.Play ();
		}
		if (envOn != null && envOff != null) {
			envOn.gameObject.SetActive (true);
			envOff.gameObject.SetActive (false);
		}


		if (spawnableFruits != null && pressed) {
			Instantiate (spawnableFruits, new Vector3(Random.Range(-5f,5f),30f+Random.Range(5f,20f),7f), Quaternion.identity);

		}
		pressed = true;
	}
}
