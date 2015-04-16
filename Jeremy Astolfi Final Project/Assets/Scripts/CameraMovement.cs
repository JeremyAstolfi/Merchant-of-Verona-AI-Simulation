using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour {

    private GameObject followingGO;
    float yZoom = 0;
    float zZoom = 0;

	// Use this for initialization
	void Start () {
        followingGO = GameObject.FindGameObjectWithTag("Merchant");
	}
	
	// Update is called once per frame
	void Update () {
        transform.position = new Vector3(followingGO.transform.position.x, followingGO.transform.position.y + 6 + yZoom, followingGO.transform.position.z - 6 - zZoom);
        Change();
	}

    private void Change()
    {
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.RotateAround(followingGO.transform.position, Vector3.up, 2);
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.RotateAround(followingGO.transform.position, Vector3.up, -2);
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            if (yZoom > 0)
            {
                yZoom -= 0.5f;
                zZoom -= 0.5f;
            }
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            if (yZoom < 10)
            {
                yZoom += 0.5f;
                zZoom += 0.5f;
            }
        }
    }
}
