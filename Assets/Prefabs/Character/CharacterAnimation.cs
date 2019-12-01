using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimation : MonoBehaviour {
    public CharacterFootsteps footsteps;
    public Character character;
    public Animator animator;
    public float maxSpeed = 1.0f;

    void Update() {
        var velocity = character.rigidBody.velocity;

        var horizontalMovement = new Vector3(velocity.x, 0.0f, velocity.z).magnitude / maxSpeed;
        var verticalMovement = velocity.y;
        animator.SetFloat("MovementSpeed", horizontalMovement);
        animator.SetFloat("VerticalSpeed", verticalMovement);
        animator.SetFloat("GlidePitch", character.glidePitch);
        animator.SetBool("Grounded", character.grounded);
        animator.SetInteger("MovementMode", (int)character.movementMode);
        animator.SetBool("Charging", character.chargingLaunch);

        // Update camera FOV for launch charge.
        var camera = character.camera.GetComponent<Camera>();
        var chargeAmount = character.launchHoldTimer;
        camera.fieldOfView = Mathf.Lerp(75.0f, 90.0f, chargeAmount);
    }

    public void StepLeft() {
        this.footsteps.StepLeft();
    }

    public void StepRight() {
        this.footsteps.StepRight();
    }
}
