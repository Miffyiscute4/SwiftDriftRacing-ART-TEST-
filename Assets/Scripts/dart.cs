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
        transform.Translate(Vector3.forward* speed);



        destroyTimer = Time.deltaTime;
        if (destroyTimer >= 7.5)
        {
            Destroy(gameObject);
            destroyTimer = 0;
        }
    }

    void OnTriggernEnter(Collider other)
    {
        Destroy(gameObject);
    }
}
