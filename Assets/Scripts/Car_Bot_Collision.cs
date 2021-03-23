using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car_Bot_Collision : MonoBehaviour
{
    public AudioSource powerUpBoxSound;
    public Car_Bot carBot;

    public GameObject coinObject, bigCoinObject, powerUpBoxObject;
    public Transform coinInstantiatePoint;

    private float lavaTimer = 0, PowerUpRegenTimer = 0, usePowerUpTimer = 0;
    public int coinCount;


    string powerUpSlot1, powerUpSlot2;
    int currentPowerUpSlot, randomNum1;
    public Transform powerUpInstantiatePoint, carDirection;

    float powerUpBoxDelay;

    public string[] powerUpType;

    public GameObject dartObject;


    // Start is called before the first frame update
    void Start()
    {
        randomNum1 = Random.Range(1, 10);

        currentPowerUpSlot = 1;
    }

    // Update is called once per frame
    void Update()
    {

        //Debug.Log(usePowerUpTimer + " " + randomNum1);

        usePowerUpTimer += Time.deltaTime;

        if (usePowerUpTimer >= randomNum1 && (powerUpSlot1 != null || powerUpSlot2 != null))
        {

            UsePowerUp();
            Debug.Log("Powerup function called");
            randomNum1 = Random.Range(1, 10);

            usePowerUpTimer = 0;
        }




        if (powerUpBoxObject != null && !powerUpBoxObject.activeInHierarchy)
        {
            powerUpBoxDelay += Time.deltaTime;

            if (powerUpBoxDelay >= 3)
            {
                powerUpBoxObject.SetActive(true);
                powerUpBoxDelay = 0;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
         if (other.gameObject.tag == "Coin")
         {
             coinCount++;

             Destroy(other.gameObject);
         }

        if (other.gameObject.tag == "PowerUpBox")
        {
            AddPowerUp(powerUpType[Random.Range(0, powerUpType.Length)]);

            powerUpBoxObject = other.gameObject;
            powerUpBoxObject.SetActive(false);
        }

        if (other.gameObject.tag == "weapon")
        {
            carBot.forwardAccelBuildUp = carBot.forwardAccelBuildUp / 2;
        }
    }

    public void AddPowerUp(string powerUp)
    {
        powerUpBoxSound.Play();

        Debug.Log("slot 1: " + powerUpSlot1);


        if (powerUpSlot1 == null)
        {
            powerUpSlot1 = powerUp; //Debug.Log("1");
            Debug.Log("powerup slot 1: " + powerUpSlot1);
        }
        else
        {
            powerUpSlot2 = powerUp; //Debug.Log("2");

            Debug.Log("powerup slot 2: " + powerUpSlot2);
        }


        //Debug.Log(powerUpSlot1);
    }

    void UsePowerUp()
    {

        if (currentPowerUpSlot == 1)
        {
           
            Debug.Log(powerUpSlot1 + " working");

            switch (powerUpSlot1)
            {
                case "boost":

                    carBot.isBoosted = true;

                    powerUpSlot1 = null;
                    break;

                case "dart":

                    Instantiate(dartObject, powerUpInstantiatePoint.position, powerUpInstantiatePoint.rotation);

                    powerUpSlot1 = null;
                    break;


                
            }
            //powerUpSlot1 = null;
        }
        else
        {
            //powerUpSlot2 = null;
        }
    }

    void SwapPowerUp()
    {
        if (currentPowerUpSlot == 1)
        {
            currentPowerUpSlot = 2;
        }
        else
        {
            currentPowerUpSlot = 1;
        }
    }


}
