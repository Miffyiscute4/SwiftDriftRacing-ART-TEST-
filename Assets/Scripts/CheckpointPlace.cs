using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointPlace : MonoBehaviour
{
    [HideInInspector] public int currentPlace = 0;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "CheckPoint")
        {
            currentPlace++;
        }
    }
}
