using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    [Header("Speed")]
    [SerializeField] float sneakSpeed = 1;
    [SerializeField] float walkSpeed = 2;
    [SerializeField] float runSpeed = 6;

    [Header("Other")]
    [SerializeField] Camera playerCam;

    private Vector3 moveDir;
    private float currentSpeed = 0;

    private CharacterController charController;

    void Awake() {
        charController = GetComponent<CharacterController>();
    }

    void Update() {
        charController.Move(moveDir * currentSpeed * Time.deltaTime);
    }

    // Run this in update
    public void MoveKeyboard(float horiz, float vert, bool isRunning) {
        float speed = isRunning ? runSpeed : walkSpeed;
        Move(horiz, vert, speed);
    }

    public void MoveAnalogue (float horiz, float vert) {
        // Map and clamp creates deadzones where the character is sneaking and running at a constant speed
        float mag = ExtensionMethods.Map(new Vector2(horiz, vert).magnitude, 0, 1, -0.1f, 1.1f);

        float speed = mag * walkSpeed;
        speed = Mathf.Clamp(speed, sneakSpeed, walkSpeed);

        Move(horiz, vert, speed);
    }

    private void Move (float horiz, float vert, float speed) {
        Vector3 input = new Vector3(horiz, 0, vert).normalized;

        if (input != Vector3.zero) {
            //transform.eulerAngles = Vector3.up * Mathf.Atan2(inputDir.x, inputDir.y) * Mathf.Rad2Deg;
            moveDir = Quaternion.Euler(0, playerCam.transform.rotation.eulerAngles.y, 0) * input;
            //transform.forward = moveDir;

            currentSpeed = speed;
        } else {
            currentSpeed = 0;
        }
    }
}
