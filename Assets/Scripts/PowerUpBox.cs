using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpBox : MonoBehaviour
{

    float regenTimer = 0;

    public AudioSource sound_PowerUpBox;
    public GameManager gameManager;

    Car_Player_Collision carPlayer;
    Car_Bot carBot;

    public string[] powerUpList;

    void Update()
    {
        //waits 3 seconds for the object to reappear if it's not already active
        if (!gameObject.activeInHierarchy)
        {
            regenTimer += Time.deltaTime;
            if (regenTimer >= 3)
            {
                gameObject.SetActive(true);
            }
        }

        regenTimer = 0; Debug.Log(powerUpList.Length);
    }



    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {


            //sound_PowerUpBox.Play();

            

            carPlayer = other.gameObject.GetComponent<Car_Player_Collision>();
            carPlayer.AddPowerUp(powerUpList[Random.Range(0, powerUpList.Length)]);

            //gameObject.SetActive(false);
        }

        if (other.gameObject.tag == "Bot")
        {

            //sound_PowerUpBox.Play();
            

            carBot = other.gameObject.GetComponent<Car_Bot>();
            carBot.AddPowerUp(powerUpList[Random.Range(0, powerUpList.Length)]);

            gameObject.SetActive(false);
        }



    }


}
