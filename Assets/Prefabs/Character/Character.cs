using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Character : MonoBehaviour {
    public Rigidbody rigidBody;
    public new GameObject camera;
    public GameObject cameraArm;
    public GameObject cameraBase;
    private float cameraYaw = 0.0f;
    public float rotationSpeed = 5.0f;

    bool chargingLaunch = false;
    public float launchHoldTimer = 0.0f;
    public float launchHoldRate = 1.0f;
    public float maxLaunchForce;

    public bool jumping = false;
    public float jumpTimer = 0.0f;
    public float maxJumpTime = 1.0f;
    public float jumpAcceleration;
    public bool grounded;

    private Vector3 lastPositiveMovementDirection = Vector3.zero;
    private Vector2 movementInput = Vector2.zero;
    public float groundAcceleration;
    public float airAcceleration;
    public float inputAirVelocity;
    public float groundFriction;
    public float maxGroundVelocity;

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
    }

    void FixedUpdate() {
        // Handle grounded state.
        this.grounded = Physics.Raycast(
            transform.position + Vector3.up,
            Vector3.down,
            out var hitInfo,
            1.25f
        );

        // Calculate our movement forces.
        var cameraForward = this.camera.transform.forward;
        var cameraRight = this.camera.transform.right;
        var characterForward = new Vector3(cameraForward.x, 0.0f, cameraForward.z).normalized;
        var characterRight = new Vector3(cameraRight.x, 0.0f, cameraRight.z);

        var inputForward = movementInput.y * characterForward;
        var inputRight = movementInput.x * characterRight;

        var movementDirection = inputForward + inputRight;

        // Update our last movement direction if this new movement isn't zero length.
        lastPositiveMovementDirection = movementDirection.magnitude < 0.01f
            ? lastPositiveMovementDirection
            : movementDirection;

        // Turn to face our movementDirection.
        if (this.lastPositiveMovementDirection.magnitude > 0.0f)
        {
            var targetRotation = Quaternion
                .LookRotation(this.lastPositiveMovementDirection);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                Time.fixedDeltaTime * rotationSpeed
            );
        }

        // Handle jump state.
        jumpTimer = jumping ? jumpTimer + Time.deltaTime : 0.0f;
        jumping = jumpTimer >= maxJumpTime ? false : jumping;

        // Handle movement.
        var gravityForce = Vector3.down * 9.81f;
        var totalForce = gravityForce;

        if (grounded) {
            // Handle Jumping.
            var jumpForce = jumping
                ? Vector3.up * jumpAcceleration
                : Vector3.zero;

            // Handle floor magnetism.
            var floorMagnetForce = this.grounded
                ? hitInfo.normal * -50.0f
                : Vector3.zero;

            // Apply ground friction.
            var velocityDirection = this.rigidBody.velocity.normalized;
            var angleDot = Vector3.Dot(hitInfo.normal, Vector3.up);
            var normalForce = 1.0f * 9.81f * angleDot;
            var frictionForce = -velocityDirection * this.groundFriction * normalForce;

            totalForce += movementDirection * groundAcceleration
                + jumpForce
                + frictionForce;
            rigidBody.AddForce(totalForce, ForceMode.Acceleration);

            // Clamp velocity.
            this.rigidBody.velocity = Vector3
                .ClampMagnitude(this.rigidBody.velocity, maxGroundVelocity);
        }

        if (!grounded) {
            totalForce += movementDirection * airAcceleration;
            rigidBody.AddForce(totalForce, ForceMode.Acceleration);
        }

        if(transform.position.y < -15)
        {
            this.resetPlayer();
        }
    }

    public void OnMove(InputValue value) {
        var inputDirection = value.Get<Vector2>();
        this.movementInput = inputDirection;
    }

    public void OnLook(InputValue value) {
        cameraMovementDirection = value.Get<Vector2>();
    }

    public void OnAttack() {
        // Debug.Log("Hello Attack");
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
    }

    public void OnStartJumping() {
        this.jumping = grounded;
    }

    public void OnStopJumping() {
        this.jumping = false;
    }

    public void resetPlayer()
    {
        this.rigidBody.velocity = Vector3.zero;
        transform.position = this.spawnPosition;
    }
}
