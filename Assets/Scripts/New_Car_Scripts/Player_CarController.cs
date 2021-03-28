using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_CarController : MonoBehaviour
{
    //objects & components
    [Header("Objects & Components")]
    public Rigidbody rb;
    public Transform groundRayPoint;

    //car variables
    [Header("Car Variables")]
    public float speedMultiplier = 7.5f; public float maxSpeed = 15; public float verticalDelayTime = 0.5f; public float turnStrength = 30; public float driftMultiplier = 2;

    float speedInput, currentSpeed, driftInput;

    //groundcheck
    [Header("Ground Check")]
    public LayerMask whatIsGround;
    public float groundRayLength;

    bool isGrounded;

    //counters
    float stopWatch_VerticalBuildUp;

    // Start is called before the first frame update
    void Start()
    {
        
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
        stopWatch_VerticalBuildUp += Time.deltaTime;


        speedInput = currentSpeed * speedMultiplier;

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
            }
           
        }
    }



    void TurnInput()
    {

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            driftInput = Input.GetAxisRaw("Horizontal");
        }
        else if (Input.GetKey(KeyCode.LeftShift))
        {
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0f, driftMultiplier * (currentSpeed / 10) * driftInput * (currentSpeed / 10) * turnStrength * Time.deltaTime * Input.GetAxis("Vertical"), 0f));
        }
        else if (Input.GetAxisRaw("Horizontal") != 0)
        {
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0f, Input.GetAxis("Horizontal") * (currentSpeed / 10) * turnStrength * Time.deltaTime * Input.GetAxis("Vertical"), 0f));
        }

    }




    void GroundCheck()
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



    void ApplyForce()
    {
        if (isGrounded)
        {
            rb.drag = 3;

            if (Mathf.Abs(speedInput) > 0)
            {
                //moves car
                rb.AddForce(transform.forward * speedInput);

                rb.AddForce(-Vector3.up * 10);
            }
        }
        else
        {
            rb.drag = 0.1f;

            rb.AddForce(-Vector3.up * 5000);
        }
    }
}
