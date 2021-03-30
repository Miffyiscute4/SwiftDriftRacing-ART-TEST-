using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car_Bot_Collision : CarCollision
{
    void Start()
    {
        currentPowerUpSlot = 1;
    }

    void Update()
    {
        lavaTimer += Time.deltaTime;
        PowerUpRegenTimer += Time.deltaTime;

        BoolActions();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            UsePowerUp();
        }

        if (Input.GetKeyDown(KeyCode.LeftAlt))
        {
            SwapPowerUp();
        }

        if (powerUpBoxObject != null && !powerUpBoxObject.activeInHierarchy)
        {
            powerUpBoxDelay += Time.deltaTime;

            if (powerUpBoxDelay >= 3)
            {
                powerUpBoxObject.SetActive(true);
                powerUpBoxDelay = 0;
            }
        }

        //Debug.Log(coinCount);
    }
}
