using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockPiece : MonoBehaviour {
    void OnCollisionEnter(Collision collision) {
        var isPlayer = collision.gameObject.GetComponent<Character>() == null ? false : true;
        var isRock = collision.gameObject.GetComponent<RockPiece>() == null ? false : true;
        if (!isPlayer && !isRock) {
            // Debug-draw all contact points and normals
            var audioSource = GetComponent<AudioSource>();
            audioSource.spatialBlend = 1.0f;
            audioSource.Play();
            audioSource.pitch = Random.Range(0.8f, 1.2f);
        }
    }
}
