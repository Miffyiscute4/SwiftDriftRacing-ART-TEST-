using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bot_CarController: Player_CarController
{
    //path
    [Header("Path")]
    public GameObject pathToFollow;
    List<Transform> nodes;
    int currentNode = 1;
    Vector3 currentNodePosition;
    public float distanceTillNextNode = 30;

    //aiming & avoiding
    [Header("Aiming & Avoiding")]
    private bool avoiding = false; private bool aimingForObject = false;
    public GameObject objectToAimFor;

    //sensor
    [Header("Sensor")]
    public Vector3 frontSensorPos = new Vector3(0, 0.2f, 0.5f);
    public float sensorLength = 5f, frontSideSensorPos, frontSensorAngle = 30;

    // Start is called before the first frame update
    void Start()
    {
        Transform[] pathTransforms = pathToFollow.GetComponentsInChildren<Transform>();
        nodes = new List<Transform>();

        for (int i = 0; i < pathTransforms.Length; i++)
        {
            if (pathTransforms[i] != pathToFollow.transform)
            {
                nodes.Add(pathTransforms[i]);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isGrounded)
        {
            Vertical();
            Turn();
        }

        //follows the car at all times
        transform.position = rb.transform.position;
        currentNodePosition = nodes[currentNode].position;
    }

    void FixedUpdate()
    {
        GroundCheck();
        ApplyForce();
        CheckWayPoint();
        Sensors();
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

    void Turn()
    {

            rb.AddForce(transform.forward * currentSpeed * speedMultiplier * 10);

            stopWatch_VerticalBuildUp += Time.deltaTime;

            if (stopWatch_VerticalBuildUp < verticalDelayTime && currentSpeed < maxSpeed)
            {
                currentSpeed++;
                stopWatch_VerticalBuildUp = 0;
            }


            turnStrength = currentSpeed;

            /*if (bot.coinCount < 10)
            {
                maxForwardAccelBuildUp = maxSpeed + bot.coinCount;
            }
            else
            {
                maxForwardAccelBuildUp = maxSpeed + 10;
            }*/
    }



    void Vertical()
    {
        
        if (!avoiding)
        {
            Vector3 difference = new Vector3(nodes[currentNode].position.x, transform.position.y, nodes[currentNode].position.z) - transform.position;

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
            transform.Rotate(transform.rotation.x, avoidMultiplier * (turnStrength / 10), transform.rotation.z);


        }
        //Debug.Log(avoiding);
    }



    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, currentNodePosition);
    }
}
