using UnityEngine;

public class JoystickModelRotate : MonoBehaviour
{
    [SerializeField] private Transform modelToRotate;
    [SerializeField] private float rotationSpeed = 120f;
    [SerializeField] private float deadzone = 0.15f;

    void Update()
    {
        if (modelToRotate == null)
        {
            Debug.LogWarning("Model To Rotate is not assigned.");
            return;
        }

        Vector2 rightStick = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick);
        float xInput = rightStick.x;

        if (Mathf.Abs(xInput) > deadzone)
        {
            // Rotate around its OWN CENTER (Earth-like rotation)
            modelToRotate.RotateAround(
                modelToRotate.position,   // pivot point (center)
                Vector3.up,               // Y-axis (vertical)
                -xInput * rotationSpeed * Time.deltaTime
            );
        }
    }
}