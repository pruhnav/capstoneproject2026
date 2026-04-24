using UnityEngine;
using UnityEngine.InputSystem;

public class JoystickModelRotate : MonoBehaviour
{
    [SerializeField] private Transform modelToRotate;
    [SerializeField] private InputActionProperty rightJoystickAction;

    [SerializeField] private float rotationSpeed = 120f;
    [SerializeField] private float deadzone = 0.15f;

    void OnEnable()
    {
        rightJoystickAction.action.Enable();
    }

    void Update()
    {
        if (modelToRotate == null) return;

        Vector2 input = rightJoystickAction.action.ReadValue<Vector2>();
        float xInput = input.x;

        Debug.Log("X Input: " + xInput);

        if (Mathf.Abs(xInput) > deadzone)
        {
            modelToRotate.Rotate(
                0f,
                -xInput * rotationSpeed * Time.deltaTime,
                0f,
                Space.Self
            );
        }
    }
}