using UnityEngine;
using System.Collections;

public class GrabRotateScript : MonoBehaviour 
{
	[Tooltip("Material used to outline the object when selected.")]
	public Material selectedObjectMaterial;

	[Tooltip("Smooth factor used for object rotations.")]
	public float smoothFactor = 3.0f;

	[Tooltip("GUI-Text used to display information messages.")]
	public GUIText infoGuiText;

	
	private InteractionManager manager;
	private bool isLeftHandDrag;

	private GameObject selectedObject;
	private Material savedObjectMaterial;


	void Update() 
	{
		manager = InteractionManager.Instance;

		if(manager != null && manager.IsInteractionInited())
		{
			Vector3 screenNormalPos = Vector3.zero;
			Vector3 screenPixelPos = Vector3.zero;

			if(selectedObject == null)
			{
				// no object is currently selected or dragged.
				// if there is a hand grip, try to select the underlying object and start dragging it.
				if(manager.IsLeftHandPrimary())
				{
					// if the left hand is primary, check for left hand grip
					if(manager.GetLastLeftHandEvent() == InteractionManager.HandEventType.Grip)
					{
						isLeftHandDrag = true;
						screenNormalPos = manager.GetLeftHandScreenPos();
					}
				}
				else if(manager.IsRightHandPrimary())
				{
					// if the right hand is primary, check for right hand grip
					if(manager.GetLastRightHandEvent() == InteractionManager.HandEventType.Grip)
					{
						isLeftHandDrag = false;
						screenNormalPos = manager.GetRightHandScreenPos();
					}
				}
				
				// check if there is an underlying object to be selected
				if(screenNormalPos != Vector3.zero)
				{
					// convert the normalized screen pos to pixel pos
					screenPixelPos.x = (int)(screenNormalPos.x * Camera.main.pixelWidth);
					screenPixelPos.y = (int)(screenNormalPos.y * Camera.main.pixelHeight);
					Ray ray = Camera.main.ScreenPointToRay(screenPixelPos);
					
					// check for underlying objects
					RaycastHit hit;
					if(Physics.Raycast(ray, out hit))
					{
						if(hit.collider.gameObject == gameObject)
						{
							selectedObject = gameObject;
						
							savedObjectMaterial = selectedObject.GetComponent<Renderer>().material;
							selectedObject.GetComponent<Renderer>().material = selectedObjectMaterial;
						}
					}
				}
				
			}
			else
			{
				// continue dragging the object
				screenNormalPos = isLeftHandDrag ? manager.GetLeftHandScreenPos() : manager.GetRightHandScreenPos();

				float angleArounfY = screenNormalPos.x * 360f;  // horizontal rotation
				float angleArounfX = screenNormalPos.y * 360f;  // vertical rotation
				Vector3 newObjectRotation = new Vector3(-angleArounfX,-angleArounfY,180);

				selectedObject.transform.rotation = Quaternion.Slerp(selectedObject.transform.rotation, Quaternion.Euler(newObjectRotation), smoothFactor * Time.deltaTime);

				// check if the object (hand grip) was released
				bool isReleased = isLeftHandDrag ? (manager.GetLastLeftHandEvent() == InteractionManager.HandEventType.Release) :
					(manager.GetLastRightHandEvent() == InteractionManager.HandEventType.Release);

				if(isReleased)
				{
					// restore the object's material and stop dragging the object
					selectedObject.GetComponent<Renderer>().material = savedObjectMaterial;
					selectedObject = null;
				}
			}
		}
	}


	
	void OnGUI()
	{
		if(infoGuiText != null && manager != null && manager.IsInteractionInited())
		{
			string sInfo = string.Empty;
			
			long userID = manager.GetUserID();
			if(userID != 0)
			{
				if(selectedObject != null)
					sInfo =  selectedObject.name + " grabbed. You can turn it now in all directions.";
				else
					sInfo = "Please grab the object to turn it.";
			}
			else
			{
				KinectManager kinectManager = KinectManager.Instance;

				if(kinectManager && kinectManager.IsInitialized())
				{
					sInfo = "Waiting for Users...";
				}
				else
				{
					sInfo = "Kinect is not initialized. Check the log for details.";
				}
			}
			
			infoGuiText.GetComponent<GUIText>().text = sInfo;
		}
	}

}
