using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI : MonoBehaviour
{
    [Header("Instantiate Points")]
    public List<GameObject> point;

    [Header("Still PowerUp Objects")]
    public GameObject boostObject;
    public GameObject dartObject, invincibleObject, bombObject, magnetObject, rocketObject, iceSpikesObject;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DisplayPowerUp(string powerUpType, int powerUpSlot)
    {
        

        switch (powerUpType)
        {
            case "Boost":
                Instantiate(boostObject, point[powerUpSlot - 1].transform.position, point[powerUpSlot - 1].transform.rotation, point[powerUpSlot - 1].transform.parent);
                break;

            case "Dart":
                Instantiate(dartObject, point[powerUpSlot - 1].transform.position, point[powerUpSlot - 1].transform.rotation, point[powerUpSlot - 1].transform.parent);
                break;

            case "InvincibilityOrb":
                Instantiate(invincibleObject, point[powerUpSlot - 1].transform.position, point[powerUpSlot - 1].transform.rotation, point[powerUpSlot - 1].transform.parent);
                break;

            case "Bomb":
                Instantiate(bombObject, point[powerUpSlot - 1].transform.position, point[powerUpSlot - 1].transform.rotation, point[powerUpSlot - 1].transform.parent);
                break;

            case "Magnet":
                Instantiate(magnetObject, point[powerUpSlot - 1].transform.position, point[powerUpSlot - 1].transform.rotation, point[powerUpSlot - 1].transform.parent);
                break;

            case "Rocket":
                Instantiate(rocketObject, point[powerUpSlot - 1].transform.position, point[powerUpSlot - 1].transform.rotation, point[powerUpSlot - 1].transform.parent);
                break;

            case "IceSpikes":
                Instantiate(iceSpikesObject, point[powerUpSlot - 1].transform.position, point[powerUpSlot - 1].transform.rotation, point[powerUpSlot - 1].transform.parent);
                break;
        }
    }
}
