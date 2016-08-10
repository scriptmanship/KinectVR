using UnityEngine;
using System.Collections;

public class ForegroundToImage : MonoBehaviour 
{

	void Update () 
	{
		BackgroundRemovalManager backManager = BackgroundRemovalManager.Instance;

		if(backManager && backManager.IsBackgroundRemovalInitialized())
		{
			GUITexture guiTexture = GetComponent<GUITexture>();

			if(guiTexture && guiTexture.texture == null)
			{
				guiTexture.texture = backManager.GetForegroundTex();
			}
		}
	}

}
