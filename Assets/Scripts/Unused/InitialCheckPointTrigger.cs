using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitialCheckPointTrigger : MonoBehaviour
{

    public int ghostLapCount = 1;
    public Transform ghostCar;
    public CheckpointPlace checkPointPlayer;
    public UI ui;

    bool isInRangeOfGhost;

    float stopwatch_range;

    void Start()
    {
        isInRangeOfGhost = false;
    }
    void Update()
    {/*
        if (Vector3.Distance(transform.position, ghostCar.position) < 45 && isInRangeOfGhost)
        {
            ghostLapCount++;
            isInRangeOfGhost = false;
        }
        else
        {
            stopwatch_range += Time.deltaTime;
            if (stopwatch_range >= 3)
            {
                isInRangeOfGhost = true;
                stopwatch_range = 0;
            }
            
        }

        if (ghostLapCount >= checkPointPlayer.maxLaps)
        {
            ui.EndGame();
        }
    }*/
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("working");
    }
}
