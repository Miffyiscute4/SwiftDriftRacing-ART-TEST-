using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    [Header("Instantiate Points")]
    public List<GameObject> point;

    [Header("Still PowerUp Objects")]
    public GameObject boostObject;
    public GameObject dartObject, invincibleObject, bombObject, magnetObject, rocketObject, iceSpikesObject;

    List<GameObject> currentlyDisplayedPowerUp = new List<GameObject> {null, null};

    [Header("Other Objects")]
    public Text speedText; public Text lapText;

    public Player_CarController car;
    public CarCollision carcol;
    public GhostPlayer ghostPlayer;

    public GameObject gainPowerUpParticle;

    Transform transformStore1;
    Transform transformStore2;

    public RawImage UI_CurrentlyEquipped, UI_Circle1, UI_Circle2;

    public Text coinText, timerText, countDownText;
    public CarCollision colPlayer;

    float stopwatch_Sec, restartTime, stopwatch_RaceCountDown, stopwatch_RaceCountDownGo;
        
    int stopwatch_Min, countDownNum = 3, InitialcountDownNum, InitialcountDownNumGo;

    public AudioSource sound_CountDown, sound_CountDownGo;

    [HideInInspector] public bool startTimer;

    public Text endText, lapUiText;

    public CheckpointPlace checkPointPlayer, checkPointGhost;

    //public InitialCheckPointTrigger ghostCheckPoint;

    public Text leaderBoardText, leaderBoardTextSmall, ghostUiText, endScreenTimeText;

    public GameObject endPage;

    public PauseMenu pauseMenu;
    public DifficultySelect difficulty;
    public GameObject coinUi;

    public Color endScreenColour1, endScreenColour2;

    public AudioSource endMusic;

    // Start is called before the first frame update
    void Start()
    {
        transformStore1 = point[1].transform;
        transformStore2 = point[0].transform;

        leaderBoardText.text = "";
        leaderBoardTextSmall.text = "";

        switch (difficulty.ghostNumber)
        {
            case 0:
                ghostUiText.text = "Ghost (Easy)";
                break;

            case 1:
                ghostUiText.text = "Ghost (Normal)";
                break;

            case 2:
                ghostUiText.text = "Ghost (Hard)";
                break;
        }


        //endText.enabled = false;

        endPage.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        lapUiText.text = "Lap: " + (checkPointPlayer.lapCount) + "/" +  (checkPointPlayer.maxLaps - 1);

        if (checkPointPlayer.currentPlace > checkPointGhost.currentPlace)
        {
            leaderBoardText.text = "1";
            leaderBoardText.color = Color.yellow;

            leaderBoardTextSmall.text = "st";
            leaderBoardTextSmall.color = Color.yellow;
        }
        else if (checkPointPlayer.currentPlace < checkPointGhost.currentPlace)
        {
            leaderBoardText.text = "2";
            leaderBoardText.color = Color.gray;

            leaderBoardTextSmall.text = "nd";
            leaderBoardTextSmall.color = Color.gray;
        }



        //speedText.text = "Speed: " + Mathf.Abs(car.currentSpeed);

        lapText.text = "Lap " + carcol.lapCount;

        coinText.text = /*"Coins: " + */colPlayer.coinCount + "";

        if (startTimer)
        {
            stopwatch_Sec += Time.deltaTime;

            if (stopwatch_Sec >= 60)
            {
                stopwatch_Min++;
                stopwatch_Sec = 0;
            }

            if (stopwatch_Sec < 10)
            {
                timerText.text = stopwatch_Min + ":" + "0" + (int)stopwatch_Sec;
            }
            else
            {
                timerText.text = stopwatch_Min + ":" + (int)stopwatch_Sec;
            }

            if (stopwatch_Min >= restartTime)
            {
                GameOver();
            }
        }
        

        
    }

    public void DisplayPowerUp(string powerUpType, int powerUpSlot)
    {
        

        switch (powerUpType)
        {
            case "Boost":
                currentlyDisplayedPowerUp[powerUpSlot] = Instantiate(boostObject, point[powerUpSlot].transform.position, point[powerUpSlot].transform.rotation, point[powerUpSlot].transform.parent);
                break;

            case "Dart":
                currentlyDisplayedPowerUp[powerUpSlot] = Instantiate(dartObject, point[powerUpSlot].transform.position, point[powerUpSlot].transform.rotation, point[powerUpSlot].transform.parent);
                break;

            case "InvincibilityOrb":
                currentlyDisplayedPowerUp[powerUpSlot] = Instantiate(invincibleObject, point[powerUpSlot].transform.position, point[powerUpSlot].transform.rotation, point[powerUpSlot].transform.parent);
                break;

            case "Bomb":
                currentlyDisplayedPowerUp[powerUpSlot] = Instantiate(bombObject, point[powerUpSlot].transform.position, point[powerUpSlot].transform.rotation, point[powerUpSlot].transform.parent);
                break;

            case "Magnet":
                currentlyDisplayedPowerUp[powerUpSlot] = Instantiate(magnetObject, point[powerUpSlot].transform.position, point[powerUpSlot].transform.rotation, point[powerUpSlot].transform.parent);
                break;

            case "Rocket":
                currentlyDisplayedPowerUp[powerUpSlot] = Instantiate(rocketObject, point[powerUpSlot].transform.position, point[powerUpSlot].transform.rotation, point[powerUpSlot].transform.parent);
                break;

            case "IceSpikes":
                currentlyDisplayedPowerUp[powerUpSlot] = Instantiate(iceSpikesObject, point[powerUpSlot].transform.position, point[powerUpSlot].transform.rotation, point[powerUpSlot].transform.parent);
                break;

        }

        Instantiate(gainPowerUpParticle, point[powerUpSlot].transform.position, point[powerUpSlot].transform.rotation, point[powerUpSlot].transform.parent);

        if (powerUpSlot == 1)
        {
            currentlyDisplayedPowerUp[powerUpSlot].transform.localScale /= 2;

            if (currentlyDisplayedPowerUp[1].transform.position == point[0].transform.position)
            {
                currentlyDisplayedPowerUp[1].transform.position = point[0].transform.position;
            }
        }
        else if (powerUpSlot == 0)
        {
            if (currentlyDisplayedPowerUp[0].transform.position == point[1].transform.position)
            {
                currentlyDisplayedPowerUp[0].transform.position = point[1].transform.position;
            }
        }

        


    }

    public void DestroyPreviousObject(int powerUpSlot)
    {
        Destroy(currentlyDisplayedPowerUp[powerUpSlot]);
    }

    public void SwapPowerUps()
    {/*
        if (currentlyDisplayedPowerUp[0].transform.position == point[0].transform.position || currentlyDisplayedPowerUp[1].transform.position == point[1].transform.position)
        {
            if (currentlyDisplayedPowerUp[0] != null)
            {
                currentlyDisplayedPowerUp[0].transform.position = point[1].transform.position;
            }

            if (currentlyDisplayedPowerUp[1] != null)
            {
                currentlyDisplayedPowerUp[1].transform.position = point[0].transform.position;
            }
        }
        else if (currentlyDisplayedPowerUp[0].transform.position == point[1].transform.position || currentlyDisplayedPowerUp[1].transform.position == point[0].transform.position)
        {
            if (currentlyDisplayedPowerUp[0] != null)
            {
                currentlyDisplayedPowerUp[0].transform.position = point[0].transform.position;
            }

            if (currentlyDisplayedPowerUp[1] != null)
            {
                currentlyDisplayedPowerUp[1].transform.position = point[1].transform.position;
            }
        }
        */

        /*
        if (transformStore1 = point[1].transform)
        {
            transformStore1 = point[0].transform;
            transformStore2 = point[1].transform;
        }
        else if (transformStore1 = point[0].transform)
        {
            transformStore1 = point[1].transform;
            transformStore2 = point[0].transform;
        }
        

        point[0].transform.position = transformStore2.position;
        currentlyDisplayedPowerUp[0].transform.position = transformStore2.position;

        point[1].transform.position = transformStore1.position;
        currentlyDisplayedPowerUp[1].transform.position = transformStore1.position;
        */
        if (currentlyDisplayedPowerUp[0] != null)
        {
            if (currentlyDisplayedPowerUp[0].transform.position == point[0].transform.position)
            {
                currentlyDisplayedPowerUp[0].transform.position = point[1].transform.position;
                currentlyDisplayedPowerUp[0].transform.localScale /= 2;
            }
            else if (currentlyDisplayedPowerUp[0].transform.position == point[1].transform.position)
            {
                currentlyDisplayedPowerUp[0].transform.position = point[0].transform.position;
                currentlyDisplayedPowerUp[0].transform.localScale *= 2;
            }
        }


        if (currentlyDisplayedPowerUp[1] != null)
        {
            if (currentlyDisplayedPowerUp[1].transform.position == point[1].transform.position)
            {

                currentlyDisplayedPowerUp[1].transform.position = point[0].transform.position;
                currentlyDisplayedPowerUp[1].transform.localScale *= 2;
            }
            else if (currentlyDisplayedPowerUp[1].transform.position == point[0].transform.position)
            {

                currentlyDisplayedPowerUp[1].transform.position = point[1].transform.position;
                currentlyDisplayedPowerUp[1].transform.localScale /= 2;
            }
        }

        /*if (UI_CurrentlyEquipped.transform.position == UI_Circle1.transform.position)
        {
            UI_CurrentlyEquipped.transform.position = UI_Circle2.transform.position;
            UI_CurrentlyEquipped.transform.localScale = new Vector3(1.8f, 1.8f, 1.8f);
        }
        else if (UI_CurrentlyEquipped.transform.position == UI_Circle2.transform.position)
        {
            UI_CurrentlyEquipped.transform.position = UI_Circle1.transform.position;
            UI_CurrentlyEquipped.transform.localScale = new Vector3(3,3,3);
        }*/


    }

    void GameOver()
    {

    }

    public void RaceCountDown()
    {
        if (InitialcountDownNum < 1)
        {
            countDownText.text = countDownNum + "";
            sound_CountDown.Play();

            InitialcountDownNum++;
        }
        stopwatch_RaceCountDown += Time.deltaTime;

        if (stopwatch_RaceCountDown >= 1)
        {
            /*if (countDownNum < 1)
            {
                countDownText.text = "Go!";
                sound_CountDownGo.Play();

                if (stopwatch_RaceCountDown >= 2)
                {
                    stopwatch_RaceCountDown = 0;
                }
                
            }*/

            if (countDownNum > 1)
            {
                countDownNum--;
                stopwatch_RaceCountDown = 0;

                countDownText.text = countDownNum + "";

                sound_CountDown.Play();
            }

        }
    }

    public void RaceCountDownGo()
    {
        stopwatch_RaceCountDownGo += Time.deltaTime;
        if (InitialcountDownNumGo < 1)
        {
            countDownText.text = "Go!";
            sound_CountDownGo.Play();

            InitialcountDownNumGo++;
        }
        

        if (stopwatch_RaceCountDownGo >= 1)
        {
            stopwatch_RaceCountDown = 0;
            car.isStarting = false;
        }
    }



    public void EndGame()
    {
        Cursor.lockState = CursorLockMode.None;

        car.enabled = false;
        //ghostPlayer.enabled = false;

        endPage.SetActive(true);

        
        pauseMenu.enabled = false;

        //endText.enabled = true;

        if (checkPointPlayer.lapCount > checkPointGhost.lapCount)
        {
            endText.color = endScreenColour2;
            endText.text = "You Win!";
            
            endScreenTimeText.text = "Time:  " + timerText.text;

            foreach (AudioSource a in pauseMenu.carSounds)
            {
                a.Stop();
            }

            pauseMenu.music.Stop();

            endMusic.Play();
        }
        else if (checkPointPlayer.lapCount < checkPointGhost.lapCount)
        {
            endText.color = endScreenColour1;
            endText.text = "You Lose!";

            endScreenTimeText.text = "Time:  " + timerText.text;

            pauseMenu.music.Stop();

            endMusic.Play();
        }
        

    }

}
