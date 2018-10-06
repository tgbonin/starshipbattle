using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shipMovement : MonoBehaviour {

    public float maxForwardSpeed;
    public float forwardAcceleration;
    public float maxRotationSpeed;
    public float rotationalAcceleration;
    public float currentForwardSpeed;
    public float currentRotationalSpeed;
    
    List<Vector2> movementQueue;
    List<GameObject> movementLines;

    void Start()
    {
        movementQueue = new List<Vector2>();
        movementLines = new List<GameObject>();
    }

	// Update is called once per frame
	void Update () {
        if (movementQueue.Count != 0)
            MoveTowardsLocation((Vector2)movementQueue[0]);
        else
        {
            if (currentForwardSpeed != 0)
            {
                currentForwardSpeed -= Mathf.Abs(forwardAcceleration * Time.deltaTime);
                if (currentForwardSpeed < 0.01)
                    currentForwardSpeed = 0;

                transform.Translate(Vector3.right * (currentForwardSpeed * Time.deltaTime));
            }
            if(currentRotationalSpeed != 0)
            {
                var sign = Mathf.Sign(currentRotationalSpeed);
                currentRotationalSpeed = Mathf.Abs(currentRotationalSpeed) - (rotationalAcceleration * Time.deltaTime);
                currentRotationalSpeed *= sign;

                var rotationAmount = currentRotationalSpeed * Time.deltaTime;
                if (Mathf.Abs(rotationAmount) < 0.05)
                    rotationAmount = 0;
                transform.Rotate(new Vector3(0, 0, 1), rotationAmount);
            }
        }

        if(movementQueue.Count != 0)
            UpdateMovementLines();
    }

    void MoveTowardsLocation(Vector2 targetPosition)
    {
        /* Considering the Angle to Target
         * - Negative angle means right of forward
         * - Positive angle means left of forward
         * - Less than abs(90) means ahead
         * - Greater than abs(90) means behind
         */

        var distanceToTarget = Mathf.Abs((targetPosition - (Vector2)transform.position).magnitude);
        if(distanceToTarget < 5)
        {
            var moveLine = movementLines[0];
            movementLines.RemoveAt(0);
            Destroy(moveLine);

            movementQueue.RemoveAt(0);

            return;
        }

        // Rotation
        var angleToTarget = getAngleToTarget(targetPosition);
        Debug.Log("Angle to Target: " + angleToTarget);
        if(angleToTarget != 0)
        {
            var secondsToZeroRotationSpeed = Mathf.Abs(currentRotationalSpeed) / rotationalAcceleration;
            var secondsToTargetRotation = Mathf.Abs(angleToTarget) / Mathf.Abs(currentRotationalSpeed);

            float sign;
            sign = Mathf.Sign(angleToTarget);

            if (secondsToZeroRotationSpeed > secondsToTargetRotation)
                currentRotationalSpeed = Mathf.Abs(currentRotationalSpeed) - (rotationalAcceleration * Time.deltaTime);
            else if (currentRotationalSpeed < maxRotationSpeed)
                currentRotationalSpeed = Mathf.Abs(currentRotationalSpeed) + (rotationalAcceleration * Time.deltaTime);

            currentRotationalSpeed *= sign;

            var rotationAmount = currentRotationalSpeed * Time.deltaTime;
            if (Mathf.Abs(rotationAmount) > Mathf.Abs(angleToTarget))
                rotationAmount = angleToTarget;
            else if (Mathf.Abs(rotationAmount) < 0.01)
                rotationAmount = 0;
            transform.Rotate(new Vector3(0,0,1), rotationAmount);
        }

        // Forward Movement
        var secondsToZeroForwardSpeed = currentForwardSpeed / forwardAcceleration;
        var secondsToTargetLocation = distanceToTarget / currentForwardSpeed;

        Debug.Log(secondsToZeroForwardSpeed + "::" + secondsToTargetLocation);

        if (secondsToZeroForwardSpeed > secondsToTargetLocation) {
            if (movementQueue.Count == 1)
                currentForwardSpeed -= Mathf.Abs(forwardAcceleration * Time.deltaTime);
        }
        else if (currentForwardSpeed < maxForwardSpeed)
            currentForwardSpeed += Mathf.Abs(forwardAcceleration * Time.deltaTime);

        if (currentForwardSpeed < 0.01)
            currentForwardSpeed = 0;
        transform.Translate(Vector3.right * (currentForwardSpeed * Time.deltaTime));
    }
    
    float getAngleToTarget(Vector2 targetPosition)
    {
        var forward = (Vector2)transform.right;
        var toTarget = targetPosition - (Vector2)transform.position;

        var sign = Mathf.Sign(forward.x * toTarget.y - forward.y * toTarget.x);
        return sign * Vector2.Angle(forward, toTarget);
    }

    public void SetMovementTarget(Vector2 targetPoint)
    {
        movementQueue.RemoveRange(0, movementQueue.Count);

        foreach (var line in movementLines)
            Destroy(line);
        movementLines.RemoveRange(0, movementLines.Count);
        AddToMovementQueue(targetPoint);
    }

    public void AddToMovementQueue(Vector2 targetPoint)
    {
        Vector2 lineStart = movementQueue.Count == 0 ? (Vector2)transform.position : movementQueue[movementQueue.Count - 1];
        CreateMovementLine(lineStart, targetPoint, Color.green);
        movementQueue.Add(targetPoint);
    }

    void CreateMovementLine(Vector2 from, Vector2 to, Color color)
    {
        GameObject myLine = new GameObject();
        myLine.transform.position = from;
        myLine.AddComponent<LineRenderer>();
        LineRenderer lr = myLine.GetComponent<LineRenderer>();
        lr.material = new Material(Shader.Find("Particles/Alpha Blended Premultiply"));
        lr.startColor = color;
        lr.endColor = color;
        lr.startWidth = 0.1f;
        lr.endWidth = 0.1f;
        lr.SetPosition(0, from);
        lr.SetPosition(1, new Vector3(to.x, to.y));

        movementLines.Add(myLine);
    }
    
    void UpdateMovementLines()
    {
        movementLines[0].GetComponent<LineRenderer>().SetPosition(0, gameObject.transform.position);
    }
}
