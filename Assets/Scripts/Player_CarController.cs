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
    public float speedMultiplier = 10; public float originalMaxSpeed = 20;internal float maxSpeed; public float verticalDelayTime = 0.2f; public float turnStrength = 7.5f; public float driftMultiplier = 1.25f; public float boostAmount;

     internal float speedInput, driftInput;
    internal float currentSpeed;

    //groundcheck
    [Header("Ground Check")]
    public LayerMask whatIsGround = 8;
    public float groundRayLength = 3;

    internal bool isGrounded; 

    //counters
    internal float stopWatch_VerticalBuildUp; internal float stopWatch_Boost; internal float stopWatch_Drift;

    internal bool isBoosted;
    internal bool isOffTrack;
    internal bool readyToBoost;

    [Header("Wheels")]
    public Transform leftFrontWheel;
    public Transform rightFrontWheel;

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

        speedInput = currentSpeed * speedMultiplier * Mathf.Abs(Input.GetAxisRaw("Vertical"));

        if (carCol.coinCount > carCol.maxCoinCount)
        {
            
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

        //changes the speed value on input
        if (Input.GetAxisRaw("Vertical") == 1 && currentSpeed < maxSpeed && stopWatch_VerticalBuildUp >= verticalDelayTime)
        {
            currentSpeed++;
            stopWatch_VerticalBuildUp = 0;
        }
        else if (Input.GetAxisRaw("Vertical") == -1 && currentSpeed > -maxSpeed / 2 && stopWatch_VerticalBuildUp >= verticalDelayTime)
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



    void TurnInput()
    {
        //when the key is pressed
        if (Input.GetKeyDown(KeyCode.LeftShift) && Input.GetAxisRaw("Vertical") != 0 && Input.GetAxisRaw("Horizontal") != 0)
        {
            driftInput = Input.GetAxisRaw("Horizontal");

            stopWatch_Drift = 0;
        }//when the key is held
        else if (Input.GetKey(KeyCode.LeftShift) && Input.GetAxisRaw("Vertical") != 0 && Input.GetAxisRaw("Horizontal") != 0)
        {
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0f, driftMultiplier * driftInput * turnStrength * Time.deltaTime * Input.GetAxis("Vertical"), 0f));

            stopWatch_Drift += Time.deltaTime;

            if (stopWatch_Drift >= 99)
            {
                stopWatch_Drift = 0;

                readyToBoost = true;
            }
        }
        else if (Input.GetAxisRaw("Horizontal") != 0)
        {
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0f, Input.GetAxis("Horizontal") * turnStrength * Time.deltaTime * Input.GetAxis("Vertical"), 0f));

            if (readyToBoost)
            {
                isBoosted = true;
                readyToBoost = false;
            }
        }
        else
        {
            if (readyToBoost)
            {
                isBoosted = true;
                readyToBoost = false;
            }
        }
        

        leftFrontWheel.localRotation = Quaternion.Euler(leftFrontWheel.localRotation.eulerAngles.x, Input.GetAxis("Horizontal") * turnStrength * currentSpeed - 180, leftFrontWheel.localRotation.eulerAngles.z);
        rightFrontWheel.localRotation = Quaternion.Euler(rightFrontWheel.localRotation.eulerAngles.x, Input.GetAxis("Horizontal") * turnStrength * currentSpeed, rightFrontWheel.localRotation.eulerAngles.z);

    }




    public void GroundCheck()
    {
        RaycastHit hit;

        if (Physics.Raycast(groundRayPoint.position, -transform.up, out hit, groundRayLength, whatIsGround))
        {
            isGrounded = true;

            Quaternion smoothtransition = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;

            transform.rotation = Quaternion.Lerp(transform.rotation, smoothtransition, Time.deltaTime * 10);

            
        }
        else
        {
            isGrounded = false;
        }
    }



    public void ApplyForce()
    {
        Debug.Log(isGrounded);



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
                rb.AddForce(transform.forward * boostAmount * 10000);

                if (stopWatch_Boost >= 3)
                {
                    
                    stopWatch_Boost = 0;

                    carCol.isBoosted = false;
                    isBoosted = false;
                }
            }

            rb.AddForce(-transform.up * 500);
        }
        else
        {
            rb.drag = 0.1f;

            rb.AddForce(-transform.up * 5000);
        }
    }
}
