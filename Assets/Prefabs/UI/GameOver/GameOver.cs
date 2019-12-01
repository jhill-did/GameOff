using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameOver : MonoBehaviour
{
    public BottleInventory inventory;
    public TextMeshProUGUI countLabel;
    float currentCount = 0;

    bool didWin = false;

    void Start()
    {
        countLabel.SetText("");
    }

    void Update()
    {
        if (inventory.bottlesCollected == BottleInventory.maxBottles && !didWin) {
            countLabel.SetText("Thanks for playing!");
            didWin = true;
        }
        
    }
}
