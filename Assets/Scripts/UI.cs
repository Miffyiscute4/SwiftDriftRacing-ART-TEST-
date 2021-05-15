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

    public GameObject gainPowerUpParticle;

    Transform transformStore1;
    Transform transformStore2;

    public RawImage UI_CurrentlyEquipped, UI_Circle1, UI_Circle2;

    public Text coinText, timerText;
    public CarCollision colPlayer;

    float stopwatch_Sec, restartTime;
        
    int stopwatch_Min;

    // Start is called before the first frame update
    void Start()
    {
        transformStore1 = point[1].transform;
        transformStore2 = point[0].transform;
    }

    // Update is called once per frame
    void Update()
    {
        //speedText.text = "Speed: " + Mathf.Abs(car.currentSpeed);

        lapText.text = "Lap " + carcol.lapCount;

        coinText.text = "Coins: " + colPlayer.coinCount;

        stopwatch_Sec += Time.deltaTime;

        if (stopwatch_Sec >= 60)
        {
            stopwatch_Min++;
            stopwatch_Sec = 0;
        }

        if (stopwatch_Sec < 10)
        {
            timerText.text = "Time: " + stopwatch_Min + ":" + "0" + (int)stopwatch_Sec;
        }
        else
        {
            timerText.text = "Time: " + stopwatch_Min + ":" + (int)stopwatch_Sec;
        }

        if (stopwatch_Min >= restartTime)
        {
            GameOver();
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
}
