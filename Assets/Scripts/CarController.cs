using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    public float speed;
    public float turnSpeed;
    public float gravityMultiplier;


    Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.W))
        {
            rb.AddRelativeForce(Vector3.forward.x, 0, Vector3.forward.z * speed);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            rb.AddRelativeForce(Vector3.forward.x, 0, Vector3.forward.z * -speed);
        }

        if (Input.GetKey(KeyCode.D))
        {
            rb.AddTorque(Vector3.up * turnSpeed);
        }
        else if (Input.GetKey(KeyCode.A))
        {
            rb.AddTorque(-Vector3.up * turnSpeed);
        }

        rb.AddForce(Vector3.down * gravityMultiplier);

        Vector3 localVelocity = transform.InverseTransformDirection(rb.velocity);
        localVelocity.x = 0;
        rb.velocity = transform.TransformDirection(localVelocity);
    }
}
