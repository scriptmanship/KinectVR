using UnityEngine;
using System.Collections;

using System.Collections.Generic;

public class OnlineBodyView : MonoBehaviour {
	private static OnlineBodyView singleton;
	public static OnlineBodyView s {get {return singleton;}}
	protected void Awake(){
		singleton = this;
	}	

	public Material sphere;

	public OnlineBody[] bodies;

	public Transform avatarPrefab;

	private float handClosedDist = 0.19f;



	// Use this for initialization
	void Start () {

		bodies = new OnlineBody[6];

	}
	
	// Update is called once per frame
	void Update () {
		MapAvatarRotation ();
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
				//bodies[i].avatar = Instantiate (ObjectManager.s.avatarPrefab, bodies[i].go.transform.position,  ObjectManager.s.avatarPrefab.rotation) as Transform;
				//bodies[i].avatar.parent = bodies[i].go.transform;
				//bodies[i].avatar.localPosition = new Vector3(0,0,0);
				//bodies[i].anim = bodies[i].avatar.gameObject.GetComponent<Animator>();
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
				Destroy (bodies [i].avatarScript.avatar.gameObject);
				Destroy(bodies[i].go);
				//Destroy (bodies[i].avatar.gameObject);
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

							//bodies [i].parts [r].go.transform.localPosition = pos;
							float interpAmount = 50f;
							bodies [i].parts [r].go.transform.localPosition = Vector3.Lerp(bodies [i].parts [r].go.transform.localPosition,pos,Time.deltaTime*interpAmount);
								//bodies [i].parts [r].go.transform.localRotation = rot;

								
							bodies [i].parts [r].infered = int.Parse(part[8]);


							if (part[8] == "0"){
								bodies [i].parts [r].r.material.color = ObjectManager.s.coolMat.color;
								}else{
								bodies [i].parts [r].r.material.color = new Color(1f,0f,0f);
								}

							}


						}
						
					}

					break;

				}
			}


	}

	public void MapAvatarRotation (){
		foreach (OnlineBody body in bodies){
			if (body != null){
				//Quaternion newrot = Quaternion.FromToRotation(body.partsDic["WristRight"].go.transform.position,body.partsDic["HandRight"].go.transform.position);



				// Line Renderer Positions
				/*
				body.partsDic["SpineShoulder"].lr.SetPosition(0, body.partsDic["SpineShoulder"].go.transform.position);
				body.partsDic["SpineShoulder"].lr.SetPosition(1, body.partsDic["Neck"].go.transform.position);

				body.partsDic["Neck"].lr.SetPosition(0, body.partsDic["Neck"].go.transform.position);
				body.partsDic["Neck"].lr.SetPosition(1, body.partsDic["Head"].go.transform.position);

				body.partsDic["WristRight"].lr.SetPosition(0, body.partsDic["WristRight"].go.transform.position);
				body.partsDic["WristRight"].lr.SetPosition(1, body.partsDic["ElbowRight"].go.transform.position);

				body.partsDic["ElbowRight"].lr.SetPosition(0, body.partsDic["ElbowRight"].go.transform.position);
				body.partsDic["ElbowRight"].lr.SetPosition(1, body.partsDic["ShoulderRight"].go.transform.position);

				body.partsDic["ShoulderRight"].lr.SetPosition(0, body.partsDic["ShoulderRight"].go.transform.position);
				body.partsDic["ShoulderRight"].lr.SetPosition(1, body.partsDic["SpineShoulder"].go.transform.position);

				body.partsDic["WristLeft"].lr.SetPosition(0, body.partsDic["WristLeft"].go.transform.position);
				body.partsDic["WristLeft"].lr.SetPosition(1, body.partsDic["ElbowLeft"].go.transform.position);
				
				body.partsDic["ElbowLeft"].lr.SetPosition(0, body.partsDic["ElbowLeft"].go.transform.position);
				body.partsDic["ElbowLeft"].lr.SetPosition(1, body.partsDic["ShoulderLeft"].go.transform.position);
				
				body.partsDic["ShoulderLeft"].lr.SetPosition(0, body.partsDic["ShoulderLeft"].go.transform.position);
				body.partsDic["ShoulderLeft"].lr.SetPosition(1, body.partsDic["SpineShoulder"].go.transform.position);

				body.partsDic["SpineShoulder"].lr.SetPosition(0, body.partsDic["SpineShoulder"].go.transform.position);
				body.partsDic["SpineShoulder"].lr.SetPosition(1, body.partsDic["SpineMid"].go.transform.position);

				body.partsDic["SpineMid"].lr.SetPosition(0, body.partsDic["SpineMid"].go.transform.position);
				body.partsDic["SpineMid"].lr.SetPosition(1, body.partsDic["SpineBase"].go.transform.position);

				body.partsDic["HipRight"].lr.SetPosition(0, body.partsDic["HipRight"].go.transform.position);
				body.partsDic["HipRight"].lr.SetPosition(1, body.partsDic["SpineBase"].go.transform.position);

				body.partsDic["KneeRight"].lr.SetPosition(0, body.partsDic["KneeRight"].go.transform.position);
				body.partsDic["KneeRight"].lr.SetPosition(1, body.partsDic["HipRight"].go.transform.position);

				body.partsDic["AnkleRight"].lr.SetPosition(0, body.partsDic["AnkleRight"].go.transform.position);
				body.partsDic["AnkleRight"].lr.SetPosition(1, body.partsDic["KneeRight"].go.transform.position);

				body.partsDic["FootRight"].lr.SetPosition(0, body.partsDic["FootRight"].go.transform.position);
				body.partsDic["FootRight"].lr.SetPosition(1, body.partsDic["AnkleRight"].go.transform.position);

				body.partsDic["HipLeft"].lr.SetPosition(0, body.partsDic["HipLeft"].go.transform.position);
				body.partsDic["HipLeft"].lr.SetPosition(1, body.partsDic["SpineBase"].go.transform.position);
				
				body.partsDic["KneeLeft"].lr.SetPosition(0, body.partsDic["KneeLeft"].go.transform.position);
				body.partsDic["KneeLeft"].lr.SetPosition(1, body.partsDic["HipLeft"].go.transform.position);
				
				body.partsDic["AnkleLeft"].lr.SetPosition(0, body.partsDic["AnkleLeft"].go.transform.position);
				body.partsDic["AnkleLeft"].lr.SetPosition(1, body.partsDic["KneeLeft"].go.transform.position);
				
				body.partsDic["FootLeft"].lr.SetPosition(0, body.partsDic["FootLeft"].go.transform.position);
				body.partsDic["FootLeft"].lr.SetPosition(1, body.partsDic["AnkleLeft"].go.transform.position);
				*/

				// Joint Rotations
				//body.partsDic["WristRight"].go.transform.LookAt(body.partsDic["HandTipRight"].go.transform.position);
				//body.partsDic["WristLeft"].go.transform.LookAt(body.partsDic["HandTipLeft"].go.transform.position);
				//Debug.Log (newrot);



				if (Vector3.Distance(body.partsDic["HandTipRight"].go.transform.position, body.partsDic["HandRight"].go.transform.position) < handClosedDist){
					//body.handRight.Close();
				}else{
					//body.handRight.Open();
				}

				if (Vector3.Distance(body.partsDic["HandTipLeft"].go.transform.position, body.partsDic["HandLeft"].go.transform.position) < handClosedDist){
					//body.handLeft.Close();
				}else{
					//body.handLeft.Open();
				}

			}
		}

	}



}


