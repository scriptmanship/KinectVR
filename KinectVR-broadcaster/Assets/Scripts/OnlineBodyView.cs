using UnityEngine;
using System.Collections;

public class OnlineBodyView : MonoBehaviour {
	private static OnlineBodyView singleton;
	public static OnlineBodyView s {get {return singleton;}}
	protected void Awake(){
		singleton = this;
	}	

	public Material sphere;

	public OnlineBody[] bodies;

	public Transform avatarPrefab;


	// Use this for initialization
	void Start () {

		bodies = new OnlineBody[6];

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void UpdateBodyList (string data){
		string[] r = data.Split (","[0]);

		for (int i = 0; i<bodies.Length; i++) {
			if (bodies[i] != null){
				bool check = false;
				for (int x = 0; x<r.Length; x++) {

					if (bodies[i].name == r[x]) {
						
						check = true;
					}
				}
				if (!check){

					DeleteBody(bodies[i].name);
				}
			}
		}
		//Debug.Log (data);
	}

	public void CreateBody (string name){
		for (int i = 0; i<bodies.Length; i++) {
			if (bodies[i] == null){
			bodies[i] = new OnlineBody (name);

				//bodies[i].character = Instantiate(avatarPrefab, new Vector3(0f, 0f, 0f), avatarPrefab.rotation) as Transform;
				//MapToKinect map = bodies[i].character.GetComponent<MapToKinect>();
				//map.AssignBody (name);

				break;
			}

		}
	}

	public void DeleteBody (string name){

		for (int i = 0; i<bodies.Length; i++) {
			if (bodies[i].name == name){
				//Destroy(bodies[i].character.gameObject);
				OnlineBodyPhysics physScript = bodies[i].go.GetComponent<OnlineBodyPhysics>();
				Destroy(physScript.heightSphere.gameObject);
				Destroy(bodies[i].go);
				bodies[i] = null;
				if (PlayerManager.s.currentPlayer != null && name == PlayerManager.s.currentPlayer.name){
						PlayerManager.s.currentPlayer = null;
					}
				break;
			}
		}
	}

	public void SyncState (string[] data){
		Debug.Log ("Sync State");
		for (int i = 1; i<data.Length-1; i++) {
			bool check = false;
			for (int j = 0; j<bodies.Length; j++){
				if (bodies[j] != null)
				if (data[i] == bodies[j].name){
					check = true;
					break;
				}
			}
			Debug.Log (check);
			if (!check){
				CreateBody(data[i]);
			}
		}

		for (int r = 0; r<bodies.Length; r++) {
			if (bodies[r] != null){
				bool check = false;
				for (int k = 0; k<data.Length; k++){
						if(bodies[r].name == data[k])
							check = true;
				}
				if (check == false){
					DeleteBody (bodies[r].name);
				}
			}
		}

	}



	public void RefreshBody (string[] data){


			//name = data [1];
			var bodyname = data [1];
			for (int i = 0; i<bodies.Length; i++) {
				if (bodies [i] != null)
				if (bodies [i].name == bodyname) {
					for (int j = 2; j<data.Length-1; j++) {
						string[] part = data [j].Split ("," [0]);
						for (int r = 0; r<bodies[i].parts.Length; r++) {
							if (bodies [i].parts [r].name == part [0]) {
								Vector3 pos = new Vector3 (0f, 0f, 0f);
								Quaternion rot = new Quaternion (0f, 0f, 0f, 0f);

								float.TryParse (part [1], out pos.x);
								float.TryParse (part [2], out pos.y);
								float.TryParse (part [3], out pos.z);

								float.TryParse (part [4], out rot.x);
								float.TryParse (part [5], out rot.y);
								float.TryParse (part [6], out rot.z);
								float.TryParse (part [7], out rot.w);

								bodies [i].parts [r].go.transform.localPosition = pos;
								bodies [i].parts [r].go.transform.rotation = rot;

								
							bodies [i].parts [r].infered = int.Parse(part[8]);
								

							if (part[8] == "0"){
								bodies [i].parts [r].r.material.color = new Color(1f,1f,1f);
								}else{
								bodies [i].parts [r].r.material.color = new Color(1f,0f,0f);
								}

							}

							//LineRenderer lr
						}
						
					}

					break;

				}
			}


	}
}


public class OnlineBody
{
	public string name;
	public GameObject go;
	public Transform character;
	public BodyPart[] parts;
	public OnlineBody(string aName)
	{
		parts = new BodyPart[25];
		string bodypartlist = "FootLeft,AnkleLeft,KneeLeft,HipLeft,FootRight,AnkleRight,KneeRight,HipRight,HandTipLeft,ThumbLeft,HandLeft,WristLeft,ElbowLeft,ShoulderLeft,HandTipRight,ThumbRight,HandRight,WristRight,ElbowRight,ShoulderRight,SpineBase,SpineMid,SpineShoulder,Neck,Head";
		string[] bodypartarray;
		bodypartarray = bodypartlist.Split ("," [0]);
		name = aName;
		go = new GameObject ("OnlineBody:" + name);
		go.transform.position = KinectWorld.s.transform.position;
		go.transform.parent = OnlineBodyView.s.transform;






		//height.transform.parent = go.transform;
	
		GameObject height = new GameObject ("Height:" + name);
		OnlineBodyHeight h = height.AddComponent<OnlineBodyHeight> ();
		for (int i = 0; i<bodypartarray.Length; i++) {
			parts[i] = new BodyPart(bodypartarray[i],go);

			if (parts[i].name == "SpineBase"){
				h.bodyName = name;
				h.transform.position = parts[i].go.transform.position;
				h.spineBase = parts[i].go.transform;

			}
		}
		OnlineBodyPhysics phys = go.AddComponent<OnlineBodyPhysics> ();
		phys.heightSphere = h;





		//OnlineBodyView.s.maptokinect.AssignBody (name);
	}
}

public class BodyPart
{
	public string name;
	public GameObject go;
	public Transform transform;
	public int infered;
	public Renderer r;

	public BodyPart(string aName, GameObject parentBody){
		name = aName;
		go = GameObject.CreatePrimitive (PrimitiveType.Sphere);
		//go.GetComponent<Renderer> ().enabled = false;
		go.name = name;
		go.transform.parent = parentBody.transform;
		go.transform.localScale =  new Vector3 (0.3f, 0.3f, 0.3f);
		go.layer = LayerMask.NameToLayer ("PlayerPoints");
		Rigidbody rb = go.AddComponent<Rigidbody> ();
		rb.useGravity = false;
		rb.isKinematic = true;
		go.AddComponent<SphereCollider> ();
		r = go.GetComponent<Renderer> ();
		r.material = ObjectManager.s.coolMat;

	}



}


