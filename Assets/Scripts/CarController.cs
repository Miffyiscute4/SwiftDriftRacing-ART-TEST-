using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    public Rigidbody rb;

    public float maxForwardAccel = 18, maxReverseAccel = 9, turnStrength = 30, gravityForce = 10, dragOnGround = 3, delayAmount = 0.2f;

    private float speedInput, turnInput, accelDelay, decelDelay, forwardAccelBuildUp, reverseAccelBuildUp;

    private bool grounded;

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
        

        speedInput = 0f;
        //forwardAccelBuildUp = 0;

        accelDelay += Time.deltaTime;
        decelDelay += Time.deltaTime;



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
        else
        {
            if (decelDelay >= delayAmount && forwardAccelBuildUp > 0 || reverseAccelBuildUp > 0)
            {
                forwardAccelBuildUp -= 1;
                reverseAccelBuildUp -= 1;

                decelDelay = 0;

            }
        }

        if(forwardAccelBuildUp < 0)
        {
            forwardAccelBuildUp = 0;
        }
        else if (reverseAccelBuildUp < 0)
        {
            reverseAccelBuildUp = 0;
        }

            turnInput = Input.GetAxis("Horizontal");

        if (grounded)
        {

            if (Input.GetKey(KeyCode.Space) && Input.GetAxis("Vertical") > 0)
            {
                int driftInput = Mathf.RoundToInt(Input.GetAxis("Horizontal"));

                transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0f, driftInput * (turnStrength * 2) * Time.deltaTime * Input.GetAxis("Vertical"), 0f));


            }
            else
            {
                transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0f, turnInput * turnStrength * Time.deltaTime * Input.GetAxis("Vertical"), 0f));
            }
        }

        leftFrontWheel.localRotation = Quaternion.Euler(leftFrontWheel.localRotation.eulerAngles.x, (turnInput * maxWheelTurn) - 180, leftFrontWheel.localRotation.eulerAngles.z);
        rightFrontWheel.localRotation = Quaternion.Euler(rightFrontWheel.localRotation.eulerAngles.x, turnInput * maxWheelTurn, rightFrontWheel.localRotation.eulerAngles.z);
       
        transform.position = rb.transform.position;
    }

    void FixedUpdate()
    {
        grounded = false;
        RaycastHit hit;

        if (Physics.Raycast(groundRayPoint.position, -transform.up, out hit, groundRayLength, whatIsGround))
        {
            grounded = true;

            transform.rotation = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
        }

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
