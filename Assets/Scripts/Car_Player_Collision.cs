using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car_Player_Collision : MonoBehaviour
{

    public AudioSource coin1, coin2, coin1Drop, coin2Drop, powerUpBoxSound;
    public GameManager gameManager;
    public Car_Player carPlayer;
    public GameObject coinObject, bigCoinObject, powerUpBoxObject;
    public Transform coinInstantiatePoint;

    private float lavaTimer = 0, PowerUpRegenTimer = 0, invincibleTimer = 0;
    public int coinCount;


    string powerUpSlot1, powerUpSlot2;
    int currentPowerUpSlot;
    public Transform powerUpInstantiatePoint, carDirection;

    float powerUpBoxDelay, coinTimer;

    public string[] powerUpType;

    [Header("Powerup Objects")] public GameObject dartObject; public GameObject bombObject;

    [Header("PowerUp Booleans")] public bool isBoosted; public bool isShootingDart; public bool isInvincible; public bool isShootingBomb; public bool isMagnetic;



    // Update is called once per frame

    void Start()
    {
        currentPowerUpSlot = 1;
    }

    void Update()
    {
        lavaTimer += Time.deltaTime;
        PowerUpRegenTimer += Time.deltaTime;

        BoolActions();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            UsePowerUp();
        }

        if (Input.GetKeyDown(KeyCode.LeftAlt))
        {
            SwapPowerUp();
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

        //Debug.Log(coinCount);
    }





    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Coin")
        {
            if (isMagnetic)
            {
                other.gameObject.transform.position += transform.position;
            }
            else if (!isMagnetic)
            {
                Destroy(other.gameObject);
                coin1.Play();
                coinCount++;
            }
        }

        if (other.gameObject.tag == "Big_Coin")
        {
           
            if (!isMagnetic)
            {
                Destroy(other.gameObject);
                coin2.Play();
                coinCount += 5;
            }
            
        }

        if (other.gameObject.tag == "Ice")
        {
            carPlayer.turnStrength *= 2;
        }

        if (other.gameObject.tag == "OffTrack")
        {
            carPlayer.isOffTrack = true;
        }

        if (other.gameObject.tag == "BoostPad")
        {
            carPlayer.isBoosted = true;
        }

        if (other.gameObject.tag == "PowerUpBox")
        {
            AddPowerUp(powerUpType[Random.Range(0, powerUpType.Length)]);

            powerUpBoxObject = other.gameObject;
            powerUpBoxObject.SetActive(false);
        }

        if (other.gameObject.tag == "weapon")
        {
            if (!isInvincible)
            {
                switch (other.gameObject.name)
                {
                    case "Dart":
                        carPlayer.forwardAccelBuildUp = carPlayer.forwardAccelBuildUp / 2;
                        break;

                    case "Explosion(Clone)":
                        carPlayer.forwardAccelBuildUp = 0;
                        break;

                }

                
            }

        }

    }





    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Lava" && lavaTimer >= 2 && coinCount > 0)
        {
            //Quaternion carDirection = Quaternion.Euler(carController.transform.localRotation.x, carController.transform.localRotation.y + 90, carController.transform.localRotation.z);

            /* if (gameManager.totalcoins >= 5)
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
             }*/

         }


        if (isMagnetic && (other.gameObject.tag == "Coin" || other.gameObject.tag == "Big_Coin"))
        {
            other.gameObject.transform.position = Vector3.MoveTowards(other.gameObject.transform.position, transform.position, Time.deltaTime * 10);

            coinTimer += Time.deltaTime;

            if (coinTimer >= 0.5)
            {
                Destroy(other.gameObject);
                coin2.Play();

                if ((other.gameObject.tag == "Coin"))
                {
                    coinCount++;
                }
                else
                {
                    coinCount += 5;
                }
            }

            //other.gameObject.transform.LookAt(transform.position);
            //other.gameObject.transform.position = other.gameObject.transform.forward * 0.1f;
        }

        






    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Ice")
        {
            carPlayer.turnStrength /= 2;
        }

        if (other.gameObject.tag == "OffTrack")
        {
            carPlayer.isOffTrack = false;
        }

    }

    //--------------------------------------------------------------------------------------------------------------

    public void AddPowerUp(string powerUp)
    {
        powerUpBoxSound.Play();

        Debug.Log("slot 1: "+powerUpSlot1);


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
           
            //Debug.Log(powerUpSlot1);

            switch (powerUpSlot1)
            {
                case "boost":
                    isBoosted = true;

                    powerUpSlot1 = null;
                    break;

                case "dart":
                    isShootingDart = true;
                    powerUpSlot1 = null;
                    break;

                case "invincibilityOrb":

                    isInvincible = true;

                    powerUpSlot1 = null;
                    break;

                case "Bomb":

                    isShootingBomb = true;

                    powerUpSlot1 = null;
                    break;

                case "Magnet":

                    isMagnetic = true;

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

    public void BoolActions()
    {
        if (isBoosted)
        {
            carPlayer.isBoosted = true;

            isBoosted = false;
        }

        if (isShootingDart)
        {
            Instantiate(dartObject, powerUpInstantiatePoint.position, powerUpInstantiatePoint.rotation);

            isShootingDart = false;
        }

        if (isInvincible)
        {
            invincibleTimer += Time.deltaTime;

            if (invincibleTimer >= 8)
            {
                invincibleTimer = 0;

                isInvincible = false;
            }
            
        }

        if (isShootingBomb)
        {
            Instantiate(bombObject, powerUpInstantiatePoint.position, powerUpInstantiatePoint.rotation);

            isShootingBomb = false;
        }

        if (isMagnetic)
        {
            invincibleTimer += Time.deltaTime;

            if (invincibleTimer >= 999999999999)
            {
                invincibleTimer = 0;

                isMagnetic = false;
            }

            
        }

    }
}
