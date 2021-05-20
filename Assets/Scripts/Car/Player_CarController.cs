using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.Rendering.PostProcessing;

public class Player_CarController : MonoBehaviour
{
    //objects & components
    [Header("Objects & Components")]
    public Rigidbody rb;
    public Transform groundRayPoint;
    public CarCollision carCol;
    //public GameObject carFrame;
    public GameObject carModel;

    public CinemachineVirtualCamera vc;


    //car variables
    [Header("Car Variables")]
    public float speedMultiplier = 10; public float originalMaxSpeed = 20;internal float maxSpeed; public float verticalDelayTime = 0.2f; public float turnStrength = 7.5f; internal float driftMultiplier = 1f; public float boostAmount;

    internal float speedInput, driftInput;
    internal float currentSpeed;
    internal bool isDrifting;
    internal float driftStrength;

    //groundcheck
    [Header("Ground Check")]
    public LayerMask whatIsGround = 8;
    public float groundRayLength = 3;
    public float snapSpeed;

    internal bool isGrounded; 

    //counters
    internal float stopWatch_VerticalBuildUp; internal float stopWatch_Boost; internal float stopWatch_Drift; internal float stopwatch_trails; internal float stopwatch_DriftMove; internal float stopwatch_StopDrift; internal float stopwatch_CancelBoost, stopwatch_StartDelay, stopwatch_Respawn;

    internal bool isBoosted;
    internal bool isOffTrack;
    internal bool readyToBoost;

    [Header("Wheels")]
    
    //front
    public Transform leftFrontWheel;
    public Transform rightFrontWheel;

    //back
    public Transform leftBackWheel;
    public Transform rightBackWheel;

    [Header("Trails / Particles")]

    public ParticleSystem driftTransitionParticle;
    public ParticleSystem boostParticle;
    public ParticleSystem smokeParticles;

    public List <ParticleSystem> driftParticles;
    public List<Material> driftParticlesMaterials;

    public Transform driftPoint1;
    public Transform driftPoint2;


    public GameObject allParticles;

    internal float driftBoostStage = 0;

    float screenX;

    [Header("Audio")]
    public AudioSource sound_Accelerate;
    public AudioSource sound_Drift, sound_driftBoostStage, sound_Boost;

    [Header("post processing")]
    public PostProcessLayer postProcessingLayer1;

    [Header("other")]
    public float startDelay;

    [HideInInspector] public bool isStarting;

    public Animator cameraAnim;
    public UI ui;

    CheckpointPlace checkPoint;

    //public List<GameObject> trails;


    // Start is called before the first frame update
    void Start()
    {
        //rb = transform.parent.FindChild("CarSphere").GetComponent<Rigidbody>();

        for (int i = 0; i < driftParticles.Count; i++)
        {
            driftParticles[i].Stop();
        }

        smokeParticles.Stop();

        vc.enabled = false;
        ui.countDownText.enabled = false;
        ui.coinText.enabled = false;
        ui.timerText.enabled = false;
        ui.coinUi.SetActive(false);

        isStarting = true;

        checkPoint = carCol.GetComponent<CheckpointPlace>();
    }



