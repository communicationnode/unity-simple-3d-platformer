using System.ComponentModel;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Camera))]
public class RaycastByCamera : MonoBehaviour
{
    //property fields
    [Header("This script require the \"Camera\" component")]
    [SerializeField] private new Camera camera;

    [Header("Raycast layers mask")]
    [SerializeField] private LayerMask layerMask;

    //values
    [SerializeField] private bool isMouseOnUI = false;
    [SerializeField] private Collider raycastedCollider;

    //methods
    private void Start() {
        TryGetComponent<Camera>(out camera);
    }
    private void Update() {
        if (camera == null) return;

        //draw ray
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 100;
        mousePos = camera.ScreenToWorldPoint(mousePos);
        Debug.DrawRay(transform.position, mousePos - transform.position, Color.blue);

        if (EventSystem.current.IsPointerOverGameObject()) {
            isMouseOnUI = true;
        } else {
            isMouseOnUI = false;
        }

        //raycast by mousePos
        if (!isMouseOnUI && Physics.Raycast(transform.position, mousePos - transform.position,out RaycastHit raycastHit ,float.MaxValue, layerMask, QueryTriggerInteraction.Ignore)) {
            raycastedCollider = raycastHit.collider;
        } else {
            raycastedCollider = null;
        }
    }
    private void OnValidate() {
        if (camera != null) return;
        if (TryGetComponent<Camera>(out camera)) {
            Debug.Log($"finded {camera}");
        }
    }
}
