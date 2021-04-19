using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_CarController : MonoBehaviour
{
    //objects & components
    [Header("Objects & Components")]
    public Rigidbody rb;
    public Transform groundRayPoint;
    public CarCollision carCol;


    //car variables
    [Header("Car Variables")]
    public float speedMultiplier = 10; public float originalMaxSpeed = 20;internal float maxSpeed; public float verticalDelayTime = 0.2f; public float turnStrength = 7.5f; internal float driftMultiplier = 1f; public float boostAmount;

    internal float speedInput, driftInput;
    internal float currentSpeed;
    internal bool isDrifting;

    //groundcheck
    [Header("Ground Check")]
    public LayerMask whatIsGround = 8;
    public float groundRayLength = 3;
    public float snapSpeed;

    internal bool isGrounded; 

    //counters
    internal float stopWatch_VerticalBuildUp; internal float stopWatch_Boost; internal float stopWatch_Drift; internal float stopwatch_trails; internal float stopwatch_DriftMove; internal float stopwatch_StopDrift;

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

    public Transform driftPoint1;
    public Transform driftPoint2;

    public GameObject allParticles;


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


    }



    // Update is called once per frame
    void Update()
    {
        if (isGrounded)
        {
            VerticalInput();
            TurnInput();
        }

        //follows the car at all times
        transform.position = rb.transform.position;

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
            if (Input.GetAxisRaw("Vertical") == 1 && currentSpeed < maxSpeed && stopWatch_VerticalBuildUp >= verticalDelayTime)
            {
                currentSpeed++;
                stopWatch_VerticalBuildUp = 0;
            }
            else if (Input.GetAxisRaw("Vertical") == -1 && currentSpeed > -maxSpeed && stopWatch_VerticalBuildUp >= verticalDelayTime)
            {
                currentSpeed--;
                stopWatch_VerticalBuildUp = 0;
            }
            else if (Input.GetAxisRaw("Vertical") == 0)
            {
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

        Debug.Log("current speed: " + currentSpeed);


        //when the key is pressed
        if (Input.GetKeyDown(KeyCode.Space) && Input.GetAxisRaw("Horizontal") != 0 && currentSpeed > 10 && currentSpeed <= maxSpeed)
        {
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

            

            //-----------------------------------------------------------

            isDrifting = true;
            currentSpeed = 10;
            if (!readyToBoost)
            {
                driftParticles[0].Play();
            }
            
            //driftMultiplier = 3;
        }
        
        if (isDrifting)
        {
            //when the key is held
            if (Input.GetKey(KeyCode.Space))
            {
                transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0f, currentSpeed / 3 * driftInput * turnStrength * Time.deltaTime, 0f));

                /*
                //trails
                for (int i = 0; i < trails.Count; i++)
                {
                    trails[i].SetActive(true);
                }*/

                stopWatch_Drift += Time.deltaTime;

                //Debug.Log("drift " + (int)stopWatch_Drift);

                //drift boost
                if (stopWatch_Drift >= 40 / currentSpeed && !readyToBoost)
                {
                    stopWatch_Drift = 0;

                    driftParticles[0].Stop();
                    driftTransitionParticle.Play();
                    driftParticles[1].Play();

                    readyToBoost = true;
                }
            }
            else
            {
                stopwatch_StopDrift += Time.deltaTime;

                if (stopwatch_StopDrift >= 0.5)
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


                if (Input.GetAxisRaw("Horizontal") == driftInput && currentSpeed < maxSpeed)
                {
                    currentSpeed += 1;
                }
                else if (Input.GetAxisRaw("Horizontal") == -driftInput && currentSpeed > 10)
                {
                    currentSpeed -= 1;
                }
            }
            

        }
        else if (Input.GetAxisRaw("Horizontal") != 0)
        {
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0f, Input.GetAxis("Horizontal") * turnStrength * Time.deltaTime * Input.GetAxis("Vertical") * 2.5f, 0f));

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
        

        leftFrontWheel.rotation = Quaternion.Euler(leftFrontWheel.rotation.eulerAngles.x - currentSpeed * 5, leftFrontWheel.rotation.y + 45 * Input.GetAxis("Horizontal"), leftFrontWheel.rotation.z);
        rightFrontWheel.rotation = Quaternion.Euler(rightFrontWheel.rotation.eulerAngles.x - currentSpeed * 5, rightFrontWheel.rotation.y + 45 * Input.GetAxis("Horizontal"), rightFrontWheel.rotation.z);

        leftBackWheel.rotation = Quaternion.Euler(leftBackWheel.rotation.eulerAngles.x + currentSpeed * 5, leftBackWheel.rotation.eulerAngles.y, leftBackWheel.rotation.eulerAngles.z);
        rightBackWheel.rotation = Quaternion.Euler(rightBackWheel.rotation.eulerAngles.x + currentSpeed * 5, rightBackWheel.rotation.eulerAngles.y, rightBackWheel.rotation.eulerAngles.z);

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
                stopWatch_Boost += Time.deltaTime;
                rb.AddForce(transform.forward * boostAmount * 50);

                boostParticle.Play();

                if (stopWatch_Boost >= 3)
                {
                    
                    stopWatch_Boost = 0;

                    boostParticle.Stop();

                    carCol.isBoosted = false;
                    isBoosted = false;
                }
            }

            //force to apply when grounded
            rb.AddForce(-transform.up * 250);
        }
        else
        {
            rb.drag = 0.05f;

            rb.AddForce(-transform.up * 500);
        }
    }
}
