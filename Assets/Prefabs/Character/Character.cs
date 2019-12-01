using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum MovementMode { Ground = 0, Air = 1, Glide = 2};

public class Character : MonoBehaviour {
    public CharacterRagdoll ragdoll;
    public Rigidbody rigidBody;
    public new GameObject camera;
    public GameObject cameraArm;
    public GameObject cameraBase;
    private float cameraYaw = 0.0f;
    public float rotationSpeed = 5.0f;

    public MovementMode movementMode;

    public AudioClip launchSound;
    public AudioSource chargeAudioSource;
    public bool chargingLaunch = false;
    public float launchHoldTimer = 0.0f;
    public float launchHoldRate = 1.0f;
    public float maxLaunchForce;

    public bool jumping = false;
    public float jumpTimer = 0.0f;
    public float maxJumpTime = 1.0f;
    public float jumpAcceleration;
    public bool grounded;
    public RaycastHit floorHitInfo;

    private Vector3 lastPositiveMovementDirection = Vector3.zero;
    private Vector2 movementInput = Vector2.zero;
    public float groundAcceleration;
    public float airAcceleration;
    public float inputAirVelocity;
    public float groundFriction;
    public float maxGroundVelocity;

    public float glideAngle = 0.0f;
    public float glidePitch = 1.0f; // 0 = Down, 1 = Forward.

    public float cameraMovementSensitivity = 0.2f;
    private Vector2 cameraMovementDirection = Vector3.zero;

    public Vector3 spawnPosition;

    void Start() {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update() {
        // Camera Yaw.
        this.cameraYaw = this.cameraYaw
            + this.cameraMovementDirection.x
            * this.cameraMovementSensitivity;

        cameraBase.transform.rotation = Quaternion
            .Euler(Vector3.up * this.cameraYaw);

        // Camera Pitch.
        var localPitch = cameraArm.transform.localEulerAngles.x;
        localPitch = localPitch > 180 ? localPitch - 360 : localPitch;

        var adjustedPitch = localPitch
            - this.cameraMovementDirection.y
            * this.cameraMovementSensitivity;

        var clampedPitch = Mathf.Clamp(adjustedPitch, -60.0f, 89.0f);

        cameraArm.transform.localRotation = Quaternion
            .Euler(Vector3.right * clampedPitch);

        launchHoldTimer = chargingLaunch
            ? Mathf.Clamp01(launchHoldTimer + launchHoldRate * Time.deltaTime)
            : 0.0f;

        chargeAudioSource.volume = launchHoldTimer;

        // Code for levitating surrounding objects
        if (chargingLaunch) {
            var hits = Physics.SphereCastAll(transform.position, 20, transform.forward, 10.0f);
            foreach(RaycastHit hit in hits) {
                var hitRigidBody = hit.transform.gameObject.GetComponent<Rigidbody>();
                var isPlayer = hit.transform.gameObject.GetComponent<Character>() == null ? false : true;
                if(hitRigidBody != null && !isPlayer) {
                    hitRigidBody.AddForce(Vector3.up * 1000 * Time.deltaTime, ForceMode.Acceleration);
                }
            }
        }
    }

    void TurnTowards(Vector3 direction) {
        var targetRotation = Quaternion
            .LookRotation(direction.getHorizontalPart());

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            Time.fixedDeltaTime * rotationSpeed
        );
    }

    Vector3 GroundMove(Vector3 currentForce) {
        var movementDirection = GetInputDirection();

        // If we're charging, point in our camera direction.
        if (chargingLaunch) {
            TurnTowards(camera.transform.forward);
        }

        // Turn to face our movementDirection.
        if (!chargingLaunch && this.lastPositiveMovementDirection.magnitude > 0.0f) {
            TurnTowards(this.lastPositiveMovementDirection);
        }

        // Handle Jumping.
        var jumpForce = jumping
            ? Vector3.up * jumpAcceleration
            : Vector3.zero;

        // Handle floor magnetism.
        var floorMagnetForce = this.grounded
            ? floorHitInfo.normal * -50.0f
            : Vector3.zero;

        // Apply ground friction.
        var velocityDirection = this.rigidBody.velocity.normalized;
        var angleDot = Vector3.Dot(floorHitInfo.normal, Vector3.up);
        var normalForce = 1.0f * 9.81f * angleDot;
        var frictionForce = -velocityDirection * this.groundFriction * normalForce;

        var totalForce = currentForce
            + movementDirection * groundAcceleration
            + jumpForce
            + frictionForce;

        // Clamp velocity.
        this.rigidBody.velocity = Vector3
            .ClampMagnitude(this.rigidBody.velocity, maxGroundVelocity);

        return totalForce;
    }

    Vector3 AirMove(Vector3 currentForce) {
        var movementDirection = GetInputDirection();
        var totalForce = currentForce
            + movementDirection * airAcceleration;

        var horizontalVelocity = this.rigidBody
            .velocity
            .getHorizontalPart();

        if (horizontalVelocity.magnitude > 0.0f) {
            TurnTowards(horizontalVelocity);
        }

        return totalForce;
    }

