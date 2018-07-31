using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    [Header("Speed")]
    [SerializeField] float minSpeed = 1;
    [SerializeField] float sneakSpeed = 2;
    [SerializeField] float walkSpeed = 4;
    [SerializeField] float runSpeed = 6;

    [Header("Acceleration")]
    [SerializeField] float accelTime = 0.1f;
    [SerializeField] float decelTime = 0.1f;
    [SerializeField] float runAccelTime = 0.3f;
    [SerializeField] float runDecelTime = 0.1f;

    [Header("Run Cancel")]
    [SerializeField] float runAngleCheckTime = 0.2f;
    [SerializeField] float runAngleCancelRange = 60;

    [Header("Other")]
    [SerializeField] Camera playerCam;

    private bool isMoving = false;
    private bool runCancel = false;

    private Vector3 moveDir, velocity, currentVelocity;
    private float currentSpeed = 0;

    private float runAngleTimer, maxRunAngle;
    private Vector3 initRunDir;

    private CharacterController charController;

    void Awake() {
        charController = GetComponent<CharacterController>();
    }

    void Update() {
        if (currentSpeed == runSpeed && !runCancel) {
            float thisRunAngle = Vector3.Angle(initRunDir, moveDir);
            if (thisRunAngle > maxRunAngle)
                maxRunAngle = thisRunAngle;

            if (runAngleTimer < runAngleCheckTime) {
                runAngleTimer += Time.deltaTime;
            }
            else {
                if (maxRunAngle > 180 - (runAngleCancelRange / 2f)) {
                    runCancel = true;
                }

                runAngleTimer = 0;
                maxRunAngle = 0;
                initRunDir = moveDir;
            }
        }

        charController.Move(velocity * Time.deltaTime);
    }

    private void LateUpdate() {
        float smoothTime;
        Vector3 targetVel = moveDir * currentSpeed;
        if (velocity.magnitude > targetVel.magnitude) {
            smoothTime = decelTime;
        } else {
            if (currentSpeed == runSpeed) {
                smoothTime = runAccelTime;
            } else {
                smoothTime = accelTime;
            }
        }

        velocity = Vector3.SmoothDamp(velocity, moveDir * currentSpeed, ref currentVelocity, smoothTime);

        if (velocity != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(velocity, Vector3.up);

    }

    // Run this in update
    public void MoveKeyboard(float horiz, float vert, bool isRunning, bool isSneaking) {
        float speed;
        if (isRunning) {
            speed = runSpeed;
        } else if (isSneaking) {
            speed = sneakSpeed;
        } else {
            speed = walkSpeed;
        }

        Move(horiz, vert, speed);
    }

    public void MoveAnalogue (float horiz, float vert, bool isRunning, ref bool _runCancel) {
        float speed;
        if (isRunning) {
            speed = runSpeed;
        } else {
            // Map and clamp creates deadzones where the character is sneaking and running at a constant speed
            float mag = ExtensionMethods.Map(new Vector2(horiz, vert).magnitude, 0, 1, -0.1f, 1.1f);

            speed = mag * walkSpeed;
            speed = Mathf.Clamp(speed, minSpeed, walkSpeed);
        }

        Move(horiz, vert, speed);

        _runCancel = runCancel;
    }

    private void Move (float horiz, float vert, float speed) {
        Vector3 input = new Vector3(horiz, 0, vert).normalized;

        isMoving = input != Vector3.zero;
        if (isMoving) {
            //transform.eulerAngles = Vector3.up * Mathf.Atan2(inputDir.x, inputDir.y) * Mathf.Rad2Deg;
            moveDir = Quaternion.Euler(0, playerCam.transform.rotation.eulerAngles.y, 0) * input;
            //transform.forward = moveDir;

            if (speed != runSpeed) {
                runCancel = false;
                initRunDir = moveDir;
            }

            if (runCancel && speed == runSpeed) {
                currentSpeed = walkSpeed;
            }
            else {
                currentSpeed = speed;
            }

        } else {
            currentSpeed = 0;
        }
    }
}