public class OnlineBody
{
	public string name;
	public GameObject go;
	public Transform avatar;
	public BodyPart[] parts;
	public Animator anim;


	public Dictionary<string, BodyPart> partsDic;
	
	public HandModel handLeft;
	public HandModel handRight;
	public AvatarOnlineBody avatarScript;

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

		partsDic = new Dictionary<string, BodyPart> ();


		//height.transform.parent = go.transform;
	
		GameObject height = new GameObject ("Height:" + name);
		OnlineBodyHeight h = height.AddComponent<OnlineBodyHeight> ();
		for (int i = 0; i<bodypartarray.Length; i++) {
			parts[i] = new BodyPart(bodypartarray[i],go,this);
			partsDic.Add (parts[i].name,parts[i]);

			if (parts[i].name == "SpineBase"){
				h.bodyName = name;
				h.transform.position = parts[i].go.transform.position;
				h.spineBase = parts[i].go.transform;

			}
		}
		OnlineBodyPhysics phys = go.AddComponent<OnlineBodyPhysics> ();
		 avatarScript = go.AddComponent<AvatarOnlineBody> ();
		avatarScript.body = this;
		phys.onlineBody = this;
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
	public LineRenderer lr;
	public OnlineBody parentOnlineBody;

	public BodyPart(string aName, GameObject parentBody, OnlineBody body){
		parentOnlineBody = body;
		name = aName;
		go = GameObject.CreatePrimitive (PrimitiveType.Sphere);
		//go = new GameObject (name);
		//go.GetComponent<Renderer> ().enabled = false;
		go.name = name;
		go.transform.parent = parentBody.transform;
		go.transform.localScale = new Vector3 (0.2f, 0.2f, 0.2f);
		go.layer = LayerMask.NameToLayer ("PlayerPoints");
		Rigidbody rb = go.AddComponent<Rigidbody> ();
		rb.useGravity = false;
		rb.isKinematic = true;
		go.AddComponent<SphereCollider> ();
		r = go.GetComponent<Renderer> ();
		r.material = ObjectManager.s.coolMat;
		r.enabled = false;
		//lr = go.AddComponent<LineRenderer> ();
		//lr.SetVertexCount (2);
		//lr.SetWidth (0.1f, 0.1f);
		//lr.SetColors (Color.black, Color.black);
		//lr.material = ObjectManager.s.lineMat;

		if (aName == "HandTipLeft" || aName == "HandTipRight" || aName == "HandLeft" || aName == "HandRight" || aName == "ThumbRight" || aName == "ThumbLeft") {
			r.enabled = false;
		} else if (aName == "WristLeft" || aName == "WristRight" ) {
			r.enabled = false;

			/*
			HandModel hand = go.AddComponent<HandModel>();

			if (aName == "WristLeft")
				parentOnlineBody.handLeft = hand;
			if (aName == "WristRight")
				parentOnlineBody.handRight = hand;
			*/

		} else {

		}

	}



}


