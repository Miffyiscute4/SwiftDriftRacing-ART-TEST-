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
    List<string> powerUpSlot = new List<string> {"",""};
    internal int currentPowerUpSlot;
    string[] powerUpType = {"Boost","Dart","InvincibilityOrb","Bomb","Magnet","Rocket","IceSpikes"};
    string[] specialPowerUpType = { "Boost","InvincibilityOrb"};

    [Header("Audio")]

    //boost sound is already in the car controllers
    public AudioSource sound_Dart; public AudioSource sound_Invincible, sound_Bomb, sound_Magnet, sound_Rocket, sound_Block, sound_CheckPoint, sound_Hit; 

    //stopwatches
    internal float powerUpBoxDelay, coinTimer;
    internal float lavaTimer = 0, PowerUpRegenTimer = 0, invincibleTimer = 0;

    float stopwatch_Random, randomDelayTime;

    [Header("Collider")]
    public SphereCollider sc;
    public float originalTriggerRadius = 0.5f;
    public float increasedTriggerRadius = 1.25f;

    [Header("Checkpoints")]
    public GameObject allCheckPointsObject;
    Transform lastCheckPoint;
    public int lastCheckPointNumber = 0;
    Transform[] allCheckPoints;

    [Header("Particles")]
    public ParticleSystem coinParticle1;
    public ParticleSystem coinParticle2;
    public ParticleSystem magneticParticle;
    public ParticleSystem invincibleParticle;

    [Header("Materials")]
    public Material defaultMaterial;
    public Material invincibleMaterial;

    public GameObject carBody;

    [Header("UI")]
    public UI ui;



    public Animator checkPointText;

    [HideInInspector] public int lapCount;




    [Header("Powerup Objects")] public GameObject dartObject; public GameObject bombObject; public GameObject rocketObject; public GameObject iceSpikesObject;

    [Header("PowerUp Booleans")] public bool isBoosted; public bool isShootingDart; public bool isInvincible; public bool isShootingBomb; public bool isMagnetic; public bool isShootingRocket; public bool isShootingIceSpikes;

    



    // Update is called once per frame

    void Start()
    {
        carBody.GetComponent<MeshRenderer>().material = defaultMaterial; 

        currentPowerUpSlot = 1;

        allCheckPoints = allCheckPointsObject.GetComponentsInChildren<Transform>();

        //last checkpoint = first checkpoint
        lastCheckPointNumber = 0;

        lastCheckPoint = allCheckPoints[lastCheckPointNumber];

        currentPowerUpSlot = 0;

        powerUpSlot[0] = null;
        powerUpSlot[1] = null;


        randomDelayTime = Random.Range(1, 5);
    }

    void Update()
    {
        //Debug.Log(allCheckPoints[1].name);
        //magneticParticle.Stop();

        //Debug.Log(currentPowerUpSlot);

        lavaTimer += Time.deltaTime;
        PowerUpRegenTimer += Time.deltaTime;

        BoolActions();

        if (Input.GetKeyDown(KeyCode.E) && !isBot || isBot && powerUpSlot[currentPowerUpSlot] != null)
        {
            UsePowerUp();
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            SwapPowerUp();
            Debug.Log("PRESSED");
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

        //Debug.Log(lastCheckPointNumber);

        //Debug.Log(coinCount);


        //turns the car model sideways depending on the input axis: horizontal

        /*if (carBody.transform.rotation.eulerAngles.x > -25)
        {
            carBody.transform.rotation.eulerAngles += Quaternion.Euler(1, 0, 0);
        }
        else if (carBody.transform.rotation.eulerAngles.x < -25)
        {
            carBody.transform.rotation.eulerAngles += Quaternion.Euler(1, 0, 0);
        }*/

        
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
                Instantiate(coinParticle1, other.transform.position, other.transform.rotation);
                coinCount++;
            }
        }

        if (other.gameObject.tag == "Big_Coin")
        {
           
            if (!isMagnetic)
            {
                Destroy(other.gameObject);
                coin2.Play();
                Instantiate(coinParticle2, other.transform.position, other.transform.rotation);
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
            if (!carPlayer.isBoosted || carPlayer.driftBoostStage < 2)
            {
                isBoosted = true;
                carPlayer.driftBoostStage = 2;
            }
            
        }

        if (other.gameObject.tag == "PowerUpBox" && (powerUpSlot[0] == null || powerUpSlot[1] == null))
        {
            AddPowerUp(powerUpType[Random.Range(0, powerUpType.Length)]);

            powerUpBoxObject = other.gameObject;
            powerUpBoxObject.SetActive(false);
        }

        if (other.gameObject.tag == "PowerUpBox_Special" && (powerUpSlot[0] == null || powerUpSlot[1] == null))
        {
            AddPowerUp(specialPowerUpType[Random.Range(0, specialPowerUpType.Length)]);

            powerUpBoxObject = other.gameObject;
            powerUpBoxObject.SetActive(false);
        }


        if (other.gameObject.tag == "weapon")
        {
            sound_Hit.Play();

            if (!isInvincible)
            {
                switch (other.gameObject.name)
                {
                    case "Dart(Clone)":

                        if (isBot)
                        {
                            carBot.currentSpeed /= 2;
                        }
                        else
                        {
                            carPlayer.currentSpeed /= 2;
                        }

                        break;

                    case "Explosion(Clone)":
                        if (isBot)
                        {
                            carBot.currentSpeed = 0;
                        }   
                        else
                        {
                            carPlayer.currentSpeed = 0;
                        }    
                        
                        break;

                    case "IceSpikes(Clone)":
                        //carPlayer.currentSpeed = 0;
                        break;

                }

                
            }


        }

        if (other.gameObject.tag == "CheckPoint")
        {
            if (other.gameObject.transform == allCheckPoints[1] && other.gameObject.transform == allCheckPoints[lastCheckPointNumber])
            {
                Debug.Log("start");

                //checkPointText.Play("CheckPoint_UI");
                if (!isBot)
                {
                    sound_CheckPoint.Play();

                    checkPointText.SetBool("isTouchingInitialCheckPoint", true);

                    /*if(checkPointText.GetCurrentAnimatorStateInfo(0).IsName("CheckPoint_UI_Idle"))
                    {

                    }*/

                    lapCount++;

                    StartCoroutine("StopAnimation");
                }
                

                
                
            }

            if (lastCheckPointNumber + 1 > allCheckPoints.Length)
            {
                lastCheckPointNumber = 0;
                lastCheckPoint = allCheckPoints[lastCheckPointNumber];
                Debug.Log("checkpoint number reset");
            }

            //if the collider is with the correct game object
            if (other.gameObject.transform == allCheckPoints[lastCheckPointNumber])
            {
                //LastCheckPoint.position = other.gameObject.transform.position;
                //LastCheckPoint.rotation = other.gameObject.transform.rotation;
                if (lastCheckPointNumber < allCheckPoints.Length)
                {
                    lastCheckPointNumber++;
                    Debug.Log("checkpoint + 1");
                    lastCheckPoint = allCheckPoints[lastCheckPointNumber -1];
                }

                


            }
            else if (other.gameObject.transform != allCheckPoints[lastCheckPointNumber])
            {
                Debug.Log("not touching correct checkpoint");

                transform.position = lastCheckPoint.position;
                /*if (isBot)
                {
                    carBot.gameObject.transform.rotation = Quaternion.Euler(lastCheckPoint.rotation.eulerAngles.x, lastCheckPoint.rotation.eulerAngles.y + 270, lastCheckPoint.rotation.eulerAngles.z);
                }
                else
                {
                    carPlayer.gameObject.transform.rotation = Quaternion.Euler(lastCheckPoint.rotation.eulerAngles.x, lastCheckPoint.rotation.eulerAngles.y + 270, lastCheckPoint.rotation.eulerAngles.z);
                }*/
                

                //transform.rotation = lastCheckPoint.rotation;

                GetComponent<Rigidbody>().velocity = Vector3.zero;

            }


        }

        if (other.gameObject.tag == "DestroyZone")
        {
            transform.position = lastCheckPoint.position;

            if (isBot)
            {
                carBot.gameObject.transform.rotation = Quaternion.Euler(lastCheckPoint.rotation.eulerAngles.x, lastCheckPoint.rotation.eulerAngles.y + 270, lastCheckPoint.rotation.eulerAngles.z);
            }
            else
            {
                carPlayer.gameObject.transform.rotation = Quaternion.Euler(lastCheckPoint.rotation.eulerAngles.x, lastCheckPoint.rotation.eulerAngles.y + 270, lastCheckPoint.rotation.eulerAngles.z);
            }

            if (isBot)
            {
                carBot.transform.rotation = lastCheckPoint.rotation;
            }
            else
            {
                //carPlayer.gameObject.transform.rotation = lastCheckPoint.rotation;

                carPlayer.gameObject.transform.rotation = Quaternion.Euler(lastCheckPoint.rotation.eulerAngles.x -270, lastCheckPoint.rotation.eulerAngles.y, lastCheckPoint.rotation.eulerAngles.z);

            }


            GetComponent<Rigidbody>().velocity = Vector3.zero;

            //carPlayer.isDrifting = false;
            //carPlayer.isBoosted = false;
            //carPlayer.readyToBoost = false;
            //isBoosted = false;
            
            Debug.Log("DestroyZone");
        }

        Debug.Log("checkpoint reached");

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




            if (powerUpSlot[0] == null)
            {
                powerUpSlot[0] = powerUp; //Debug.Log("1");
                
                if (!isBot)
                {
                    ui.DisplayPowerUp(powerUpSlot[0], 0);
                }
                
            }
            else if (powerUpSlot[0] != null && powerUpSlot[1] == null)
            {
                powerUpSlot[1] = powerUp; //Debug.Log("2");
                
                if (!isBot)
                {
                    ui.DisplayPowerUp(powerUpSlot[1], 1);
                }
            }

            if (!isBot)
            {
            }


        Debug.Log(powerUpSlot[0] + "___" + powerUpSlot[1]);
    }

    public void UsePowerUp()
    {
        Debug.Log(stopwatch_Random);
        if (isBot)
        {
            //int randomDelayTime = Random.Range(1, 5);

            stopwatch_Random += Time.deltaTime;

            if (stopwatch_Random >= randomDelayTime)
            {
                switch (powerUpSlot[currentPowerUpSlot])
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

                    case "IceSpikes":

                        isShootingIceSpikes = true;
                        break;



                }

                
                stopwatch_Random = 0;

                randomDelayTime = Random.Range(1, 5);
                

                powerUpSlot[currentPowerUpSlot] = null;
            }
        }
        if (!isBot)
        {
            switch (powerUpSlot[currentPowerUpSlot])
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

                case "IceSpikes":

                    isShootingIceSpikes = true;
                    break;



            }

            powerUpSlot[currentPowerUpSlot] = null;
        }

            
        if (!isBot)
        {
            ui.DestroyPreviousObject(currentPowerUpSlot);
        }
             

            
    }

    public void SwapPowerUp()
    {

        ui.SwapPowerUps();

        if (currentPowerUpSlot == 0)
        {
            currentPowerUpSlot = 1;
            
        }
        else
        {
            currentPowerUpSlot = 0;
        }


    }

    public void BoolActions()
    {
        //boost
        if (isBoosted)
        {
            if (isBot)
            {
                carBot.isBoosted = true;
            }
            else
            {
                carPlayer.isBoosted = true;
            }

            /*if (!sound_Boost.isPlaying)
            {
                sound_Boost.Play();
            }*/

        }

        //dart
        if (isShootingDart)
        {
            Instantiate(dartObject, powerUpInstantiatePoint.position, powerUpInstantiatePoint.rotation);

            if (!sound_Dart.isPlaying)
            {
                sound_Dart.Play();
            }

            isShootingDart = false;
        }

        //invinicbility orb
        if (isInvincible)
        {
            invincibleTimer += Time.deltaTime;

            carBody.GetComponent<MeshRenderer>().material = invincibleMaterial;
            invincibleParticle.Play();

            if (invincibleTimer >= 8)
            {
                invincibleTimer = 0;

                carBody.GetComponent<MeshRenderer>().material = defaultMaterial;
                invincibleParticle.Stop();

                isInvincible = false;
            }

            if (!sound_Invincible.isPlaying)
            {
                sound_Invincible.Play();
            }

        }
        else
        {
            sound_Invincible.Stop();
        }

        //bomb
        if (isShootingBomb)
        {
            Instantiate(bombObject, powerUpInstantiatePoint.position, powerUpInstantiatePoint.rotation);

            if (!sound_Bomb.isPlaying)
            {
                sound_Bomb.Play();
            }

            isShootingBomb = false;
        }

        //magnet
        if (isMagnetic)
        {
            invincibleTimer += Time.deltaTime;

            magneticParticle.Play();

            sc.radius = increasedTriggerRadius;

            if (!sound_Magnet.isPlaying)
            {
                sound_Magnet.Play();
            }

            if (invincibleTimer >= 8)
            {
                sc.radius = originalTriggerRadius;

                magneticParticle.Stop();

                invincibleTimer = 0;

                isMagnetic = false;
            }

            
        }

        //rocket
        if (isShootingRocket)
        {
            Instantiate(rocketObject, powerUpInstantiatePoint.transform.position, powerUpInstantiatePoint.transform.rotation);

            if (!sound_Rocket.isPlaying)
            {
                sound_Rocket.Play();
            }

            isShootingRocket = false;
        }

        if (isShootingIceSpikes)
        {
            Instantiate(iceSpikesObject, coinInstantiatePoint.transform.position, coinInstantiatePoint.transform.rotation);
            
            if (!sound_Block.isPlaying)
            {
                sound_Block.Play();
            }
            

            isShootingIceSpikes = false;
        }
    }

    IEnumerator StopAnimation()
    {
        yield return new WaitForSeconds(3);
        checkPointText.SetBool("isTouchingInitialCheckPoint", false);

        Debug.Log("animation stopped");
    }
}
