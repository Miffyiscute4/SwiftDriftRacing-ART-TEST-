using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    //colliders
    Transform tf;
    public float timeBeforeDestroy;
    public float AmountToAdd;

    //stopwatches
    float explosionStopWatch;

    private void Start()
    {
        tf = transform;
    }
    // Update is called once per frame
    void Update()
    {
        explosionStopWatch += Time.deltaTime;

        tf.localScale = new Vector3(tf.localScale.x + AmountToAdd, tf.localScale.y + AmountToAdd, tf.localScale.z + AmountToAdd) ;
        
        if (explosionStopWatch >= timeBeforeDestroy)
        {
            Destroy(gameObject);
        }
    }
}
