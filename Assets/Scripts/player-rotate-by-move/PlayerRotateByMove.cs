using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRotateByMove : MonoBehaviour {
    [SerializeField] private PlatformerCharacterController characterController;
    [SerializeField] private float lerpValue = 0.08f;
    private Vector3 oldPosition;
    private Vector3 rawDirection;
    private Vector3 lerpedDirection;

    private Type lastMovementState;

    private void Start() {
        oldPosition = transform.position;
    }
    private void FixedUpdate() {

        //initialize character controller direction as local value
        Vector2 sharedDirection = characterController.sharedMovementStatesManager.GetCurrentState().sharedProperties.sharedDirection;

        //if direction size greater then 0.8f -> write to rawDirection
        if (Mathf.Abs(sharedDirection.x) > 0.8f) {
            rawDirection = new Vector3(sharedDirection.x, 0,transform.position.z);
        }

        //if current state is <type> -> return
        if (characterController.sharedMovementStatesManager.GetCurrentState().GetType() == typeof(GrabStepMovementState)) {
            return;
        }
        else if (characterController.sharedMovementStatesManager.GetCurrentState().GetType() == typeof(DriveUpMovementState)) {
            return;
        }

        //apply rotation from rawDirection
        lerpedDirection = Vector3.Lerp(lerpedDirection, new Vector3(rawDirection.x, 0, rawDirection.z + (Mathf.Abs(rawDirection.x / 8) * -1f)), lerpValue);
        transform.LookAt(transform.position + lerpedDirection, Vector3.up);
    }
}
