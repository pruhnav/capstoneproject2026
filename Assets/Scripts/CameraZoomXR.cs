using UnityEngine;
using UnityEngine.XR;

public class CameraZoomXR : MonoBehaviour
{
    public float zoomSpeed = 1.5f;
    public float minDistance = 0.5f;
    public float maxDistance = 5f;

    private Transform cam;
    private InputDevice leftHand;
    private InputDevice rightHand;

    void Start()
    {
        cam = Camera.main.transform;

        leftHand = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
        rightHand = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
    }

    void Update()
    {
        Vector2 leftStick;
        Vector2 rightStick;

        float zoomInput = 0f;

        if (leftHand.TryGetFeatureValue(CommonUsages.primary2DAxis, out leftStick))
            zoomInput += leftStick.y;

        if (rightHand.TryGetFeatureValue(CommonUsages.primary2DAxis, out rightStick))
            zoomInput += rightStick.y;

        if (Mathf.Abs(zoomInput) > 0.1f)
        {
            Vector3 direction = cam.forward;
            transform.position += direction * zoomInput * zoomSpeed * Time.deltaTime;
        }
    }
}
