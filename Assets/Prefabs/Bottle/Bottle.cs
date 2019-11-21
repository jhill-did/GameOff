using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bottle : MonoBehaviour {
    public float rotationSpeed;
    public AudioClip pickUpSound;

    void Update() {
        this.transform.Rotate(Vector3.up, Time.deltaTime * this.rotationSpeed);
    }

    public void OnTriggerEnter(Collider other) {
        AudioSource.PlayClipAtPoint(pickUpSound, this.transform.position);
        Destroy(this.gameObject);
    }
}
