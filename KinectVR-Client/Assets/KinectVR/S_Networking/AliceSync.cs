using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class AliceSync : MonoBehaviour {
	//public BodySourceManager _BodyManager;

	private static AliceSync singleton;
	public static AliceSync s {get {return singleton;}}
	protected void Awake(){
		singleton = this;
	}	
	int moving;


	void Start () {

	}

	void Update () {



	}

	public void CreateBody(ulong id){
		if (AliceManager.s.net != null)
		AliceManager.s.net.Send ("*cbody," + id + "*");

	}

	public void RemoveBody(ulong id){
		if (AliceManager.s.net != null)
			AliceManager.s.net.Send ("*dbody," + id + "*");
		
	}


	public void UpdateBodyPart(ulong id, string jointName, Vector3 pos, Quaternion rot, int infered){
		if (AliceManager.s.net != null) {
			AliceManager.s.net.Send ("*ubody," + id + "," + jointName + "," + pos.x + "," + pos.y + "," + pos.z + "," + rot.x + "," + rot.y + "," + rot.z + "," + rot.w + "," + infered + "*");
		}
	}

	string EncodeData(){
		string data;

		data = "2,"+transform.position.x+","+transform.position.y+","+transform.position.z+","+transform.eulerAngles.y+","+moving;


		return data;
	}


	public IEnumerator sync (float time){
		yield return new WaitForSeconds (time);
		//if (AliceManager.s.net != null)
		//AliceManager.s.net.Send(EncodeData());
		StartCoroutine(sync (time));
	}

	public void start (IEnumerator cor){
		StartCoroutine(cor);
	}
	public void stop (IEnumerator cor){
		StopCoroutine(cor.ToString());
	}

}
