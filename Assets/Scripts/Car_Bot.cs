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
    private int currentNode = 1, forwardAccelBuildUp = 0, maxForwardAccelBuildUp;
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

    public CarBotPickup bot;

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
        ApplySteer();
        Drive();
        CheckWayPoint();
        Sensors();
        GroundCheck();
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

        if (accelDelay >= 0.3 && accelDelay <= maxForwardAccelBuildUp)
        {
            forwardAccelBuildUp++;
        }

        if (forwardAccelBuildUp > maxSpeed)
        {
            forwardAccelBuildUp = maxSpeed;
        }

        turnSpeed = forwardAccelBuildUp;

        maxForwardAccelBuildUp = maxSpeed + bot.coinCount;

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
}