    // Update is called once per frame
    void Update()
    {
     
        

        if (stopwatch_StartDelay >= startDelay)
        {
            if (isStarting)
            {
                ui.RaceCountDownGo();
            }
            else
            {
                ui.countDownText.enabled = false;

                ui.coinText.enabled = true;
                ui.timerText.enabled = true;
                ui.coinUi.SetActive(true);

                ui.startTimer = true;
            }



            if (isGrounded)
            {
                if (stopwatch_Respawn != 0)
                {
                    stopwatch_Respawn = 0;
                }

                VerticalInput();
                TurnInput();

                if (Input.GetKey(KeyCode.Q) && Input.GetKeyDown(KeyCode.Q))
                {
                    vc.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset.z *= -1;
                }
                else if (Input.GetKeyUp(KeyCode.Q))
                {
                    vc.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset = new Vector3(Mathf.Abs(vc.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset.x), Mathf.Abs(vc.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset.y), -Mathf.Abs(vc.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset.z));
                }

                screenX = vc.GetCinemachineComponent<CinemachineComposer>().m_ScreenX;

            }
            else
            {
                stopwatch_Respawn += Time.deltaTime;

                if (stopwatch_Respawn >= 5)
                {
                    checkPoint.RespawnPlayer();
                    stopwatch_Respawn = 0;
                }
            }

            //follows the car at all times
            transform.position = rb.transform.position;
        }
        else
        {

            if (!cameraAnim.GetCurrentAnimatorStateInfo(0).IsName("Camera_TrackPreview") || !cameraAnim.GetCurrentAnimatorStateInfo(0).IsName("Camera_TrackPreview_Tutorial"))
            {
                stopwatch_StartDelay += Time.deltaTime;

                cameraAnim.enabled = false;
                vc.enabled = true;

                if (stopwatch_StartDelay >= 2)
                {
                    ui.countDownText.enabled = true;
                    ui.RaceCountDown();
                }
                
            }
            


        }
        

    }


    void FixedUpdate()
    {
        GroundCheck();
        ApplyForce();
    }



    void VerticalInput()
    {
        maxSpeed = originalMaxSpeed + carCol.coinCount;

        stopWatch_VerticalBuildUp += Time.deltaTime;

        if (!isDrifting)
        {
            speedInput = currentSpeed * speedMultiplier * Mathf.Abs(Input.GetAxisRaw("Vertical"));
        }
        else
        {
            speedInput = currentSpeed * speedMultiplier * 1;
        }        
        



        if (maxSpeed > originalMaxSpeed + carCol.maxCoinCount)
        {
            maxSpeed = originalMaxSpeed + carCol.maxCoinCount;
        }

        if (currentSpeed > maxSpeed)
        {
            currentSpeed = maxSpeed;
        }
        else if (currentSpeed < -maxSpeed)
        {
            currentSpeed = -maxSpeed;
        }


        if (!isDrifting)
        {

            if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.S))
            {
                smokeParticles.Play();
            }
            else
            {
                smokeParticles.Stop();
            }



