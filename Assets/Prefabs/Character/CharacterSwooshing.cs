using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSwooshing : MonoBehaviour {
    public Rigidbody rigidBody;
    public AudioSource audioSource;
    private float volume = 0.0f;
    public float volumeDecay = 2.0f;
    public AnimationCurve volumeCurve;

    void Update() {
        var speed = this.rigidBody.velocity.magnitude;
        /*
        this.volume = Mathf.Clamp01(
            this.volume
            + adjustedVelocity * Time.deltaTime
            - volumeDecay * Time.deltaTime
        );*/

        var value = this.volumeCurve.Evaluate(speed);
        this.audioSource.volume = value;
        this.audioSource.pitch = value + 0.5f;
    }
}
