using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarCollision : MonoBehaviour
{
    [Header("IsBot")]
    public bool isBot;


    [Header("Scripts")]
    public Player_CarController carPlayer;
    public Bot_CarController carBot;

    [Header("Objects")]
    public GameObject coinObject; public GameObject bigCoinObject; internal GameObject powerUpBoxObject;
    public Transform coinInstantiatePoint, powerUpInstantiatePoint;

    [Header("Audio")]
    public AudioSource coin1; public AudioSource coin2, coin1Drop, coin2Drop, powerUpBoxSound;

    [Header("Coins")]
    public int coinCount; public int maxCoinCount;

    //powerups
    string powerUpSlot1, powerUpSlot2;
    internal int currentPowerUpSlot;
    string[] powerUpType = {"Boost","Dart","InvincibilityOrb","Bomb","Magnet","Rocket"};

    //stopwatches
    internal float powerUpBoxDelay, coinTimer;
    internal float lavaTimer = 0, PowerUpRegenTimer = 0, invincibleTimer = 0;

    [Header("Collider")]
    public SphereCollider sc;
    public float originalTriggerRadius = 0.5f;
    public float increasedTriggerRadius = 1.25f;

    [Header("Checkpoints")]
    public GameObject allCheckPointsObject;
    Transform LastCheckPoint;
    int LastCheckPointNumber;
    Transform[] allCheckPoints;



    [Header("Powerup Objects")] public GameObject dartObject; public GameObject bombObject; public GameObject rocketObject;

    [Header("PowerUp Booleans")] public bool isBoosted; public bool isShootingDart; public bool isInvincible; public bool isShootingBomb; public bool isMagnetic; public bool isShootingRocket;

    

    

    // Update is called once per frame

    void Start()
    {
        currentPowerUpSlot = 1;

        allCheckPoints = allCheckPointsObject.GetComponentsInChildren<Transform>();

        LastCheckPointNumber = 0;

        LastCheckPoint = allCheckPoints[LastCheckPointNumber];
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





    public void OnTriggerEnter(Collider other)
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
            if (isBot)
            {
                carBot.turnStrength *= 2;
            }
            else
            {
                carPlayer.turnStrength *= 2;
            }    
        }

        if (other.gameObject.tag == "OffTrack")
        {
            if (isBot)
            {
                carBot.isOffTrack = true;
            }
            else
            {
                carPlayer.isOffTrack = true;
            }
        }

        if (other.gameObject.tag == "BoostPad")
        {
            isBoosted = true;
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
                        carPlayer.currentSpeed = carPlayer.currentSpeed / 2;
                        break;

                    case "Explosion(Clone)":
                        carPlayer.currentSpeed = 0;
                        break;

                }

                
            }

           

        }

        if (other.gameObject.transform == allCheckPoints[LastCheckPointNumber + 1])
        {
            if (other.gameObject.tag == "CheckPoint")
            {
                //LastCheckPoint.position = other.gameObject.transform.position;
                //LastCheckPoint.rotation = other.gameObject.transform.rotation;
                if (LastCheckPointNumber < allCheckPoints.Length)
                {
                    LastCheckPointNumber++;
                }
                else
                {
                    LastCheckPointNumber = 0;
                }

                LastCheckPoint = allCheckPoints[LastCheckPointNumber];

                Debug.Log("checkpoint" + LastCheckPointNumber);


            }
        }

        

        if (other.gameObject.tag == "DestroyZone")
        {
            transform.position = LastCheckPoint.position;
            transform.rotation = LastCheckPoint.rotation;

            Debug.Log("DestroyZone");
        }
    }





    public void OnTriggerStay(Collider other)
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

    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Ice")
        {
            if (isBot)
            {
                carBot.turnStrength /= 2;
            }
            else
            {
                carPlayer.turnStrength /= 2;
            }
        }

        if (other.gameObject.tag == "OffTrack")
        {
            if (isBot)
            {
                carBot.isOffTrack = false;
            }
            else
            {
                carPlayer.isOffTrack = false;
            }
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

    public void UsePowerUp()
    {

        if (currentPowerUpSlot == 1)
        {
           
            //Debug.Log(powerUpSlot1);

            switch (powerUpSlot1)
            {
                case "Boost":
                    isBoosted = true;
                    break;

                case "Dart":
                    isShootingDart = true;
                    break;

                case "InvincibilityOrb":

                    isInvincible = true;
                    break;

                case "Bomb":

                    isShootingBomb = true;
                    break;

                case "Magnet":

                    isMagnetic = true;
                    break;

                case "Rocket":

                    isShootingRocket = true;
                    break;



            }

            powerUpSlot1 = null;
        }
        else
        {
            //powerUpSlot2 = null;
        }
    }

    public void SwapPowerUp()
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
        //boost
        if (isBoosted)
        {
            if (isBot)
            {
                carBot.isBoosted = false;
            }
            else
            {
                carPlayer.isBoosted = false;
            }

            isBoosted = false;
        }

        //dart
        if (isShootingDart)
        {
            Instantiate(dartObject, powerUpInstantiatePoint.position, powerUpInstantiatePoint.rotation);

            isShootingDart = false;
        }

        //invinicbility orb
        if (isInvincible)
        {
            invincibleTimer += Time.deltaTime;

            if (invincibleTimer >= 8)
            {
                invincibleTimer = 0;

                isInvincible = false;
            }
            
        }

        //bomb
        if (isShootingBomb)
        {
            Instantiate(bombObject, powerUpInstantiatePoint.position, powerUpInstantiatePoint.rotation);

            isShootingBomb = false;
        }

        //magnet
        if (isMagnetic)
        {
            invincibleTimer += Time.deltaTime;

            sc.radius = increasedTriggerRadius;

            if (invincibleTimer >= 8)
            {
                sc.radius = originalTriggerRadius;
                invincibleTimer = 0;

                isMagnetic = false;
            }

            
        }

        //rocket
        if (isShootingRocket)
        {
            Instantiate(rocketObject, powerUpInstantiatePoint.transform.position, powerUpInstantiatePoint.transform.rotation);

            isShootingRocket = false;
        }

    }
}
