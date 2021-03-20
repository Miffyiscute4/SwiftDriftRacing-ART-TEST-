using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpBox : MonoBehaviour
{

    float regenTimer = 0;

    public AudioSource sound_PowerUpBox;

    Car_Player carPlayer;
    Car_Bot carBot;

    public string[] powerUpList = { "333", "444" };

    void Update()
    {
        //waits 3 seconds for the object to reappear if it's not already active
        if (!gameObject.activeInHierarchy)
        {
            regenTimer += Time.deltaTime;
            if (regenTimer >= 3)
            {
                gameObject.SetActive(true);
                regenTimer = 0;
            }
        }
        
    }



    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            gameObject.SetActive(false);

            sound_PowerUpBox.Play();

            carPlayer = other.gameObject.GetComponent<Car_Player>();


            Debug.Log(other);
            
            carPlayer.AddPowerUp(powerUpList[Random.Range(0, powerUpList.Length)]);
        }

        if (other.gameObject.tag == "Bot")
        {
            gameObject.SetActive(false);

            sound_PowerUpBox.Play();

            carBot = other.gameObject.GetComponent<Car_Bot>();

            carBot.AddPowerUp(powerUpList[Random.Range(0, powerUpList.Length)]);
        }



    }


}
