using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CheckpointPlace : MonoBehaviour
{
    public bool isPlayer;

    public int maxLaps = 2;

      public int currentPlace = 0;

    public GameObject checkPointsObject;

    Transform[] checkPoints;

    int currentCheckPointNum = 1;

    bool isFinished;

    CarCollision carCol;
    Player_CarController carPlayer;

    public int lapCount = 1;
    public Text checkPointBoxText;

    public UI ui;

    [HideInInspector] public bool gameEnded = false;
    public bool isTutorial = false;

    void Start()
    {
        checkPoints = checkPointsObject.GetComponentsInChildren<Transform>();

        

        Debug.Log(checkPoints.Length + "_____" + checkPoints[0]);

        //currentCheckPointNum = 1;

        if (isPlayer)
        {
            carCol = GetComponent<CarCollision>();
            carPlayer = carCol.carPlayer;
        }    

        /*switch (ghostDifficulty.ghostNumber)
        {
            case 0:
                raceTime = ;
                break;

            case 1:
                raceTime = 230;
                break;

            case 2:
                raceTime = 193;
                break;
        }*/
    }

    void Update()
    {
        if (isPlayer)
        {
            checkPointBoxText.text = "Lap " + lapCount;
        }

        if (lapCount >= maxLaps && !gameEnded)
        {
            ui.EndGame();

            gameEnded = true;

            Debug.Log("End of game");
        }

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "CheckPoint")
        {
            if (currentCheckPointNum != checkPoints.Length - 1)
            {
                if (other.gameObject.transform == checkPoints[currentCheckPointNum + 1])
                {
                    currentCheckPointNum++;
                    currentPlace++;

                    //checkpoints.length has to subtract 1 as it does not start at 0
                    if (other.gameObject.transform == checkPoints[checkPoints.Length - 1])
                    {
                        isFinished = true;
                    }

                }

                if (isPlayer && currentCheckPointNum != checkPoints.Length - 1)
                {
                    if (other.gameObject.transform == checkPoints[1])
                    {
                        carCol.PlayAnimation();
                    }
              
                    if (other.gameObject.transform != checkPoints[currentCheckPointNum + 1] && other.gameObject.transform != checkPoints[currentCheckPointNum] && !isTutorial)
                    {
                        RespawnPlayer();
                    }
                }

                

            }   
            
            
            // it is checkpoints[1] because the first gameobject doesn't count
            if (isFinished && other.gameObject.transform == checkPoints[1])
            {
                currentCheckPointNum = 1;
                currentPlace++;

                lapCount++;

                isFinished = false;
            }

       
        }

        if (other.gameObject.tag == "DestroyZone")
        {
            RespawnPlayer();
        }
    }

    public void RespawnPlayer()
    {
        carCol.transform.position = checkPoints[currentCheckPointNum].position;
        carPlayer.transform.rotation = checkPoints[currentCheckPointNum].rotation;
    }
}
