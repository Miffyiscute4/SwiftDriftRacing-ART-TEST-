using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarPickup : MonoBehaviour
{

    public AudioSource coin1,coin2;
    public GameManager gameManager;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Coin")
        {
            Destroy(other.gameObject);
            coin1.Play();
            gameManager.AddCoins(1);
        }

        if (other.gameObject.name == "Big_Coin")
        {
            Destroy(other.gameObject);
            coin2.Play();
            gameManager.AddCoins(5);
        }
    }
}
