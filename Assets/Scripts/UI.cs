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

    public Text speedText;

    public Player_CarController car;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        speedText.text = "Speed: " + Mathf.Abs(car.currentSpeed); 
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
            
        if (powerUpSlot == 1)
        {
            currentlyDisplayedPowerUp[powerUpSlot].transform.localScale /= 2;
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
        
    }
}
