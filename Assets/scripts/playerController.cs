using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerController : MonoBehaviour {

	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButtonDown(0))
        {
            var newPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            newPoint.z = transform.position.z;

            if (Input.GetKey(KeyCode.LeftShift))
                GameObject.Find("playerShip").GetComponent<shipMovement>().AddToMovementQueue(newPoint);
            else
                GameObject.Find("playerShip").GetComponent<shipMovement>().SetMovementTarget(newPoint);
        }
    }
}
