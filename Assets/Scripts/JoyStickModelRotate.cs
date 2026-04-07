using UnityEngine;

public class JoystickModelRotate : MonoBehaviour
{
    [SerializeField] private Transform modelToRotate;
    [SerializeField] private float rotationSpeed = 120f;
    [SerializeField] private float deadzone = 0.15f;

    void Update()
    {
        Vector2 stickInput = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick);
        float xInput = stickInput.x;

        if (Mathf.Abs(xInput) > deadzone && modelToRotate != null)
        {
            modelToRotate.Rotate(0f, -xInput * rotationSpeed * Time.deltaTime, 0f, Space.Self);
        }
    }
}