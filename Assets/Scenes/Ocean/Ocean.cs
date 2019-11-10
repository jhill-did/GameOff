using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ocean : MonoBehaviour {
    public GameObject player;

    void Update() {
        var targetPosition = player.transform.position.getHorizontalPart()
            + Vector3.up * -0.67f;
        this.transform.position = targetPosition;
    }
}
