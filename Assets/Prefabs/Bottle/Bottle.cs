using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bottle : MonoBehaviour {
    public float rotationSpeed;
    public AudioClip pickUpSound;

    public void Awake() {
        BottleInventory.maxBottles += 1;
    }

    void Update() {
        this.transform.Rotate(Vector3.up, Time.deltaTime * this.rotationSpeed);
    }

    public void OnTriggerEnter(Collider other) {
        var character = other.gameObject.GetComponent<Character>();
        if (character != null) {
            var inventory = other.gameObject.GetComponent<BottleInventory>();
            inventory.CollectBottle();

            AudioSource.PlayClipAtPoint(pickUpSound, this.transform.position);
            Destroy(this.gameObject);
        }
        
    }
}
