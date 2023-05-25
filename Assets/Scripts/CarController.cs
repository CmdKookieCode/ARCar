using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    /* This script makes the car move when assembly is completed.
     * Since this script is placed on the car foundation, each assembly part has to be a child of this object.
     * Otherwise, only the car foundation would move.*/

    // Settings
    public float MoveSpeed = 20;
    public float MaxSpeed = 10;
    public float Drag = 0.98f;
    public float SteerAngle = 30;
    public float Traction = 1;

    // Variables
    private Vector3 MoveForce;

    // Update is called once per frame
    void Update()
    {
        // Moving
        MoveForce += transform.forward * MoveSpeed * 1 * Time.deltaTime;
        transform.position += MoveForce * Time.deltaTime;

        // Steering
        float steerInput = Input.GetAxis("Horizontal");
        transform.Rotate(Vector3.up * 1 * MoveForce.magnitude * SteerAngle * Time.deltaTime);

        // Drag and max speed limit
        MoveForce *= Drag;
        MoveForce = Vector3.ClampMagnitude(MoveForce, MaxSpeed);

        // Traction
        Debug.DrawRay(transform.position, MoveForce.normalized * 3);
        Debug.DrawRay(transform.position, transform.forward * 3, Color.blue);
        MoveForce = Vector3.Lerp(MoveForce.normalized, transform.forward, Traction * Time.deltaTime) * MoveForce.magnitude;
    }
}
