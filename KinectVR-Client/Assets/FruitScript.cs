using UnityEngine;
using System.Collections;

public class FruitScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnCollisionEnter (Collision col){
		Instantiate (ObjectManager.s.splatParticle, transform.position, ObjectManager.s.splatParticle.rotation);
		Destroy (gameObject);

	}
}
