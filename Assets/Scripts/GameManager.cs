using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Text coinText;
    public Car_Player_Collision car;

    // Update is called once per frame
    void Update()
    {
        coinText.text = "Coins: " + car.coinCount; 
    }
}
