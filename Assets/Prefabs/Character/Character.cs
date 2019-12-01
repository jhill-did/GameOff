using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum MovementMode { Ground, Air, Glide };

public class Character : MonoBehaviour {
    public Rigidbody rigidBody;
    public new GameObject camera;
    public GameObject cameraArm;
    public GameObject cameraBase;
    private float cameraYaw = 0.0f;
    public float rotationSpeed = 5.0f;

    MovementMode movementMode;

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

        // Code for levitating surrounding objects
        if (chargingLaunch)
        {
            var hits = Physics.SphereCastAll(transform.position, 20, transform.forward, 10.0f);
            foreach(RaycastHit hit in hits) {
                Debug.Log(hit.transform.name);
                var hitRigidBody = hit.transform.gameObject.GetComponent<Rigidbody>();
                var isPlayer = hit.transform.gameObject.GetComponent<Character>() == null ? false : true;
                if(hitRigidBody != null && !isPlayer)
                {
                    hitRigidBody.AddForce(Vector3.up * 1000 * Time.deltaTime, ForceMode.Acceleration);
                }
            }
        }
    }

    void TurnTowards(Vector3 direction) {
        var targetRotation = Quaternion.LookRotation(direction);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            Time.fixedDeltaTime * rotationSpeed
        );
    }

    Vector3 GroundMove(Vector3 currentForce) {
        var movementDirection = GetInputDirection();

        // Turn to face our movementDirection.
        if (this.lastPositiveMovementDirection.magnitude > 0.0f) {
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
        // Rotate towards our movement direction.
        var horizontalVelocity = this.rigidBody
            .velocity
            .getHorizontalPart();

        if (horizontalVelocity.magnitude > 0.0f) {
            TurnTowards(horizontalVelocity);
        }

        var inputDirection = GetInputDirection();
        this.glideAngle = Mathf.Clamp(glideAngle + inputDirection.x, -1.0f, 1.0f);

        var glideAngleReset = (glideAngle > 0.0f ? -1.0f : 1.0f)
            * Time.fixedDeltaTime
            * 4.0f;

        this.glideAngle = glideAngle + glideAngleReset;

        Debug.Log(glideAngle);


        var totalForce = currentForce
            + Vector3.up * 9.86f;

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

        this.movementMode = airToGlide
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
        chargingLaunch = true;
    }

    public void OnReleaseLaunch() {
        chargingLaunch = false;
        Launch();
    }

    public void Launch() {
        var aimingDirection = this.camera.transform.forward;
        var launchForce = aimingDirection * launchHoldTimer * maxLaunchForce;
        this.rigidBody.AddForce(launchForce, ForceMode.Impulse);

        // Pop the character off the ground slightly so we aren't
        // grounded on the next frame.
        this.transform.Translate(Vector3.up * 0.26f);


        // launch all other objects around the character
        var hits = Physics.SphereCastAll(transform.position, 20, transform.forward, 10.0f);
        foreach (RaycastHit hit in hits)
        {
            Debug.Log(hit.transform.name);
            var hitRigidBody = hit.transform.gameObject.GetComponent<Rigidbody>();
            var isPlayer = hit.transform.gameObject.GetComponent<Character>() == null ? false : true;
            if (hitRigidBody != null && !isPlayer)
            {
                //100 idk dawg it looks like it's a good number.
                hitRigidBody.AddForce(launchForce * 100, ForceMode.Impulse);
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
        this.rigidBody.velocity = Vector3.zero;
        transform.position = this.spawnPosition;
    }
}
