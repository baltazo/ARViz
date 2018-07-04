using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleARCore;

public class FurnitureController : MonoBehaviour {

	private GameObject selectedFurnitureInstance;
	private Anchor anchor;

	public bool furnitureSpawned = false;
	public GameObject selectedFurniturePrefab;

	public void SpawnSelectedFurniture(GameObject selectedObject, Quaternion testPlaneRotation)
	{
		if(anchor != null)
        {
           Destroy(anchor);
        }

        selectedFurniturePrefab = selectedObject;

		// Create Anchor at the position and rotation of the testPlane
		GameObject testPlanePositioned = GameObject.FindWithTag("TestPlane");
		Vector3 anchorPosition = testPlanePositioned.transform.position;
		Quaternion anchorRotation = testPlanePositioned.transform.rotation;

        TrackableHit hit;
        TrackableHitFlags raycastFilter = TrackableHitFlags.PlaneWithinBounds | TrackableHitFlags.PlaneWithinPolygon;

        if (Frame.Raycast(Screen.width / 2, Screen.height / 2, raycastFilter, out hit))
        {
            selectedFurnitureInstance = Instantiate(selectedFurniturePrefab, hit.Pose.position, testPlaneRotation);
            anchor = hit.Trackable.CreateAnchor(hit.Pose);
            selectedFurnitureInstance.transform.parent = anchor.transform;
            furnitureSpawned = true;
        }

	}

    public void RemoveFurniture()
    {
        if (!furnitureSpawned)
        {
            return;
        }
    	Destroy(selectedFurnitureInstance);
        Destroy(GameObject.Find("Anchor"));
    	furnitureSpawned = false;
    }
}
