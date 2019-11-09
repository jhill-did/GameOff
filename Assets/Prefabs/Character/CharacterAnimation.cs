using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimation : MonoBehaviour {
    public Character character;
    public Animator animator;
    public float maxSpeed = 1.0f;

    void Update() {
        var velocity = character.rigidBody.velocity;

        var horizontalMovement = new Vector3(velocity.x, 0.0f, velocity.z).magnitude / maxSpeed;
        var verticalMovement = velocity.y;
        animator.SetFloat("MovementSpeed", horizontalMovement);
        animator.SetFloat("VerticalSpeed", verticalMovement);
        animator.SetBool("Grounded", character.grounded);
    }
}
