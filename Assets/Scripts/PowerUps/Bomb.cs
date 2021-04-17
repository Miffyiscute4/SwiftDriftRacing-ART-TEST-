using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    public LayerMask whatIsGround;
    public GameObject explosion, prefabParent;
    Animator anim;
    Rigidbody rb;

    float stopwatch_direction;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();

        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        stopwatch_direction += Time.deltaTime;

        if (stopwatch_direction <= 0.3f)
        {
            rb.AddForce(transform.forward * 1000);
            rb.AddForce(transform.up * 1000);
        }
        else
        {
            rb.AddForce(transform.forward * 1000);
            rb.AddForce(-transform.up * 1000);
        }

    }

    void DestroyObject()
    {
        Instantiate(explosion, transform.position, transform.rotation);
        Destroy(prefabParent);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 8)
        {
            DestroyObject();
        }
    }
}

