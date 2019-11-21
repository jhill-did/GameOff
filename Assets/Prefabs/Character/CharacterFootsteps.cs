using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StepType {
    Sand,
    Stone,
    Water,
}

public class CharacterFootsteps : MonoBehaviour {
    public AudioSource audioSource;
    public Rigidbody rigidBody;
    public AnimationCurve volumeCurve;
    public AudioClip[] sandSteps;
    public AudioClip[] stoneSteps;
    public AudioClip[] waterSteps;
    private Dictionary<StepType, AudioClip[]> clipLookup;

    public Transform leftFoot;
    public Transform rightFoot;

    public void Start() {
        this.clipLookup = new Dictionary<StepType, AudioClip[]>();
        this.clipLookup.Add(StepType.Sand, sandSteps);
        this.clipLookup.Add(StepType.Stone, stoneSteps);
        this.clipLookup.Add(StepType.Water, waterSteps);
    }

    public void Update()
    {
        Debug.Log(rigidBody.velocity.magnitude);
    }

    public void StepLeft() {
        Step(this.leftFoot.position);
    }

    public void StepRight() {
        Step(this.rightFoot.position);
    }

    void Step(Vector3 footPosition) {
        var speed = rigidBody.velocity.magnitude;
        var type = StepType.Sand;

        var sounds = this.clipLookup[type];
        var randomIndex = Random.Range(0, sounds.Length);
        var randomSound = sounds[randomIndex];

        this.audioSource.pitch = Random.Range(0.95f, 1.05f);
        this.audioSource.volume = this.volumeCurve.Evaluate(speed);
        this.audioSource.PlayOneShot(randomSound);
    }
}
