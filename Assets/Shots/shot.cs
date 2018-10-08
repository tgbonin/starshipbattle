using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shot : MonoBehaviour {

	public float baseDamage;
    public float shieldMultiplier;
    public float range; 
    public float speed;
    public GameObject sourceShip;
    public GameObject hullCollisionEffect;
    public GameObject shieldCollisionEffect;
    public float percentChanceToHitPerFrame = 1;

	// Update is called once per frame
	void Update () {
        Debug.DrawLine(transform.position, transform.position + transform.right * 10);
        transform.position += transform.right * (speed * Time.deltaTime);
        range -= speed * Time.deltaTime;
        if (range < 0)
            Destroy(gameObject);
	}

    void OnTriggerEnter2D(Collider2D collidedWith)
    {
        Debug.Log("Collision Entered");
        if(collidedWith.gameObject.tag == "Ship" && collidedWith.gameObject != sourceShip)
        {
            //if (percentChanceToHitPerFrame > Random.Range(0, 100))
            //{
                Instantiate(hullCollisionEffect, transform.position, Quaternion.identity);
                Destroy(gameObject);
            //}
        }
        else if (collidedWith.gameObject.tag == "asteroid")
        {
            Instantiate(hullCollisionEffect, transform.position, Quaternion.identity);
        }
    }
}
