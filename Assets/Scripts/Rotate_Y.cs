using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate_Y : MonoBehaviour
{
    public float rotateSpeed = 1;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3(0, 100 * rotateSpeed, 0) * Time.deltaTime);
    }
}
