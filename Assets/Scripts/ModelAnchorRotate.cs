using UnityEngine;

public class ModelAnchorRotate: MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 120f;
    [SerializeField] private bool useRightThumbstick = true;
    [SerializeField] private float deadzone = 0.15f;

    void Update()
    {
        Vector2 stickInput;

        if (useRightThumbstick)
            stickInput = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick);
        else
            stickInput = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick);

        float xInput = stickInput.x;

        if (Mathf.Abs(xInput) > deadzone)
        {
            transform.Rotate(0f, -xInput * rotationSpeed * Time.deltaTime, 0f, Space.World);
        }
    }
}
