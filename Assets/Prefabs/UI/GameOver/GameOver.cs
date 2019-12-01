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
    // Start is called before the first frame update
    void Start()
    {
        countLabel.SetText("");
    }

    // Update is called once per frame
    void Update()
    {
        if(inventory.bottlesCollected == 7 && !didWin)
        {
            countLabel.SetText("Thanks for playing!");
            didWin = true;
        }
        
    }

}
