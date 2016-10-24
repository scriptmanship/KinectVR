using UnityEngine;
using System.Collections;

public class JumpTrigger : MonoBehaviour 
{
	void OnTriggerEnter()
	{
		//Debug.Log ("Jump trigger activated");

		// start the animation clip
		Animation animation = gameObject.GetComponent<Animation>();
		if(animation != null)
		{
			animation.Play();
		}

		// play the audio clip
		AudioSource audioSrc = gameObject.GetComponent<AudioSource>();
		if(audioSrc != null)
		{
			audioSrc.Play();
		}
	}
}
