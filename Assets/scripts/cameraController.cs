using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraController : MonoBehaviour {

    public float zoomSpeed = 1;
    public float maxZoom = 70;
    public float minZoom = 5;

    public GameObject followObject;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (followObject)
            transform.position = new Vector3(followObject.transform.position.x, followObject.transform.position.y, transform.position.z);

        var scrollAmount = Input.mouseScrollDelta.y;
        if(scrollAmount != 0)
        {
            
            var newZoom = GetComponent<Camera>().orthographicSize + (scrollAmount * zoomSpeed);
            if (newZoom > maxZoom)
                newZoom = maxZoom;
            else if (newZoom < minZoom)
                newZoom = minZoom;

            GetComponent<Camera>().orthographicSize = newZoom;
        }
    }

    public void SetFollowObject(GameObject g)
    {
        followObject = g;
    }
}
