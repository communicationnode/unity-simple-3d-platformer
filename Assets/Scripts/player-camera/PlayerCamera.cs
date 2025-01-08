using UnityEngine;

public class PlayerCamera : MonoBehaviour {
    //property that attach camera to player
    [Header("Script search the PlatformerCharacterController")]
    [SerializeField] private Transform playerObject;

    //property for control camera
    [SerializeField] private Vector3 cameraOffset;
    [SerializeField] private float lerpValue = 0.1f;


    //find player.transform
    void Start() {
        if (playerObject != null) return;

        playerObject = FindAnyObjectByType<PlatformerCharacterController>().transform;
    }

    //move camera by FixedUpdate
    void FixedUpdate() {
        if (playerObject == null) return;

        transform.position = Vector3.Lerp(transform.position, playerObject.position + cameraOffset, lerpValue);
    }
}
