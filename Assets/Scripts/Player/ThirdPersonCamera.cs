using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour {

    [SerializeField] Transform target;
    [SerializeField] float dstFromTarget = 2;

    [SerializeField] Vector2 pitchMinMax;

    private float yaw, pitch;

	// Update is called once per frame
	void LateUpdate () {
        Vector3 targetRotation = new Vector3(pitch, yaw);
        transform.eulerAngles = targetRotation;

        transform.position = target.position - transform.forward * dstFromTarget;
	}

    public void Move (float horiz, float vert) {
        yaw += horiz;
        pitch -= vert;
        pitch = Mathf.Clamp(pitch, pitchMinMax.x, pitchMinMax.y);
    }
}
