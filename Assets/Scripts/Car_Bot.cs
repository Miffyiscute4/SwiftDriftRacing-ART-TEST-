using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car_Bot : MonoBehaviour
{

    public Transform path;
    public float maxSteerAngle = 40, turnSpeed;
    public Rigidbody rb;
    //public float speed;
    //public CarAI_Path carPath;

    private List<Transform> nodes;
    private int currentNode = 1;
    [HideInInspector] public float forwardAccelBuildUp = 0, maxForwardAccelBuildUp, boostDelay;
    Vector3 currentNodePosition;

    public float sensorLength = 5f, frontSideSensorPos, frontSensorAngle = 30;
    public Vector3 frontSensorPos = new Vector3(0, 0.2f, 0.5f);

    private bool avoiding = false, aimingForObject = false;
    private float accelDelay, decelDelay;

    public LayerMask whatIsGround;
    public float groundRayLength = 5;
    public Transform groundRayPoint;

    public int maxSpeed;

    public GameObject objectToAimFor;

    public float distanceTillNextNode;

    public Car_Bot_Collision bot;

    string powerUpSlot1, powerUpSlot2;
    int currentPowerUpSlot;

    //--------------------------------
    public bool isOffTrack = false, isBoosted = false;

    // Start is called before the first frame update
    void Start()
    {
        rb.transform.parent = null;

        Transform[] pathTransforms = path.GetComponentsInChildren<Transform>();
        nodes = new List<Transform>();

        for (int i = 0; i < pathTransforms.Length; i++)
        {

            if (pathTransforms[i] != path.transform)
            {

                nodes.Add(pathTransforms[i]);

            }

        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        MaterialEffects();
        ApplySteer();
        Drive();
        CheckWayPoint();
        Sensors();
        GroundCheck();

        //Debug.Log(forwardAccelBuildUp);
    }

    void Update()
    {
        transform.position = rb.transform.position;

        currentNodePosition = nodes[currentNode].position;
    }

    void ApplySteer()
    {
        /* Vector3 relativeVector = transform.InverseTransformPoint(nodes[currentNode].position);
         relativeVector /= relativeVector.magnitude;

         float newSteer = (relativeVector.x / relativeVector.magnitude) * maxSteerAngle;

         transform.rotation = Quaternion.Euler(0f, newSteer, 0f);

         Debug.Log("currentNode = " + currentNode);*/

        //transform.LookAt(new Vector3(nodes[currentNode].position.x, transform.position.y, nodes[currentNode].position.z));
        if (!avoiding/* && !aimingForObject*/)
        {
            Vector3 difference = new Vector3(nodes[currentNode].position.x, transform.position.y, nodes[currentNode].position.z) -transform.position;

            Quaternion lookRotation = Quaternion.LookRotation(difference);

            transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, Time.deltaTime);
        }

        else if (!avoiding && aimingForObject && objectToAimFor != null)
        {
            Vector3 difference = new Vector3(objectToAimFor.transform.position.x, transform.position.y, objectToAimFor.transform.position.z) - transform.position;

            Quaternion lookRotation = Quaternion.LookRotation(difference);

            transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, Time.deltaTime);
        }
        
        if (objectToAimFor = null)
        {
            aimingForObject = false;
        }

    }

    void GroundCheck()
    {
        RaycastHit hit;

        if (Physics.Raycast(groundRayPoint.position, -transform.up, out hit, groundRayLength, whatIsGround))
        {
            transform.rotation = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
        }
    }



    void Drive()
    {
        rb.AddForce(transform.forward * forwardAccelBuildUp * 1000);

        accelDelay += Time.deltaTime;
        decelDelay += Time.deltaTime;

        if (accelDelay >= 0.5 && accelDelay <= maxForwardAccelBuildUp)
        {
            forwardAccelBuildUp++;
            accelDelay = 0;
        }

        if (forwardAccelBuildUp > maxSpeed)
        {
            forwardAccelBuildUp = maxSpeed;
        }

        turnSpeed = forwardAccelBuildUp;

        if (bot.coinCount < 10)
        {
            maxForwardAccelBuildUp = maxSpeed + bot.coinCount;
        }
        else
        {
            maxForwardAccelBuildUp = maxSpeed + 10;
        }
        

        //Debug.Log(forwardAccelBuildUp + " " + bot.coinCount);
    }



    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, currentNodePosition);
    }

    void CheckWayPoint()
    {
        if (Vector3.Distance(transform.position, nodes[currentNode].position) < distanceTillNextNode)
        {
            if (currentNode == nodes.Count - 1)
            {
                currentNode = 0;
            }
            else
            {
                currentNode++;
                
            }


        }
    }



    void Sensors()
    {
        RaycastHit hit;
        Vector3 sensorStartPos = transform.position;
        sensorStartPos += transform.forward * frontSensorPos.z;
        sensorStartPos += transform.up * frontSensorPos.y;

        //sensorStartPos = transform.position;

        float avoidMultiplier = 0f;
        avoiding = false;

        

        //FRONT RIGHT SENSOR
        sensorStartPos += transform.right * frontSideSensorPos;
        if (Physics.Raycast(sensorStartPos, transform.forward, out hit, sensorLength) && !hit.collider.CompareTag("Ramp"))// && !hit.collider.gameObject == rb)
        {
            Debug.DrawLine(sensorStartPos, hit.point, Color.blue);
            avoiding = true;
            avoidMultiplier -= 1f;

            if (hit.collider.CompareTag("Collectable"))
            {
                objectToAimFor = hit.collider.gameObject;
                aimingForObject = true;
            }

        }
        //FRONT RIGHT ANGLE SENSOR
        else if (Physics.Raycast(sensorStartPos, Quaternion.AngleAxis(frontSensorAngle, transform.up) * transform.forward, out hit, sensorLength) && !hit.collider.CompareTag("Ramp"))// && !hit.collider.gameObject == rb)
        {
            Debug.DrawLine(sensorStartPos, hit.point, Color.blue);
            avoiding = true;
            avoidMultiplier -= 0.5f;

            if (hit.collider.CompareTag("Collectable"))
            {
                objectToAimFor = hit.collider.gameObject;
                aimingForObject = true;
            }

        }

        //FRONT RIGHT SENSOR
        sensorStartPos -= transform.right * frontSideSensorPos * 2;
        if (Physics.Raycast(sensorStartPos, transform.forward, out hit, sensorLength) && !hit.collider.CompareTag("Ramp")) //&& !hit.collider.gameObject == rb)
        {
            Debug.DrawLine(sensorStartPos, hit.point, Color.blue);
            avoiding = true;
            avoidMultiplier += 1f;


            if (hit.collider.CompareTag("Collectable"))
            {
                objectToAimFor = hit.collider.gameObject;
                aimingForObject = true;
            }

        }//FRONT LEFT ANGLE SENSOR
        else if (Physics.Raycast(sensorStartPos, Quaternion.AngleAxis(-frontSensorAngle, transform.up) * transform.forward, out hit, sensorLength) && !hit.collider.CompareTag("Ramp")) //&& !hit.collider.gameObject == rb)
        {
            Debug.DrawLine(sensorStartPos, hit.point, Color.blue);
            avoiding = true;
            avoidMultiplier += 0.5f;

            if (hit.collider.CompareTag("Collectable"))
            {
                objectToAimFor = hit.collider.gameObject;
                aimingForObject = true;
            }


        }

        //FRONT CENTER SENSOR
        if (avoidMultiplier == 0 && Physics.Raycast(sensorStartPos, transform.forward, out hit, sensorLength) && !hit.collider.CompareTag("Ramp"))// && !hit.collider.gameObject == rb)
        {
            Debug.DrawLine(sensorStartPos, hit.point, Color.blue);
            avoiding = true;

            if (hit.normal.x > 0)
            {
                avoidMultiplier = -1;
            }
            else
            {
                avoidMultiplier = 1;
            }

            if (hit.collider.CompareTag("Collectable"))
            {
                objectToAimFor = hit.collider.gameObject;
                aimingForObject = true;
            }


        }

        if (avoiding && !aimingForObject)
        {
            sensorStartPos = transform.position;
            transform.Rotate(transform.rotation.x, avoidMultiplier * (turnSpeed / 10), transform.rotation.z);


        }
        //Debug.Log(avoiding);
    }

    public void AddPowerUp(string powerUp)
    {
        if (powerUpSlot1 != null)
        {
            powerUpSlot1 = powerUp;
        }
        else
        {
            powerUpSlot2 = powerUp;
        }
    }

    void MaterialEffects()
    {
        if (!isOffTrack)
        {
            if (isBoosted)
            {
                boostDelay += Time.deltaTime;

                if (boostDelay >= 3)
                {

                    forwardAccelBuildUp = maxSpeed;

                    isBoosted = false;

                }
                else
                {
                    rb.AddForce(transform.forward * 25000);
                    transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0f, 1 * turnSpeed * Time.deltaTime * Input.GetAxis("Vertical"), 0f));
                }

            }
            else
            {
                boostDelay = 0;

                if (bot.coinCount >= maxSpeed + bot.coinCount)
                {
                    maxSpeed = maxSpeed + bot.coinCount;
                    //maxReverseAccel = 10;
                }
                else if (bot.coinCount <= maxSpeed)
                {
                    maxSpeed = bot.coinCount + maxSpeed;
                    //maxReverseAccel = bot.coinCount + 2;
                }
            }


        }
        else
        {
            //maxForwardAccel = maxSpeed;
            //maxReverseAccel = 2;

            isBoosted = false;
        }
    }
}
