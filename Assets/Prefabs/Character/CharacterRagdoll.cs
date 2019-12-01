using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterRagdoll : MonoBehaviour {
    public GameObject characterRoot;
    public Animator animator;

    void Start() {
        SetActive(false);
    }

    public void SetActive(bool active) {
        Debug.Log("RAGDOLL");

        var rigidbodies = characterRoot.GetComponentsInChildren<Rigidbody>();
        foreach (var rigidbody in rigidbodies) {
            rigidbody.isKinematic = !active;
        }

        var colliders = characterRoot.GetComponentsInChildren<Collider>();
        foreach (var collider in colliders) {
            collider.enabled = active;
        }

        animator.enabled = !active;
    }
}
