using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    public Rigidbody rb;

    public float maxForwardAccel = 18, maxReverseAccel = 9, turnStrength = 30, gravityForce = 10, dragOnGround = 3, delayAmount = 0.2f;

    private float speedInput, turnInput, accelDelay, decelDelay, boostDelay, forwardAccelBuildUp, reverseAccelBuildUp, driftInput;

    private bool grounded;

    public bool isOffTrack = false, isBoosted = false;
    public LayerMask whatIsGround;
    public float groundRayLength = 5;
    public Transform groundRayPoint;

    public Transform leftFrontWheel, rightFrontWheel;
    public float maxWheelTurn = 25;

    public GameManager gameManager;

    
    // Start is called before the first frame update
    void Start()
    {
        rb.transform.parent = null;
    }

    // Update is called once per frame

    void Update()
    {
        VerticalInput();
        TurnInput();
    }

    void FixedUpdate()
    {
        GroundCheck();
        ApplyForce();
    }





    void VerticalInput()
    {
        Debug.Log(isBoosted);

        speedInput = 0f;

        accelDelay += Time.deltaTime;
        decelDelay += Time.deltaTime;

        if (!isOffTrack)
        {
            if (isBoosted)
            {
                boostDelay += Time.deltaTime;

                if (boostDelay >= 3)
                {

                    forwardAccelBuildUp = maxForwardAccel;

                    isBoosted = false;

                }
                else
                {
                    rb.AddForce(transform.forward * 25000);
                }

            }
            else
            {
                boostDelay = 0;

                if (gameManager.totalcoins >= 10)
                {
                    maxForwardAccel = 20;
                    maxReverseAccel = 10;
                }
                else if (gameManager.totalcoins <= 10)
                {
                    maxForwardAccel = gameManager.totalcoins + 10;
                    maxReverseAccel = gameManager.totalcoins + 2;
                }
            }


        }
        else
        {
            maxForwardAccel = 10;
            maxReverseAccel = 2;

            isBoosted = false;
        }

        if (!isBoosted)
        {
            if (Input.GetAxis("Vertical") > 0)
            {

                if (reverseAccelBuildUp <= 0)
                {
                    if (accelDelay >= delayAmount && forwardAccelBuildUp < maxForwardAccel)
                    {
                        forwardAccelBuildUp += 1;

                        accelDelay = 0;
                    }

                    speedInput = Input.GetAxis("Vertical") * forwardAccelBuildUp * 1000f;
                }
                else
                {
                    if (decelDelay >= delayAmount && reverseAccelBuildUp > 0)
                    {
                        reverseAccelBuildUp -= 1;
                    }
                }
            }
            else if (Input.GetAxis("Vertical") < 0)
            {

                if (forwardAccelBuildUp <= 0)
                {
                    if (accelDelay >= delayAmount && reverseAccelBuildUp < maxReverseAccel)
                    {
                        reverseAccelBuildUp += 1;

                        accelDelay = 0;
                    }

                    speedInput = Input.GetAxis("Vertical") * reverseAccelBuildUp * 1000f;
                }
                else
                {
                    if (decelDelay >= delayAmount && forwardAccelBuildUp > 0)
                    {
                        forwardAccelBuildUp -= 1;
                    }

                }

            }
        }

        else
        {
            if (decelDelay >= delayAmount && forwardAccelBuildUp > 0 || reverseAccelBuildUp > 0)
            {
                forwardAccelBuildUp -= 1;
                reverseAccelBuildUp -= 1;

                decelDelay = 0;

            }
        }

        if (forwardAccelBuildUp < 0)
        {
            forwardAccelBuildUp = 0;
        }
        else if (reverseAccelBuildUp < 0)
        {
            reverseAccelBuildUp = 0;
        }


    }





    void TurnInput()
    {


        turnInput = Input.GetAxis("Horizontal");

        if (grounded)
        {
            if (Input.GetKeyDown(KeyCode.LeftShift) && Input.GetAxis("Vertical") > 0)
            {
                driftInput = (int)Input.GetAxis("Horizontal");
            }
            else if (Input.GetKey(KeyCode.LeftShift) && Input.GetAxis("Vertical") > 0)
            {

                transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0f, driftInput * (forwardAccelBuildUp / 10) * (turnStrength * 2) * Time.deltaTime * Input.GetAxis("Vertical"), 0f));


            }
            else
            {

                if (forwardAccelBuildUp > 0)
                {
                    transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0f, turnInput * (forwardAccelBuildUp / 10) * turnStrength * Time.deltaTime * Input.GetAxis("Vertical"), 0f));
                }
                else if (reverseAccelBuildUp > 0)
                {
                    transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0f, turnInput * turnStrength * Time.deltaTime * Input.GetAxis("Vertical"), 0f));
                }

                leftFrontWheel.localRotation = Quaternion.Euler(leftFrontWheel.localRotation.eulerAngles.x, (turnInput * maxWheelTurn) - 180, leftFrontWheel.localRotation.eulerAngles.z);
                rightFrontWheel.localRotation = Quaternion.Euler(rightFrontWheel.localRotation.eulerAngles.x, turnInput * maxWheelTurn, rightFrontWheel.localRotation.eulerAngles.z);
            }
        }

        

        transform.position = rb.transform.position;


    }





    void GroundCheck()
    {


        
        RaycastHit hit;

        if (Physics.Raycast(groundRayPoint.position, -transform.up, out hit, groundRayLength, whatIsGround))
        {
            grounded = true;

            transform.rotation = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
        }
        else
        {
            grounded = false;
        }

    }





    void ApplyForce()
    {


        if (grounded)
        {
            rb.drag = dragOnGround;

            if (Mathf.Abs(speedInput) > 0)
            {
                rb.AddForce(transform.forward * speedInput);
            }
        }
        else
        {
            rb.drag = 0.1f;

            rb.AddForce(Vector3.up * -gravityForce * 100f);
        }


    }
}