            //changes the speed value on input
            if (Input.GetAxisRaw("Vertical") == 1 && currentSpeed < maxSpeed)
            {

                if (!sound_Accelerate.isPlaying && !sound_Drift.isPlaying)
                {
                    sound_Accelerate.Play();
                }

                if (stopWatch_VerticalBuildUp >= verticalDelayTime)
                {
                    currentSpeed++;
                }

                if (vc.m_Lens.FieldOfView < 40)
                {
                    vc.m_Lens.FieldOfView += 0.15f;
                }


                

            }
            else if (Input.GetAxisRaw("Vertical") == -1 && currentSpeed > -maxSpeed)
            {

                if (!sound_Accelerate.isPlaying && !sound_Drift.isPlaying)
                {
                    sound_Accelerate.Play();
                }

                if (stopWatch_VerticalBuildUp >= verticalDelayTime)
                {
                    currentSpeed--;

                    if (currentSpeed < 0)
                    {
                        stopWatch_VerticalBuildUp = 0;
                    }

                } 
                
                if (vc.m_Lens.FieldOfView > 35)
                {
                    vc.m_Lens.FieldOfView -= 0.15f;
                }

                
            }
            else if (Input.GetAxisRaw("Vertical") == 0)
            {
                sound_Accelerate.Stop();

                if (vc.m_Lens.FieldOfView > 35)
                {
                    vc.m_Lens.FieldOfView -= 1f;
                }

                //changes speed until it is equal to 0
                if (currentSpeed < 0)
                {
                    currentSpeed++;
                }
                else if (currentSpeed > 0)
                {
                    currentSpeed--;
                }

                //stops the rigidbody from moving once it moves too slowly
                if (rb.velocity.magnitude < 5f)
                {
                    rb.velocity = new Vector3(0, 0, 0);
                    currentSpeed = 0;
                }

            }
        }
        
    }



    void TurnInput()
    {

        if (!isDrifting)
        {
            if (screenX < 0.5)
            {
                vc.GetCinemachineComponent<CinemachineComposer>().m_ScreenX += 0.01f;
            }
            else if (screenX > 0.5)
            {
                vc.GetCinemachineComponent<CinemachineComposer>().m_ScreenX -= 0.01f;
            }
        }

        //Debug.Log("current speed: " + currentSpeed);

        //when the key is pressed
        if (Input.GetKeyDown(KeyCode.Space) && Input.GetAxisRaw("Horizontal") != 0 && currentSpeed > 5 && currentSpeed <= maxSpeed)
        {

            driftStrength = 30;


            driftInput = Input.GetAxisRaw("Horizontal");

            stopWatch_Drift = 0;

            //location of the particle systems

            if (driftInput == 1)
            {
                allParticles.transform.position = driftPoint1.position;
                allParticles.transform.rotation = driftPoint1.rotation;
            }   
            else if (driftInput == -1)
            {
                allParticles.transform.position = driftPoint2.position;
                allParticles.transform.rotation = driftPoint2.rotation;
            }


            //-----------------------------------------------------------]
            
           
            

            if (driftBoostStage == 0)
            {
                if (carCol.coinCount >= 1)
                driftParticles[0].Play();
                //currentSpeed = 10;
                isDrifting = true;
            }
            


            
            
            //driftMultiplier = 3;
        }
        else
        {
            
        }
        
        if (!Input.GetKey(KeyCode.Space))
        {
            //smoothly returns back to original rotation once the space key is no longer pressed
            carModel.transform.rotation = Quaternion.Lerp(carModel.transform.rotation, transform.rotation, Time.deltaTime * 3);


            if (carModel.transform.localRotation.eulerAngles.y < 45 || carModel.transform.localRotation.eulerAngles.y > 315 && Input.GetAxisRaw("Vertical") != 0)
            {
                carModel.transform.localRotation = Quaternion.Lerp(carModel.transform.localRotation, Quaternion.Euler(0, Input.GetAxisRaw("Horizontal") * (turnStrength * 1.25f) * 3, 0), Time.deltaTime);
            }

            if (Input.GetAxisRaw("Vertical") != 0)
            {
                transform.Rotate(new Vector3(0, Input.GetAxisRaw("Horizontal") * Time.deltaTime * turnStrength * (carCol.coinCount / 20 + 1), 0));
            }
            
        }


        if (isDrifting)
        {
            

            //when the key is held
            if (Input.GetKey(KeyCode.Space))
            {
                
                

                if (!sound_Drift.isPlaying)
                {
                    sound_Drift.Play();
                }


                if (driftInput == 1 && screenX >= 0.325)
                {
                    vc.GetCinemachineComponent<CinemachineComposer>().m_ScreenX -= 0.01f;
                }
                else if (driftInput == -1 && screenX <= 0.625)
                {
                    vc.GetCinemachineComponent<CinemachineComposer>().m_ScreenX += 0.01f;
                }


                //Debug.Log(screenX);


                //transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0f, currentSpeed / 3 * driftInput * turnStrength * Time.deltaTime, 0f));
                /*if (carCol.coinCount != 0)
                {
                    transform.Rotate(new Vector3(0, driftInput * Time.deltaTime * turnStrength * driftStrength / 7.5f * carCol.coinCount / 7.5f, 0));
                }   
                else
                {
                    transform.Rotate(new Vector3(0, driftInput * Time.deltaTime * turnStrength * driftStrength / 7.5f, 0));
                }*/

                transform.Rotate(new Vector3(0, driftInput * Time.deltaTime * turnStrength * driftStrength / 7.5f * (carCol.coinCount / 20 + 1), 0));



                //rotate the car model to exadurate rotation
                if (carModel.transform.localRotation.eulerAngles.y < 45 || carModel.transform.localRotation.eulerAngles.y > 315)
                {
                    //carModel.transform.Rotate(new Vector3(0, driftInput * Time.deltaTime * turnStrength * driftStrength / 7.5f, 0));

                    //carModel.transform.localRotation = Quaternion.Lerp(carModel.transform.rotation, Quaternion.Euler(0, 45, 0), Time.deltaTime * 3);

                    carModel.transform.localRotation = Quaternion.Lerp(carModel.transform.localRotation, Quaternion.Euler(0, driftInput * (driftStrength * 1.25f), 0), Time.deltaTime * 5);

                    //Quaternion q = Quaternion.Euler(0,  driftInput * (driftStrength * 1.25f), 0);
                    //carModel.transform.localRotation = Quaternion.Lerp(carModel.transform.rotation, q, Time.deltaTime);
                }



                


                /*else if (carModel.transform.localRotation.eulerAngles.y > 45)
                {
                    carModel.transform.rotation = Quaternion.Euler(carModel.transform.rotation.eulerAngles.x, 45, carModel.transform.rotation.eulerAngles.z);
                }
                else if (carModel.transform.localRotation.eulerAngles.y < 315)
                {
                    carModel.transform.rotation = Quaternion.Euler(carModel.transform.rotation.eulerAngles.x, 315, carModel.transform.rotation.eulerAngles.z);
                }*/

                Debug.Log(carModel.transform.localRotation.eulerAngles.y);

                /*
                //trails
                for (int i = 0; i < trails.Count; i++)
                {
                    trails[i].SetActive(true);
                }*/
                if (carCol.coinCount >= 1)
                {
                    stopWatch_Drift += Time.deltaTime;

                    //Debug.Log("drift " + (int)stopWatch_Drift);

                    //drift boost

                    //Debug.Log(driftBoostStage);
                    if (stopWatch_Drift >= 45 / (currentSpeed * (carCol.coinCount / 20 + 1)) && driftBoostStage == 0)
                    {
                        sound_driftBoostStage.Play();

                        driftParticles[1].GetComponent<ParticleSystemRenderer>().material = driftParticlesMaterials[0];

                        //boostParticle.GetComponent<ParticleSystemRenderer>().material = driftParticlesMaterials[0];
                        //boostParticle.GetComponentInChildren<ParticleSystemRenderer>().material = driftParticlesMaterials[0];

                        driftParticles[0].Stop();

                        driftTransitionParticle.Play();
                        driftParticles[1].Play();

                        readyToBoost = true;



                        stopWatch_Drift = 0;
                        driftBoostStage = 1;

                        carCol.coinCount--;

                    }
                    else if (stopWatch_Drift >= 50 / (currentSpeed * (carCol.coinCount / 20 + 1)) && driftBoostStage == 1)
                    {

                        sound_driftBoostStage.Play();

                        driftParticles[1].GetComponent<ParticleSystemRenderer>().material = driftParticlesMaterials[1];

                        //boostParticle.GetComponent<ParticleSystemRenderer>().material = driftParticlesMaterials[1];
                        //boostParticle.GetComponentInChildren<ParticleSystemRenderer>().material = driftParticlesMaterials[1];

                        driftTransitionParticle.Play();
                        driftParticles[1].Play();

                        stopWatch_Drift = 0;
                        driftBoostStage = 2;



                        carCol.coinCount --;


                    }
                    else if (stopWatch_Drift >= 75 / (currentSpeed * (carCol.coinCount / 20 + 1)) && driftBoostStage == 2)
                    {

                        sound_driftBoostStage.Play();

                        driftParticles[1].GetComponent<ParticleSystemRenderer>().material = driftParticlesMaterials[2];

                        //boostParticle.GetComponent<ParticleSystemRenderer>().material = driftParticlesMaterials[2];
                        //boostParticle.GetComponentInChildren<ParticleSystemRenderer>().material = driftParticlesMaterials[2];

                        driftTransitionParticle.Play();
                        driftParticles[1].Play();

                        stopWatch_Drift = 0;
                        driftBoostStage = 3;

                        carCol.coinCount--;
                    }
                }
                

            }
            else
            {
                sound_Drift.Stop();

                stopwatch_StopDrift += Time.deltaTime;

                if (stopwatch_StopDrift >= 1)
                {

                    
                    stopwatch_StopDrift = 0;
                    isDrifting = false;

                    for (int i = 0; i < driftParticles.Count; i++)
                    {
                        driftParticles[i].Stop();
                    }


                }

            }

            stopwatch_DriftMove += Time.deltaTime;

            if (stopwatch_DriftMove > 0.05)
            {
                stopwatch_DriftMove = 0;


                if (Input.GetAxisRaw("Horizontal") == driftInput && driftStrength < 35)
                {
                    //currentSpeed += 0.5f;
                    driftStrength += 5;
                }
                else if (Input.GetAxisRaw("Horizontal") == -driftInput && driftStrength > 20)
                {
                    //currentSpeed -= 0.5f;
                    driftStrength -= 5;
                }
            }
            

        }
        else if (Input.GetAxisRaw("Horizontal") != 0)
        {
            //transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0f, Input.GetAxis("Horizontal") * turnStrength * Time.deltaTime * Input.GetAxis("Vertical") * 2.5f, 0f));
            if (Input.GetAxisRaw("Vertical") != 0)
            {

                if (currentSpeed > 0)
                {
                    transform.Rotate(new Vector3(0, Input.GetAxisRaw("Horizontal") * Time.deltaTime * turnStrength * 2.5f, 0));
                }
                else if (currentSpeed < 0)
                {
                    transform.Rotate(new Vector3(0, -Input.GetAxisRaw("Horizontal") * Time.deltaTime * turnStrength * 2.5f, 0));
                }
                
            }    
            
            

            if (readyToBoost)
            {
                isBoosted = true;
                readyToBoost = false;
            }


            /*stopwatch_trails += Time.deltaTime;

            if (trails[0].activeInHierarchy && stopwatch_trails >= 1)
            {
                stopwatch_trails = 0;

                for (int i = 0; i < trails.Count; i++)
                {
                    trails[i].SetActive(false);
                }
            }*/
        }
        else
        {
            if (readyToBoost)
            {
                isBoosted = true;
                readyToBoost = false;
            }


            /*stopwatch_trails += Time.deltaTime;

            if (trails[0].activeInHierarchy && stopwatch_trails >= 1)
            {
                stopwatch_trails = 0;

                for (int i = 0; i < trails.Count; i++)
                {
                    trails[i].SetActive(false);
                }
            }*/

        }

        //leftBackWheel.rotation = Quaternion.Euler(leftBackWheel.rotation.eulerAngles.x + currentSpeed * 5, leftBackWheel.rotation.eulerAngles.y, leftBackWheel.rotation.eulerAngles.z);
        //rightBackWheel.rotation = Quaternion.Euler(rightBackWheel.rotation.eulerAngles.x + currentSpeed * 5, rightBackWheel.rotation.eulerAngles.y, rightBackWheel.rotation.eulerAngles.z);

        //visual wheel turning
        if (Input.GetAxisRaw("Vertical") != 0)
        {
            //front wheel turn x
            rightFrontWheel.transform.Rotate(new Vector3(currentSpeed * 100, 0f, 0f) * Time.deltaTime);
            leftFrontWheel.transform.Rotate(new Vector3(currentSpeed * 100, 0f) * Time.deltaTime);


            //back wheel turn x
            rightBackWheel.transform.Rotate(new Vector3(currentSpeed * 100, 0f, 0f) * Time.deltaTime);
            leftBackWheel.transform.Rotate(new Vector3(currentSpeed * 100, 0f, 0f) * Time.deltaTime);
        }

        //Debug.Log(leftFrontWheel.localRotation.y + "___" + rightFrontWheel.localRotation.y);

        if (Input.GetAxisRaw("Horizontal") != 0 && Input.GetAxisRaw("Vertical") != 0 /*&& isDrifting && leftFrontWheel.localRotation.y >= 0 && rightFrontWheel.localRotation.y >= 0*/)
        {
           

            //front wheel turn y
            leftFrontWheel.localRotation = Quaternion.Lerp(leftFrontWheel.localRotation, Quaternion.Euler(0, Input.GetAxis("Horizontal") * (driftStrength + 35), 0), Time.deltaTime * 5);
            rightFrontWheel.localRotation = Quaternion.Lerp(rightFrontWheel.localRotation, Quaternion.Euler(0, Input.GetAxis("Horizontal") * (driftStrength + 35), 0), Time.deltaTime * 5);



        }
        else if (Input.GetAxisRaw("Horizontal") == 0)
        {
            leftFrontWheel.localRotation = Quaternion.Lerp(leftFrontWheel.localRotation, Quaternion.Euler(0, 0, 0), Time.deltaTime * 3);
            rightFrontWheel.localRotation = Quaternion.Lerp(rightFrontWheel.localRotation, Quaternion.Euler(0, 0, 0), Time.deltaTime * 3);
        }
    }




    public void GroundCheck()
    {
        RaycastHit hit;

        if (Physics.Raycast(groundRayPoint.position, -transform.up, out hit, groundRayLength, whatIsGround))
        {
            isGrounded = true;


            
        }
        else
        {
            isGrounded = false;
        }

        if (Physics.Raycast(groundRayPoint.position, -transform.up, out hit, 35, whatIsGround))
        {

            Quaternion smoothtransition = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;

            transform.rotation = Quaternion.Lerp(transform.rotation, smoothtransition, Time.deltaTime * snapSpeed);


        }
    }



    public void ApplyForce()
    {
        //Debug.Log(isGrounded);



        if (isGrounded)
        {
            rb.drag = 3;

            if (carCol.isBot)
            {
                rb.AddForce(transform.forward * speedInput * 10);
            }
            else// if (Mathf.Abs(speedInput) > 0)
            {
                //moves car
                rb.AddForce(transform.forward * speedInput * 10);


                
            }


            if (isBoosted)
            {
                postProcessingLayer1.enabled = true;

                if (driftBoostStage != 0)
                {
                    boostParticle.GetComponent<ParticleSystemRenderer>().material = driftParticlesMaterials[(int)driftBoostStage - 1];
                    boostParticle.gameObject.transform.GetChild(0).GetComponent<ParticleSystemRenderer>().material = driftParticlesMaterials[(int)driftBoostStage - 1];

                    rb.AddForce(transform.forward * boostAmount * 50 * driftBoostStage);
                }
                else
                {   
                    rb.AddForce(transform.forward * boostAmount * 50);
                }
                

                stopWatch_Boost += Time.deltaTime;

                if (!sound_Boost.isPlaying)
                {
                    sound_Boost.Play();
                }

               

                boostParticle.Play();

                if (stopWatch_Boost >= 2 || Input.GetKeyDown(KeyCode.Space))
                {
                    StopBoost(); 
                }

            }
            else
            {
                if (!carCol.isBot)
                {
                    postProcessingLayer1.enabled = false;
                }
                
            }

            //force to apply when grounded
            rb.AddForce(-transform.up * 250);
        }
        else
        {
            

            rb.drag = 0.05f;

            rb.AddForce(-transform.up * 1250);
        }
    }

    public void StopBoost()
    {
        stopWatch_Boost = 0;

        boostParticle.Stop();

        sound_Boost.Stop();

        driftBoostStage = 0;

        carCol.isBoosted = false;
        isBoosted = false;
    }
}
