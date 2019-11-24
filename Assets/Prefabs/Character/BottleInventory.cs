using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BottleInventory : MonoBehaviour {
    public int bottlesCollected = 0;
    public static int maxBottles;
    
    public void CollectBottle() {
        this.bottlesCollected += 1;

        if (this.bottlesCollected >= maxBottles) {
            Debug.Log("Winner!");
        }
    }
}
