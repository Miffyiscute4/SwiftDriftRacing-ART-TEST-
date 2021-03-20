using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public int totalcoins;

    public string slotNumber, reserveSlotNumber;

    public Text coinText, slotText, reserveSlot;

    //powerups
    public GameObject dart;

    // Update is called once per frame
    void Update()
    {
        coinText.text = "Coins: " + totalcoins;
        slotText.text = "Slot: " + slotNumber;
        reserveSlot.text = "Slot" + reserveSlotNumber;

    }

    public void AddCoins(int coinAmount)
    {
        totalcoins += coinAmount;
    }





    public void SubtractCoins(int coinAmount)
    {
        totalcoins -= coinAmount;
    }
}
