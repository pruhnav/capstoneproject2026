using UnityEngine;
using UnityEngine.InputSystem;

public class PullwithJoystick : MonoBehaviour
{
    public InputActionProperty leftJoystickAction;
    public Transform playerCamera;
    public float moveSpeed = 1.0f;
    public float minDistance = 0.25f;
    public float maxDistance = 3.0f;

    void Update()
    {
        Vector2 input = leftJoystickAction.action.ReadValue<Vector2>();
        float y = input.y;

        if (Mathf.Abs(y) > 0.1f)
        {
            Vector3 dir = (transform.position - playerCamera.position).normalized;
            float dist = Vector3.Distance(playerCamera.position, transform.position);

            dist -= y * moveSpeed * Time.deltaTime;
            dist = Mathf.Clamp(dist, minDistance, maxDistance);

            transform.position = playerCamera.position + dir * dist;
        }
    }
}