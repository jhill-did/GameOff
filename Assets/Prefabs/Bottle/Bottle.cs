using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bottle : MonoBehaviour {
    public float rotationSpeed;

    void Update() {
        this.transform.Rotate(Vector3.up, Time.deltaTime * this.rotationSpeed);
    }

    public void OnTriggerEnter(Collider other) {
        Destroy(this.gameObject);
    }
}
