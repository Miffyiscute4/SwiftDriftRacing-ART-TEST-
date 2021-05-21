using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Debug_AdjustTimeScale : MonoBehaviour
{
    [Range(0, 100)] public int timeScaleValue;

    // Update is called once per frame
    void Update()
    {
        Time.timeScale = timeScaleValue;
    }
}
