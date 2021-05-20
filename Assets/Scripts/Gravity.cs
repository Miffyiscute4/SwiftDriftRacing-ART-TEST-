using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gravity : MonoBehaviour
{
    public Rigidbody rb;
    public float speed;

    // Update is called once per frame
    void Update()
    {
        rb.AddForce(-transform.up * speed);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            rb.isKinematic = true;
        }
        
    }
}
