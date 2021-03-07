using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarPickup : MonoBehaviour
{

    public AudioSource coin1,coin2,coin1Drop,coin2Drop, powerUpBoxSound;
    public GameManager gameManager;
    public CarController carController;
    public GameObject coinObject, bigCoinObject;
    public Transform coinInstantiatePoint;

    private int powerUpSlot1 = 0, powerUpSlot2 = 0;
    private float lavaTimer = 0, PowerUpRegenTimer = 0;
    private GameObject powerUpBox;

    private bool isCurrentPowerUpSlot1;
    // Update is called once per frame

    void Start()
    {
        isCurrentPowerUpSlot1 = true;
    }
    void Update()
    {
        lavaTimer += Time.deltaTime;
        PowerUpRegenTimer += Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            UsePowerUp();
        }

        if (Input.GetKeyDown(KeyCode.LeftAlt))
        {
            isCurrentPowerUpSlot1 = !isCurrentPowerUpSlot1;
        }

        if (isCurrentPowerUpSlot1)
        {
            gameManager.slotNumber = "1. PowerUp:" + powerUpSlot1;

            gameManager.reserveSlotNumber = "2. PowerUp:" + powerUpSlot2;
        }
        else
        {
            gameManager.slotNumber = "2. PowerUp:" + powerUpSlot2;

            gameManager.reserveSlotNumber = "1. PowerUp: " + powerUpSlot1;
        }

        if (PowerUpRegenTimer >= 5 && powerUpBox != null)
        {
            powerUpBox.SetActive(true);
            PowerUpRegenTimer += 0;
            powerUpBox = null;
        }
    }





    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Coin")
        {
            Destroy(other.gameObject);
            coin1.Play();
            gameManager.AddCoins(1);
        }

        if (other.gameObject.tag == "Big_Coin")
        {
            Destroy(other.gameObject);
            coin2.Play();
            gameManager.AddCoins(5);
        }

        if (other.gameObject.tag == "Ice")
        {
            carController.turnStrength *= 2;
        }

        if (other.gameObject.tag == "OffTrack")
        {
            carController.isOffTrack = true;
        }

        if (other.gameObject.tag == "BoostPad")
        {
            carController.isBoosted = true;
        }

        if (other.gameObject.tag == "PowerUpBox")
        {

            other.gameObject.SetActive(false);

            PowerUpRegenTimer = 0;

            powerUpBox = other.gameObject;

            powerUpBoxSound.Play();

            if (isCurrentPowerUpSlot1 && powerUpSlot1 == 0 || !isCurrentPowerUpSlot1 && powerUpSlot2 != 0)
            {
                powerUpSlot1 = Random.Range(1, 1);
            }
            else if (!isCurrentPowerUpSlot1 && powerUpSlot2 == 0 || isCurrentPowerUpSlot1 && powerUpSlot1 != 0)
            {
                powerUpSlot2 = Random.Range(1, 1);
            }
        }
    }





    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Lava" && lavaTimer >= 2 && gameManager.totalcoins > 0)
        {
            Quaternion carDirection = Quaternion.Euler(carController.transform.localRotation.x, carController.transform.localRotation.y + 90, carController.transform.localRotation.z);

            if (gameManager.totalcoins >= 5)
            {
                gameManager.SubtractCoins(5);
                Instantiate(coinObject, coinInstantiatePoint.position, carDirection);
                lavaTimer = 0;
                coin2Drop.Play();
            }
            else
            {
                gameManager.SubtractCoins(1);
                Instantiate(coinObject, coinInstantiatePoint.position, carDirection);
                lavaTimer = 0;
                coin1Drop.Play();
            }
            
        }
    }





    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Ice")
        {
            carController.turnStrength /= 2;
        }

        if (other.gameObject.tag == "OffTrack")
        {
            carController.isOffTrack = false;
        }

    }

    void UsePowerUp()
    {
        if (isCurrentPowerUpSlot1)
        {
            switch (powerUpSlot1)
            {
                case 1:

                    carController.isBoosted = true;

                    break;
            }

            powerUpSlot1 = 0;
        }
        else if (!isCurrentPowerUpSlot1)
        {
            switch (powerUpSlot2)
            {
                case 1:

                    carController.isBoosted = true;

                    break;
            }

            powerUpSlot2 = 0;
        }
        
    }


}

