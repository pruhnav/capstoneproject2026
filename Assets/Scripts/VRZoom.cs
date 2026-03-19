using UnityEngine;
using UnityEngine.XR;

public class VRZoom : MonoBehaviour
{
    public float zoomSpeed = 2f;

    void Update()
    {
        InputDevice rightHand = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);

        if (rightHand.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 joystick))
        {
            Vector3 move = transform.forward * joystick.y * zoomSpeed * Time.deltaTime;
            transform.parent.position += move;
        }
    }
}