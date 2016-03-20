using UnityEngine;
using System.Collections;

public class EggMover : MonoBehaviour 
{
    void Awake()
    {
        //GetComponent<Rigidbody>().AddForce(new Vector3(0, -10f, 0), ForceMode.Force);
    }

	void Update () 
	{
        if (transform.position.y < -10)
        {
            Destroy(gameObject);
        }
	}
}
