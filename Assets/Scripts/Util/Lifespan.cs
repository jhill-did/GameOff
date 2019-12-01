using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lifespan : MonoBehaviour {
    public float lifespan;
    private float timer;

    void Update() {
        this.timer += Time.deltaTime;

        if (timer > lifespan) {
            Destroy(this.gameObject);
        }
    }
}
