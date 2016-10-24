using UnityEngine;
using System.Collections;

public class KinectPlayerController : MonoBehaviour 
{
	private SpeechManager speechManager;
	private KinectRecorderPlayer saverPlayer;


	void Start()
	{
		saverPlayer = KinectRecorderPlayer.Instance;
	}

	void Update () 
	{
		if(speechManager == null)
		{
			speechManager = SpeechManager.Instance;
		}
		
		if(speechManager != null && speechManager.IsSapiInitialized())
		{
			if(speechManager.IsPhraseRecognized())
			{
				string sPhraseTag = speechManager.GetPhraseTagRecognized();
				
				switch(sPhraseTag)
				{
					case "RECORD":
						if(saverPlayer)
						{
							saverPlayer.StartRecording();
						}
						break;
						
					case "PLAY":
						if(saverPlayer)
						{
							saverPlayer.StartPlaying();
						}
						break;

					case "STOP":
						if(saverPlayer)
						{
							saverPlayer.StopRecordingOrPlaying();
						}
						break;

				}
				
				speechManager.ClearPhraseRecognized();
			}
		}

		// alternatively, use the keyboard
		if(Input.GetButtonDown("Jump"))  // start or stop recording
		{
			if(saverPlayer)
			{
				if(!saverPlayer.IsRecording())
				{
					saverPlayer.StartRecording();
				}
				else
				{
					saverPlayer.StopRecordingOrPlaying();
				}
			}
		}

		if(Input.GetButtonDown("Fire1"))  // start or stop playing
		{
			if(saverPlayer)
			{
				if(!saverPlayer.IsPlaying())
				{
					saverPlayer.StartPlaying();
				}
				else
				{
					saverPlayer.StopRecordingOrPlaying();
				}
			}
		}

	}

}
