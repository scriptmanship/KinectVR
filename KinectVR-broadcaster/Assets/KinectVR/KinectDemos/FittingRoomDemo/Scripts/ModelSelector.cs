using UnityEngine;
using System.Collections;
using System.IO;


public class ModelSelector : MonoBehaviour 
{
	[Tooltip("Total number of the available cloth models.")]
	public int numberOfModels = 3;

	[Tooltip("Makes the initial model position relative to this camera, to be equal to the player's position, relative to the sensor.")]
	public Camera modelRelativeToCamera = null;


	private Rect menuWindowRectangle;
	private string[] modelNames;
	private Texture2D[] modelThumbs;

	private Vector2 scroll;
	private int selected;
	private int prevSelected = -1;

	private GameObject selModel;


	void Start()
	{
		modelNames = new string[numberOfModels];
		modelThumbs = new Texture2D[numberOfModels];
		
		for (int i = 0; i < numberOfModels; i++)
		{
			modelNames[i] = string.Format("{0:0000}", i);

			string previewPath = "Clothing/" + modelNames[i] + "/preview.jpg";
			TextAsset resPreview = Resources.Load(previewPath, typeof(TextAsset)) as TextAsset;

			if(resPreview != null)
			{
				modelThumbs[i] = LoadTexture(resPreview.bytes);
			}
		}
	}
	
	void OnGUI()
	{
		menuWindowRectangle = GUI.Window(1, menuWindowRectangle, MenuWindow, "");
	}
	
	void MenuWindow(int windowID)
	{
		menuWindowRectangle = new Rect(Screen.width - 160, 40, 150, Screen.height - 60);
		
		if (modelThumbs != null)
		{
			GUI.skin.button.fixedWidth = 120;
			GUI.skin.button.fixedHeight = 163;
			
			scroll = GUILayout.BeginScrollView(scroll);
			selected = GUILayout.SelectionGrid(selected, modelThumbs, 1);
			
			if (selected >= 0 && selected < modelNames.Length && prevSelected != selected)
			{
				prevSelected = selected;
				LoadModel(modelNames[selected]);
			}
			
			GUILayout.EndScrollView();
			
			GUI.skin.button.fixedWidth = 0;
			GUI.skin.button.fixedHeight = 0;
		}
	}
	
	private Texture2D LoadTexture(byte[] btImage)
	{
		Texture2D tex = new Texture2D(4, 4);
		tex.LoadImage(btImage);
		
		return tex;
	}
	
	private void LoadModel(string modelDir)
	{
		string modelPath = "Clothing/" + modelDir + "/model";
		UnityEngine.Object modelPrefab = Resources.Load(modelPath, typeof(GameObject));
		if(modelPrefab == null)
			return;

		if(selModel != null) 
		{
			GameObject.Destroy(selModel);
		}

		selModel = (GameObject)GameObject.Instantiate(modelPrefab);
		selModel.name = "Model" + modelDir;
		selModel.transform.position = Vector3.zero;
		selModel.transform.rotation = Quaternion.Euler(0, 180f, 0);

		AvatarController ac = selModel.AddComponent<AvatarController>();
		ac.posRelativeToCamera = modelRelativeToCamera;
		ac.mirroredMovement = true;
		ac.verticalMovement = true;
		ac.smoothFactor = 0f;

		KinectManager km = KinectManager.Instance;
		ac.Awake();

		if(km.IsUserDetected())
		{
			ac.SuccessfulCalibration(km.GetPrimaryUserID());
		}

		km.avatarControllers.Clear(); // = new List<AvatarController>();
		km.avatarControllers.Add(ac);

		AvatarScaler scaler = selModel.AddComponent<AvatarScaler>();
		scaler.mirroredAvatar = true;
		scaler.continuousScaling = true;

		scaler.Start();
	}

}
