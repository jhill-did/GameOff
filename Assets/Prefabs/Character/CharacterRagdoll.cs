using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterRagdoll : MonoBehaviour {
    public GameObject characterRoot;

    void Start() {
        SetActive(false);
    }

    void SetActive(bool active) {
        var rigidbodies = characterRoot.GetComponentsInChildren<Rigidbody>();
        foreach (var rigidbody in rigidbodies) {
            rigidbody.isKinematic = !active;
        }

        var colliders = characterRoot.GetComponentsInChildren<Collider>();
        foreach (var collider in colliders) {
            collider.enabled = active;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
