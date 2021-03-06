using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarPickup : MonoBehaviour
{

    public AudioSource coin1,coin2,coin1Drop,coin2Drop;
    public GameManager gameManager;
    public CarController carController;
    public GameObject coinObject, bigCoinObject;
    public Transform coinInstantiatePoint;

    private float lavaTimer;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        lavaTimer += Time.deltaTime;

        
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Coin")
        {
            Destroy(other.gameObject);
            coin1.Play();
            gameManager.AddCoins(1);
        }

        if (other.gameObject.tag == "Big_Coin")
        {
            Destroy(other.gameObject);
            coin2.Play();
            gameManager.AddCoins(5);
        }

        if (other.gameObject.tag == "Ice")
        {
            carController.turnStrength *= 2;
        }

        if (other.gameObject.tag == "OffTrack")
        {
            carController.isOffTrack = true;
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Lava" && lavaTimer >= 2 && gameManager.totalcoins > 0)
        {
            Quaternion carDirection = Quaternion.Euler(carController.transform.localRotation.x, carController.transform.localRotation.y + 90, carController.transform.localRotation.z);

            if (gameManager.totalcoins >= 5)
            {
                gameManager.SubtractCoins(5);
                Instantiate(coinObject, coinInstantiatePoint.position, carDirection);
                lavaTimer = 0;
                coin2Drop.Play();
            }
            else
            {
                gameManager.SubtractCoins(1);
                Instantiate(coinObject, coinInstantiatePoint.position, carDirection);
                lavaTimer = 0;
                coin1Drop.Play();
            }
            
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Ice")
        {
            carController.turnStrength /= 2;
        }

        if (other.gameObject.tag == "OffTrack")
        {
            carController.isOffTrack = false;
        }

    }
}
