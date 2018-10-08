using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class turret : MonoBehaviour {

    public float rotationSpeed = 30;
    public float baseDamage = 10;
    public float shieldMultiplier = 1;
    public float shotsPerSecond = 1;
    public float range = 500;
    public GameObject shotPrefab;
    public float shotForwardStart = 2;
    public float shotSpeed = 500;
    public float accuracy = 100;

    private hardpoint assignedHardpoint;
    private shipCombat shipCombat;
    private float timeSinceLastShot;

    // Use this for initialization
    void Start () {
        assignedHardpoint = transform.parent.GetComponent<hardpoint>();
        shipCombat = transform.parent.parent.GetComponent<shipCombat>();
    }
	
	// Update is called once per frame
	void Update () {
        timeSinceLastShot += Time.deltaTime;
        if (shipCombat.currentTarget != null)
        {
            RotateTowardsTarget();

            if (timeSinceLastShot > 1 / shotsPerSecond && Random.Range(0.0f, 1.0f) > 0.95f && Mathf.Abs(getAngleToTarget(shipCombat.currentTarget.transform.position)) < 3)
            {
                FireShot();
                timeSinceLastShot = 0;
            }
        }
	}

    void FireShot()
    {
        var newShot = Instantiate(shotPrefab, transform.position + (transform.right * shotForwardStart), transform.rotation);
        var shotComponent = newShot.GetComponent<shot>();
        shotComponent.range = range;
        shotComponent.speed = shotSpeed;
        shotComponent.baseDamage = baseDamage;
        shotComponent.shieldMultiplier = shieldMultiplier;
        shotComponent.sourceShip = transform.parent.parent.gameObject;

        var accuracyAngleDiff = 100 - accuracy;
        if (accuracyAngleDiff < 0)
            accuracyAngleDiff = 0;

        var randomAngle = Random.Range(-accuracyAngleDiff, accuracyAngleDiff);
        newShot.transform.Rotate(new Vector3(0, 0, 1), randomAngle);
    }

    void RotateTowardsTarget()
    {
        var angleToTarget = getAngleToTarget(shipCombat.currentTarget.transform.position);
        if(angleToTarget != 0)
        {
            var sign = Mathf.Sign(angleToTarget);
            var rotationAmount = rotationSpeed * Time.deltaTime;
            if (Mathf.Abs(angleToTarget) < rotationAmount)
                rotationAmount = Mathf.Abs(angleToTarget);

            var currentZRotation = transform.localRotation.eulerAngles.z > 180 ? transform.localRotation.eulerAngles.z - 360 : transform.localRotation.eulerAngles.z;
            //Debug.Log("Sign: " + sign + " | Rotation amount: " + rotationAmount + " | RotZ: " + currentZRotation);            
            if (sign > 0 && rotationAmount + currentZRotation > assignedHardpoint.maxLeftRotation)
                return;
            else if (sign < 0 && -rotationAmount + currentZRotation < -assignedHardpoint.maxRightRotation)
                return;

            transform.Rotate(new Vector3(0,0,1), sign * rotationAmount);
        }
    }

    float getAngleToTarget(Vector2 targetPosition)
    {
        var forward = (Vector2)transform.right;
        var toTarget = targetPosition - (Vector2)transform.position;

        var sign = Mathf.Sign(forward.x * toTarget.y - forward.y * toTarget.x);
        return sign * Vector2.Angle(forward, toTarget);
    }
}
