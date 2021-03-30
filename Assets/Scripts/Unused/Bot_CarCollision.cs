using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bot_CarCollision : CarCollision
{

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
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
