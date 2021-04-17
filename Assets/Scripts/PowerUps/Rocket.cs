using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Rocket : MonoBehaviour
{
    List<GameObject> theCars = new List<GameObject>();
    List <Transform> theCarTransforms = new List<Transform>();

    float destroyTimer = 0;

    public float speed;

    Transform target;

    public GameObject explosion;


    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("0");

        theCars = GameObject.FindGameObjectsWithTag("Bot").ToList();//Debug.Log(theCars.Count + " " + FindCar(theCarTransforms.ToArray()));

        Debug.Log("1");

        foreach (GameObject car in theCars)
        {
            theCarTransforms.Add(car.transform);
        }

        Debug.Log("3");

        target = FindCar(theCarTransforms.ToArray());
    }

    // Update is called once per frame
    void Update()
    {
        if (target != null)
        {
            //transform.position = transform.forward;
            transform.position = Vector3.MoveTowards(transform.position, target.position, Time.deltaTime * 10 * speed);
            transform.LookAt(target.position);

            destroyTimer += Time.deltaTime;
            if (destroyTimer > 5)
            {
                Instantiate(explosion, transform.position, transform.rotation);
                Destroy(gameObject);
            }
        }
        else
        {
            Debug.LogError("There are no cars to aim for in the scene");
        }
        
    }

    Transform FindCar(Transform[] carTransforms)
    {
        Debug.Log("0");

        List<float> distance = new List<float>();
        float smallestDistance;
        smallestDistance = Mathf.Infinity;
        Transform nearestTransform;
        int carNumber = 0;

        Debug.Log("1");

        int numberOfCars = carTransforms.Length;

        Debug.Log(numberOfCars);
        for (int i = 0; i < numberOfCars; i++)
        {
            Debug.Log("2");

            distance.Add(Vector3.Distance(carTransforms[i].position, transform.position));

            Debug.Log("3");

            if (distance[i] < smallestDistance)
            {
                smallestDistance = distance[i];
                carNumber = i;


            }


        }


        Debug.Log("4");

        nearestTransform = carTransforms[carNumber];

        Debug.Log("5");

        return nearestTransform;


    }

}
