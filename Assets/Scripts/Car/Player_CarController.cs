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
    public float snapSpeed;

    internal bool isGrounded; 

    //counters
    internal float stopWatch_VerticalBuildUp; internal float stopWatch_Boost; internal float stopWatch_Drift; internal float stopwatch_trails;

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

            for (int i = 0; i < trails.Count; i++)
            {
                trails[i].SetActive(true);
            }
            

            //transform.rotation = Quaternion.Euler(0f,transform.rotation.eulerAngles.y + 1 * Time.deltaTime * turnStrength * driftInput * 10, transform.rotation.eulerAngles.z);

            //transform.Rotate(transform.right * driftInput);


            stopWatch_Drift += Time.deltaTime;

            if (stopWatch_Drift >= 99)
            {
                stopWatch_Drift = 0;

                readyToBoost = true;
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
            rb.drag = 0.1f;

            rb.AddForce(-transform.up * 5000);
        }
    }
}
