using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dart : MonoBehaviour
{

    float destroyTimer;
    public float speed;

    // Update is called once per frame
    void Update()
    {
        transform.Translate(-transform.right * speed);



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
