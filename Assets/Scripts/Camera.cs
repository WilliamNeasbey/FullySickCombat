using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour
{
    public Transform target;                // The target to follow (player character)
    public float distance = 5.0f;           // Distance from the target
    public float rotationSpeed = 5.0f;      // Camera rotation speed

    private float currentRotation = 0.0f;   // Current rotation around the target

    private void LateUpdate()
    {
        if (target == null)
            return;

        // Calculate the desired rotation based on user input
        currentRotation += Input.GetAxis("Mouse X") * rotationSpeed;

        // Calculate the camera position based on the rotation and distance from the target
        Vector3 offset = Quaternion.Euler(0.0f, currentRotation, 0.0f) * new Vector3(0.0f, 0.0f, -distance);
        Vector3 desiredPosition = target.position + offset;

        // Smoothly move the camera towards the desired position
        transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * rotationSpeed);

        // Make the camera look at the target
        transform.LookAt(target);
    }
}
