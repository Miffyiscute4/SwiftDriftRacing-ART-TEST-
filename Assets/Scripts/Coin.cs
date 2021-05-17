using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    MeshRenderer mr;
    SphereCollider sc;

    float stopwatch_hideObject;

    public float hideTime = 5;

    bool isHidden;

    void Start()
    {
        mr = GetComponent<MeshRenderer>();
        sc = GetComponent<SphereCollider>();
    }

    void Update()
    {
        if (isHidden)
        {
            mr.enabled = false;
            sc.enabled = false;

            stopwatch_hideObject += Time.deltaTime;

            if (stopwatch_hideObject >= hideTime)
            {
                mr.enabled = true;
                sc.enabled = true;

                stopwatch_hideObject = 0;
                isHidden = false;
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            isHidden = true;   
        }
    }
}
