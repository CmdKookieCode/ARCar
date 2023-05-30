using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    /* This script makes the car move when assembly is completed. This can be done by changing the position of the car foundation.
     * This script is placed on the car foundation GameObject, that way we can easily change the transform.
     * Since we made each assembly part a child of the foundation object, they will move with their parent object.
     * Otherwise, only the car foundation would move. 
     * You can take a look at this script but you do not need to fill in any gaps. */

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
        // Moving up
        MoveForce += transform.forward * MoveSpeed * 1 * Time.deltaTime;
        transform.position += MoveForce * Time.deltaTime;

        // Steering to the right
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
