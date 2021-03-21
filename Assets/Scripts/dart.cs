using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dart : MonoBehaviour
{

    float destroyTimer;


    // Update is called once per frame
    void Update()
    {
        transform.position = transform.forward;


        destroyTimer = Time.deltaTime;
        if (destroyTimer >= 7.5)
        {
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter(Collision other)
    {
        Destroy(gameObject);
    }
}
