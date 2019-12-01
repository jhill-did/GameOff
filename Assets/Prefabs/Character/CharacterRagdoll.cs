using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterRagdoll : MonoBehaviour {
    public GameObject characterRoot;
    public Animator animator;

    public bool active = false;

    struct BoneInfo {
        public Vector3 position;
        public Vector3 localPosition;
        public Quaternion rotation;
        public Quaternion localRotation;
    };

    Dictionary<Transform, BoneInfo> initialSetup = new Dictionary<Transform, BoneInfo>();

    void Start() {
        foreach (Transform child in transform) {
            var boneInfo = new BoneInfo() {
                position = child.position,
                localPosition = child.localPosition,
                rotation = child.localRotation,
                localRotation = child.localRotation
            };

            initialSetup.Add(child, boneInfo);
        }

        SetActive(this.active);
    }

    public void SetActive(bool enabled) {
        this.active = enabled;

        foreach (Transform child in transform) {
            child.gameObject.SetActive(false);
            child.gameObject.SetActive(true);
        }

        animator.enabled = !active;

        var rigidbodies = characterRoot.GetComponentsInChildren<Rigidbody>();
        foreach (var rigidbody in rigidbodies) {
            rigidbody.isKinematic = !active;
        }

        var colliders = characterRoot.GetComponentsInChildren<Collider>();
        foreach (var collider in colliders) {
            collider.enabled = active;
        }

        // Special case, replace all our local offsets when coming
        // out of a ragdoll.
        if (!enabled) {
            foreach (Transform child in transform) {
                var boneInfo = initialSetup[child];
                child.position = boneInfo.position;
                child.localPosition = boneInfo.localPosition;
                child.rotation = boneInfo.rotation;
                child.localRotation = boneInfo.localRotation;
            }
        }
    }
}
