using UnityEngine;
using System.Collections;

public class VRZoom : MonoBehaviour
{
    public float zoomSpeed = 2f;
    public float minZoom = 0.5f;
    public float maxZoom = 2f;

    private Camera cam;
    private float initialFov;

    void Start()
    {
        cam = GetComponent<Camera>();
        initialFov = cam.fieldOfView;
    }

    void Update()
    {
        Vector2 joystick = OVRInput.Get(OVRInput.Axis2D.PrimaryPrimaryThumbstick, OVRInput.Controller.RTouch);

        Vector3 move = transform.forward * joystick.y * zoomSpeed * Time.deltaTime;
        transform.parent.position += move;

    }
}