using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Ghost : ScriptableObject
{
    public bool isRecord;
    public bool isReplay;
    public float recordFrequency;

    public List<float> timeStamp;
    public List<Vector3> position;
    public List<Vector3> rotation;

    public void ResetData()
    {
        timeStamp.Clear();
        position.Clear();
        rotation.Clear();
    }

}
