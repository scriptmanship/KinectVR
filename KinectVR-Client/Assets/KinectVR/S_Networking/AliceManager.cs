using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AliceManager : MonoBehaviour {
	private static AliceManager singleton;
	public static AliceManager s {get {return singleton;}}
	protected void Awake(){
		singleton = this;
	}	
	public Alice net;
	public string host = "104.131.11.131";
	public string port = "5000";
	public string name;

	// count number of recieved messages in time period
	private int packetstemp;
	private int packets;

	public bool isServer;

	List<string> recievelist = new List<string>();
	
	void OnApplicationQuit(){
		StopServer ();
	}

	void Start () {

		//ConnectServer ();
		name = RandomString (8);

		isServer = false;
		if (!isServer)
			ConnectServer ();
		else
			StartServer ();


	}
	void Update () {
		ParseList ();
	}

	/*
	IEnumerator PPS () {
		packetstemp = packets;
		yield return new WaitForSeconds(1f);
		float result = 1000f/((packets - packetstemp)*1f/PlayerManager.s.numberonline);
		result = Mathf.Round(result);
		DebugManager.s.overlay.text = result.ToString() + " ms";
		StartCoroutine (PPS ());
	}
	*/

	string wholepacket;

	public void Recieve(string a){

		if (a [a.Length - 1] == "*" [0]) {
			if (wholepacket == null) {
				recievelist.Add (a);
			} else {
				wholepacket += a;
				recievelist.Add (wholepacket);
				wholepacket = null;
			}
		}
		else {
			wholepacket += a;
		}
	}


	public void ParseList(){
		string[] a = recievelist.ToArray();
		recievelist.Clear();
		for (int i = 0; i<a.Length; i++){
			Parser (a[i]);
		}

	}




	void Parser(string a){
		string[] e = a.Split("*"[0]);
		//Debug.Log (a);
		if (e.Length > 1) {

			string[] r = e[1].Split ("|"[0]);
			//Debug.Log (r[0]);

			if (r[0] == "1"){
				Debug.Log ("Created Body " + r[1]);
				OnlineBodyView.s.CreateBody(r[1]);
			}

			if (r[0] == "0"){
				Debug.Log ("Deleted Body " + r[1]);
				OnlineBodyView.s.DeleteBody(r[1]);
			}


			if (r[0] == "2"){
				OnlineBodyView.s.RefreshBody(r);
			}


			if (r[0] == "5"){
				OnlineBodyView.s.SyncState(r);
			}

			if (r[0] == "6"){
				OnlineBodyView.s.UpdateBodyList(r[1]);
			}
		}
		/*
		for (int i = 0; i<e.Length; i++){
			if (e[i] != ""){
				packets++;
				string[] s = e[i].Split(","[0]);
				Handler(s);
			}
		}
		*/
	}

	void Handler(string[] s){
		if (s[0] == "0"){

		}
		if (s[0] == "1"){

		}
		if (s[0] == "2"){


		}

	}

	void StartServer() {

		//BodySourceManager.s.StartTracking ();
			net = new Alice ();
			if (net.Connect (host, int.Parse(port))) {
			isServer = true;
			Debug.Log (host + ":" + port);
			Login (name);
			AliceSync.s.start(AliceSync.s.sync(0.016f));

			}else{
			net = null;
			}
		}
	void ConnectServer() {
		//BodySourceManager.s.StartTracking ();

		net = new Alice ();
		if (net.Connect (host, int.Parse(port))) {
			Debug.Log (host + ":" + port);
			net.AssignReceive(Recieve);
			Login (name);
			
		}else{
			net = null;
		}
	}

	void StopServer() {
		if (net != null)
		net.Destroy();
	}

	void Login (string nodename) {
		net.Send(Encaps ("*login,"+nodename+"*"));
	}

	string Encaps (string data) {
		return "*"+data+"*";
	}

	void OnGUI () {
		if (net == null){
		host = GUI.TextField (new Rect (10, 10, 200, 20), host, 25);
		port = GUI.TextField (new Rect (10, 40, 200, 20), port, 25);
		name = GUI.TextField (new Rect (10, 70, 200, 20), name, 25);
			if (GUI.Button(new Rect(10, 100, 100, 30), "Connect"))
				ConnectServer ();

			if (GUI.Button(new Rect(10, 140, 100, 30), "Start Server"))
				StartServer ();
		}
	}


	string RandomString(int length){
		var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
		var stringChars = new char[length];
		var random = new System.Random();
		for (int i = 0; i < stringChars.Length; i++){
			stringChars[i] = chars[random.Next(chars.Length)];
		}
		var finalString = new string(stringChars);
		return finalString;
	}


}
