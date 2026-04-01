using UnityEngine;

public class MenuFollow : MonoBehaviour
{
    public Transform cameraTransform; // Drag CenterEyeAnchor here
    public float distance = 1.5f;     // How far in front of you?
    public float followSpeed = 5.0f;  // How fast does it catch up?
    void Update()
    {
        // 1. Target position is a point in front of the camera
        Vector3 targetPosition = cameraTransform.position + (cameraTransform.forward * distance);
        // 2. Smoothly move the menu to that spot
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * followSpeed);
        // 3. Make the menu face the player
        transform.LookAt(cameraTransform);
        transform.Rotate(0, 180, 0); // Flip it so the UI isn't backwards
    }
}