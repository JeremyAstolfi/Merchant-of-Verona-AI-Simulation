using UnityEngine;
using System.Collections;

public class PathManager : MonoBehaviour {
	
	GameObject pathGuy; // a PathFollower

	//prefabs
	public GameObject PathFollowerPrefab;

	// These weights will be exposed in the Inspector window
	public float seekWt = 75.0f;

	void Start () {
		//make a target

		//make a PatFinder;
		Vector3 pos = new Vector3 (0, 1.0f, 0);
		pathGuy = (GameObject)GameObject.Instantiate (PathFollowerPrefab, pos, Quaternion.identity);
		
		//tell the camera to follow myGuy
		Camera.main.GetComponent<SmoothFollow>().target = pathGuy.transform;
		
	}
	
}
