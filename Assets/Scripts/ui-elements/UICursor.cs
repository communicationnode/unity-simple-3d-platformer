using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class UICursor : MonoBehaviour
{
    //property fields
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private bool isSystemCursorVisible = false;

    //methods
    void Start()
    {
        TryGetComponent<RectTransform>(out rectTransform);
        Cursor.visible = isSystemCursorVisible;
    }
    void Update()
    {
        Vector3 mousePos = Input.mousePosition;
        rectTransform.position = mousePos;
    }
    private void OnValidate() {
        if (rectTransform != null) return;
        if(TryGetComponent<RectTransform>(out rectTransform)) {
            Debug.Log($"finded {rectTransform}");
        }
        Cursor.visible = isSystemCursorVisible;
    }
}
