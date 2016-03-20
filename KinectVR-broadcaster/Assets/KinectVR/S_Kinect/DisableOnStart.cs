using UnityEngine;
using System.Collections;

public class DisableOnStart : MonoBehaviour {

    // Use this for initialization
    void Start () 
    {
        gameObject.SetActive (false);
    }
}
