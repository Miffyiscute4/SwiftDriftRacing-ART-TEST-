using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomScale : MonoBehaviour
{

    public float maxScale;
    // Start is called before the first frame update
    void Start()
    {
        transform.localScale = new Vector3(Random.Range(0, maxScale), Random.Range(0, maxScale), Random.Range(0, maxScale));
    }
}
