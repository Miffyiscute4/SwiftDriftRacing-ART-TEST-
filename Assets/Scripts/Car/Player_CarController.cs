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

    [Header("Trails")]

    public List<GameObject> trails;

    // Start is called before the first frame update
    void Start()
    {
        //rb = transform.parent.FindChild("CarSphere").GetComponent<Rigidbody>();
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

        Debug.Log(currentSpeed);


        //when the key is pressed
        if (Input.GetKeyDown(KeyCode.Space) && Input.GetAxisRaw("Horizontal") != 0 && currentSpeed > 10 && currentSpeed <= maxSpeed)
        {
            driftInput = Input.GetAxisRaw("Horizontal");

            stopWatch_Drift = 0;

            isDrifting = true;
            currentSpeed = 10;
            //driftMultiplier = 3;
        }
        
        if (isDrifting)
        {
            //when the key is held
            if (Input.GetKey(KeyCode.Space))
            {
                transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0f, currentSpeed / 3 * driftInput * turnStrength * Time.deltaTime, 0f));

                //trails
                for (int i = 0; i < trails.Count; i++)
                {
                    trails[i].SetActive(true);
                }

                stopWatch_Drift += Time.deltaTime;

                //drift boost
                if (stopWatch_Drift >= 80 / currentSpeed)
                {
                    stopWatch_Drift = 0;

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


            stopwatch_trails += Time.deltaTime;

            if (trails[0].activeInHierarchy && stopwatch_trails >= 1)
            {
                stopwatch_trails = 0;

                for (int i = 0; i < trails.Count; i++)
                {
                    trails[i].SetActive(false);
                }
            }
        }
        else
        {
            if (readyToBoost)
            {
                isBoosted = true;
                readyToBoost = false;
            }


            stopwatch_trails += Time.deltaTime;

            if (trails[0].activeInHierarchy && stopwatch_trails >= 1)
            {
                stopwatch_trails = 0;

                for (int i = 0; i < trails.Count; i++)
                {
                    trails[i].SetActive(false);
                }
            }

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
                rb.AddForce(transform.forward * speedInput * 100);
            }
            else// if (Mathf.Abs(speedInput) > 0)
            {
                //moves car
                rb.AddForce(transform.forward * speedInput * 100);


                
            }


            if (isBoosted)
            {
                stopWatch_Boost += Time.deltaTime;
                rb.AddForce(transform.forward * boostAmount * 6000);

                if (stopWatch_Boost >= 3)
                {
                    
                    stopWatch_Boost = 0;

                    carCol.isBoosted = false;
                    isBoosted = false;
                }
            }

            rb.AddForce(-transform.up * 1500);
        }
        else
        {
            rb.drag = 0.05f;

            rb.AddForce(-transform.up * 5000);
        }
    }
}