    Vector3 GlideMove(Vector3 currentForce) {
        var cameraYaw = this.cameraArm.transform.localEulerAngles.x;
        var adjustedYaw = cameraYaw > 180.0f ? cameraYaw - 360.0f : cameraYaw;
        glidePitch = Math.Smoothstep(90.0f, 0.0f, adjustedYaw);

        // Calculate vertical air drag.
        var verticalVelocity = rigidBody.velocity.getVerticalPart();
        var verticalVelocitySquared = Mathf.Pow(verticalVelocity.magnitude, 2.0f);
        var verticalSurfaceArea = glidePitch * 0.5f;
        var verticalDrag = 0.6f * (1.225f * verticalVelocitySquared) / 2.0f * verticalSurfaceArea;

        // Calculate horizontal air drag.
        var horizontalVelocity = this.rigidBody.velocity.getHorizontalPart();
        var horizontalVelocitySquared = Mathf.Pow(horizontalVelocity.magnitude, 2.0f);
        var horizontalSurfaceArea = 0.5f - verticalSurfaceArea;
        var horizontalDrag = 0.05f * (1.225f * horizontalVelocitySquared) / 2.0f * horizontalSurfaceArea;

        // Calculate steering force.
        var lookDirection = this.camera.transform.forward.normalized.getHorizontalPart();
        var steerForce = lookDirection * 40.0f;
        var steerDot = Vector3.Dot(horizontalVelocity.normalized, lookDirection);
        var adjustedSteerForce = steerForce * (1.5f - steerDot);

        Debug.Log("steerDot: " + steerDot.ToString());

        // Rotate towards our movement direction.
        if (horizontalVelocity.magnitude > 0.0f) {
            TurnTowards(horizontalVelocity);
        }

        var totalForce = currentForce
            + verticalVelocity.normalized * -verticalDrag
            + horizontalVelocity.normalized * -horizontalDrag
            + adjustedSteerForce;

        return totalForce;
    }

    Vector3 GetInputDirection() {
        var cameraForward = this.camera.transform.forward;
        var cameraRight = this.camera.transform.right;
        var characterForward = new Vector3(cameraForward.x, 0.0f, cameraForward.z).normalized;
        var characterRight = new Vector3(cameraRight.x, 0.0f, cameraRight.z);

        var inputForward = movementInput.y * characterForward;
        var inputRight = movementInput.x * characterRight;

        var movementDirection = inputForward + inputRight;

        return movementDirection;
    }

    void FixedUpdate() {
        // Update our last movement direction if this new movement isn't zero length.
        var movementDirection = GetInputDirection();
        lastPositiveMovementDirection = movementDirection.magnitude < 0.01f
            ? lastPositiveMovementDirection
            : movementDirection;

        // Handle grounded state.
        this.grounded = Physics.Raycast(
            transform.position + Vector3.up,
            Vector3.down,
            out this.floorHitInfo,
            1.25f
        );

        // If we were just gliding and hit the ground, start ragdolling.
        if (movementMode == MovementMode.Glide && grounded) {
            this.movementMode = MovementMode.Ground;
            Debug.Log("GLIDE GROUND HIT");
            // this.ragdoll.SetActive(true);
        }

        // Special case transition to gliding when we're
        // falling from high enough.
        bool airToGlide = this.movementMode == MovementMode.Air
            && this.rigidBody.velocity.y < 0.0f
            && this.transform.position.y > 10.0f;

        // If we're just now entering the glide state, reset
        // glide variables.
        if (airToGlide) {
            this.glideAngle = 0.0f;
            this.glidePitch = 1.0f;
        }

        this.movementMode = airToGlide || movementMode == MovementMode.Glide
            ? MovementMode.Glide
            : this.grounded
                ? MovementMode.Ground
                : MovementMode.Air;

        // Out-of-bounds check.
        if (transform.position.y < -10.0f) {
            this.resetPlayer();
        }

        // Handle jump state.
        jumpTimer = jumping ? jumpTimer + Time.deltaTime : 0.0f;
        jumping = jumpTimer >= maxJumpTime ? false : jumping;

        // Handle movement.
        var gravityForce = Vector3.down * 9.81f;
        var totalForce = gravityForce;

        switch (movementMode) {
            case MovementMode.Ground:
                totalForce = GroundMove(totalForce);
                break;
            case MovementMode.Air:
                totalForce = AirMove(totalForce);
                break;
            case MovementMode.Glide:
                totalForce = GlideMove(totalForce);
                break;
            default: break;
        }

        rigidBody.AddForce(totalForce, ForceMode.Acceleration);
    }

    public void OnMove(InputValue value) {
        var inputDirection = value.Get<Vector2>();
        this.movementInput = inputDirection;
    }

    public void OnLook(InputValue value) {
        cameraMovementDirection = value.Get<Vector2>();
    }

    public void OnChargeLaunch() {
        chargingLaunch = grounded;
    }

    public void OnReleaseLaunch() {
        if (!chargingLaunch) {
            return;
        }

        chargingLaunch = false;
        Launch();
    }

    public void Launch() {
        var aimingDirection = this.camera.transform.forward;
        var launchForce = aimingDirection * launchHoldTimer * maxLaunchForce;
        this.rigidBody.AddForce(launchForce, ForceMode.Impulse);

        AudioSource.PlayClipAtPoint(launchSound, transform.position);

        // Pop the character off the ground slightly so we aren't
        // grounded on the next frame.
        this.transform.Translate(Vector3.up * 0.26f);

        // Launch all other objects around the character.
        var hits = Physics.SphereCastAll(transform.position, 20, transform.forward, 10.0f);
        foreach (RaycastHit hit in hits) {
            var hitRigidBody = hit.transform.gameObject.GetComponent<Rigidbody>();
            var isPlayer = hit.transform.gameObject.GetComponent<Character>() == null ? false : true;
            if (hitRigidBody != null && !isPlayer) {
                // 100 idk dawg it looks like it's a good number.
                var variationLaunch = Random.Range(0.8f, 1.2f) * 100;
                hitRigidBody.AddForce(launchForce * variationLaunch, ForceMode.Impulse);
            }
        }
    }

    public void OnStartJumping() {
        this.jumping = grounded;
    }

    public void OnStopJumping() {
        this.jumping = false;
    }

    public void resetPlayer() {
        this.ragdoll.SetActive(false);
        this.rigidBody.velocity = Vector3.zero;
        transform.position = this.spawnPosition;
    }
}
