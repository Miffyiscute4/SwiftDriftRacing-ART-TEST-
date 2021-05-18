using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostPlayer : MonoBehaviour
{
    public Ghost ghost;
    float timeValue;
    int index1;
    int index2;

    void Awake()
    {
        timeValue = 0;
    }

    void Update()
    {
        timeValue += Time.deltaTime;

        if (ghost.isReplay)
        {
            GetIndex();
            SetTransform();
        }
    }

    void GetIndex()
    {
        for (int i = 0; i < ghost.timeStamp.Count - 2; i++)
        {
            if (ghost.timeStamp[i] == timeValue)
            {
                index1 = i;
                index2 = i;
                return;
            }
            else if (ghost.timeStamp[i] < timeValue && timeValue < ghost.timeStamp[i + 1])
            {
                index1 = i;
                index2 = i + 1;
                return;
            }

            

        }

        index1 = ghost.timeStamp.Count - 1;
        index2 = ghost.timeStamp.Count - 1;

    }

    void SetTransform()
    {
        if (index1 == index2)
        {
            this.transform.position = ghost.position[index1];
            this.transform.eulerAngles = ghost.rotation[index1];
        }
        else
        {
            float interpolationFactor = (timeValue - ghost.timeStamp[index1]) / (ghost.timeStamp[index2] - ghost.timeStamp[index1]);

            this.transform.position = Vector3.Lerp(ghost.position[index1], ghost.position[index2], interpolationFactor);
            this.transform.eulerAngles = Vector3.Lerp(ghost.rotation[index1], ghost.rotation[index2], interpolationFactor);
        }
   
    }
}
