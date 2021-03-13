using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car_Bot : MonoBehaviour
{

    public Transform path;
    public float maxSteerAngle = 40;
    public Rigidbody rb;
    public float speed;
    public CarAI_Path carPath;

    private List<Transform> nodes;
    private int currentNode = 0;

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
    }

    void Update()
    {
        transform.position = rb.transform.position;
    }

    void ApplySteer()
    {
        Vector3 relativeVector = transform.InverseTransformPoint(nodes[currentNode].position);
        relativeVector /= relativeVector.magnitude;

        float newSteer = (relativeVector.x / relativeVector.magnitude) * maxSteerAngle;

        transform.rotation = Quaternion.Euler(0f, transform.rotation.y + newSteer, 0f);

        Debug.Log(newSteer);
        Debug.Log("currentNode" + currentNode);

    }

    void Drive()
    {
        rb.AddForce(transform.forward * speed);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(Vector3.zero, carPath.nodeCurrent);
    }
}
