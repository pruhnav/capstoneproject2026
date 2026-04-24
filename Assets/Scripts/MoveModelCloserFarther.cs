using UnityEngine;
using UnityEngine.InputSystem;

public class MoveModelCloserFarther : MonoBehaviour
{
    [Header("Left Joystick Input")]
    [SerializeField] private InputActionProperty leftJoystick;

    [Header("Camera")]
    [SerializeField] private Transform playerCamera;

    [Header("Move Settings")]
    [SerializeField] private float moveSpeed = 2.0f;
    [SerializeField] private float deadzone = 0.1f;
    [SerializeField] private float minDistance = 0.3f;
    [SerializeField] private float maxDistance = 3.0f;

    private Transform currentModel;

    void OnEnable()
    {
        leftJoystick.action.Enable();
    }

    void Update()
    {
        if (currentModel == null)
        {
            GameObject anchor = GameObject.Find("ModelAnchor");

            if (anchor != null && anchor.transform.childCount > 0)
            {
                currentModel = anchor.transform.GetChild(0);
            }
        }

        if (currentModel == null || playerCamera == null) return;

        Vector2 input = leftJoystick.action.ReadValue<Vector2>();
        float y = input.y;

        if (Mathf.Abs(y) > deadzone)
        {
            Vector3 direction = (currentModel.position - playerCamera.position).normalized;
            float distance = Vector3.Distance(playerCamera.position, currentModel.position);

            distance += y * moveSpeed * Time.deltaTime;
            distance = Mathf.Clamp(distance, minDistance, maxDistance);

            currentModel.position = playerCamera.position + direction * distance;
        }
    }
}