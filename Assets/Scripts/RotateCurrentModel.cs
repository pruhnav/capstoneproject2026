using UnityEngine;
using UnityEngine.InputSystem;

public class RotateCurrentModel : MonoBehaviour
{
    [Header("Joystick Input")]
    [SerializeField] private InputActionProperty rightJoystick;

    [Header("Rotation")]
    [SerializeField] private float rotationSpeed = 120f;
    [SerializeField] private float deadzone = 0.1f;

    private Transform currentModel;

    void OnEnable()
    {
        rightJoystick.action.Enable();
    }

    void Update()
    {
        // Step 1: find model automatically
        if (currentModel == null)
        {
            GameObject anchor = GameObject.Find("ModelAnchor");
            if (anchor != null && anchor.transform.childCount > 0)
            {
                currentModel = anchor.transform.GetChild(0);
            }
        }

        if (currentModel == null) return;

        // Step 2: read joystick
        Vector2 input = rightJoystick.action.ReadValue<Vector2>();
        float x = input.x;

        // Step 3: rotate
        if (Mathf.Abs(x) > deadzone)
        {
            currentModel.Rotate(
                0f,
                -x * rotationSpeed * Time.deltaTime,
                0f,
                Space.Self
            );
        }
    }
}