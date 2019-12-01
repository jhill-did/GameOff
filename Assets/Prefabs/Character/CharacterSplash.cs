using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSplash : MonoBehaviour {
    public GameObject splashEffects;
    public AudioClip splashSound;
    public new Rigidbody rigidbody;
    public float velocityThreshold;
    public float heightThreshold;

    private bool underWater = false;

    void Update() {
        var playerVelocity = rigidbody.velocity.magnitude;
        bool fastEnough = playerVelocity > velocityThreshold;

        var verticalPosition = transform.position.y;
        var rightHeight = verticalPosition < this.heightThreshold;

        if (fastEnough && rightHeight && !underWater) {
            // Play a sound as we hit the water.
            AudioSource.PlayClipAtPoint(splashSound, transform.position);
            Instantiate(splashEffects, transform.position, Quaternion.identity);

            underWater = true;
        }

        // Make sure we don't trigger the sound again until we exit the water.
        if (!rightHeight) {
            this.underWater = false;
        }
    }
}
