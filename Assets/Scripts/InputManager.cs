using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
using XInputDotNetPure;
#endif

public class InputManager : MonoBehaviour {

    [SerializeField] PlayerController playerController;
    [SerializeField] ThirdPersonCamera playerCam;

    [Header("Sticks")]
    [Range(0, 1)]
    [SerializeField] float stickDeadzone;

    [SerializeField] float rightStickSensitivity;

    bool playerIndexSet = false;
    #if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN

    PlayerIndex playerIndex;
    GamePadState state;
    GamePadState prevState;

    #endif

    // Use this for initialization
    void Start () {
        Cursor.lockState = CursorLockMode.Locked;
	}
	
	// Update is called once per frame
	void Update () {

        float horiz = KeyInputHold(KeyCode.D, KeyCode.A);
        float vert = KeyInputHold(KeyCode.W, KeyCode.S);

#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
        if (!playerIndexSet || !prevState.IsConnected)
        {
            for (int i = 0; i < 4; ++i)
            {
                PlayerIndex testPlayerIndex = (PlayerIndex)i;
                GamePadState testState = GamePad.GetState(testPlayerIndex);
                if (testState.IsConnected)
                {
                    Debug.Log(string.Format("GamePad found {0}", testPlayerIndex));
                    playerIndex = testPlayerIndex;
                    playerIndexSet = true;
                }
            }
        }

        prevState = state;
        state = GamePad.GetState(playerIndex);

        Vector2 leftStick = ApplyStickDeadzone(state.ThumbSticks.Left);
        Vector2 rightStick = ApplyStickDeadzone(state.ThumbSticks.Right);

        if (leftStick != Vector2.zero) {
            playerController.MoveAnalogue(leftStick.x, leftStick.y);
        } else {
            playerController.MoveKeyboard(horiz, vert, Input.GetKey(KeyCode.LeftShift));
        }

        if (rightStick != Vector2.zero) {
            playerCam.Move(rightStick.x, rightStick.y);
        } else {
            playerCam.Move(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        }

        float horizCam = state.ThumbSticks.Right.X != 0 ? state.ThumbSticks.Right.X * rightStickSensitivity : Input.GetAxisRaw("Mouse X");
        float vertCam = state.ThumbSticks.Right.Y != 0 ? state.ThumbSticks.Right.Y * rightStickSensitivity : Input.GetAxisRaw("Mouse Y");
        //playerCam.SetAxes(horizCam, vertCam);

        bool aimInput = state.Triggers.Left > 0.1f || Input.GetMouseButton(1);
        bool jump = prevState.Buttons.A == ButtonState.Released && state.Buttons.A == ButtonState.Pressed || Input.GetKeyDown(KeyCode.Space);

#else

        playerController.Move(horiz, vert);

        float horizCam = Input.GetAxisRaw("Mouse X");
        float vertCam = Input.GetAxisRaw("Mouse Y");
        bool aimInput = Input.GetMouseButton(1);
        bool jump = Input.GetKeyDown(KeyCode.Space);

#endif


    }

    float KeyInputHold (KeyCode posKey, KeyCode negKey) {
        float result = 0;

        if (Input.GetKey(posKey))
            result += 1;

        if (Input.GetKey(negKey))
            result -= 1;

        return result;
    }

    Vector2 ApplyStickDeadzone (GamePadThumbSticks.StickValue stick) {
        float stickX = stick.X;
        if (stickX > 0) {
            stickX = Mathf.Clamp(stickX, stickDeadzone, 1);
            stickX = ExtensionMethods.Map(stickX, stickDeadzone, 1, 0, 1);
        }
        else if (stickX < 0) {
            stickX = Mathf.Clamp(stickX, -1, -stickDeadzone);
            stickX = ExtensionMethods.Map(stickX, -1, -stickDeadzone, -1, 0);
        }
        

        float stickY = stick.Y;
        if (stickY > 0) {
            stickY = Mathf.Clamp(stickY, stickDeadzone, 1);
            stickY = ExtensionMethods.Map(stickY, stickDeadzone, 1, 0, 1);
        }
        else if (stickY < 0) {
            stickY = Mathf.Clamp(stickY, -1, -stickDeadzone);
            stickY = ExtensionMethods.Map(stickY, -1, -stickDeadzone, -1, 0);
        }

        return new Vector2(stickX, stickY);
    }
}
