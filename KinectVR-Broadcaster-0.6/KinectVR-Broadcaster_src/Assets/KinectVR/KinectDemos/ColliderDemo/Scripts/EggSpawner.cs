using UnityEngine;
using System.Collections;

public class EggSpawner : MonoBehaviour 
{
	[Tooltip("Prefab (model and components) used to instantiate eggs in the scene.")]
    public Transform eggPrefab;

    private float nextEggTime = 0.0f;
    private float spawnRate = 1.5f;
 	
	void Update () 
	{
        if (nextEggTime < Time.time)
        {
            SpawnEgg();
            nextEggTime = Time.time + spawnRate;

            spawnRate = Mathf.Clamp(spawnRate, 0.3f, 99f);
        }
	}

    void SpawnEgg()
    {
		KinectManager manager = KinectManager.Instance;

		if(eggPrefab && manager && manager.IsInitialized() && manager.IsUserDetected())
		{
			long userId = manager.GetPrimaryUserID();
			Vector3 posUser = manager.GetUserPosition(userId);

			float addXPos = Random.Range(-2f, 2f);
			Vector3 spawnPos = new Vector3(addXPos, 5f, posUser.z - 0.2f);
			
			Transform eggTransform = Instantiate(eggPrefab, spawnPos, Quaternion.identity) as Transform;
			eggTransform.parent = transform;
		}
    }

}
