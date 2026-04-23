using UnityEngine;

public class ModelMoveAndRotate : MonoBehaviour
{
    [Header("Model")]
    [SerializeField] private Transform modelToControl;
    [SerializeField] private Transform playerCamera;

    [Header("Rotation Settings")]
    [SerializeField] private float rotationSpeed = 120f;
    [SerializeField] private float rotationDeadzone = 0.15f;

    [Header("Move Settings")]
    [SerializeField] private float moveSpeed = 2.0f;
    [SerializeField] private float moveDeadzone = 0.1f;
    [SerializeField] private float minDistance = 0.3f;
    [SerializeField] private float maxDistance = 3.0f;

    void Update()
    {
        if (modelToControl == null || playerCamera == null) return;

        RotateModel();
        MoveModel();
    }

    void RotateModel()
    {
        Vector2 rightStick = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick);
        float xInput = rightStick.x;

        if (Mathf.Abs(xInput) > rotationDeadzone)
        {
            modelToControl.Rotate(
                0f,
                -xInput * rotationSpeed * Time.deltaTime,
                0f,
                Space.World
            );
        }
    }

    void MoveModel()
    {
        Vector2 leftStick = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick);
        float yInput = leftStick.y;

        if (Mathf.Abs(yInput) > moveDeadzone)
        {
            Vector3 direction = (modelToControl.position - playerCamera.position).normalized;
            float distance = Vector3.Distance(playerCamera.position, modelToControl.position);

            distance -= yInput * moveSpeed * Time.deltaTime;
            distance = Mathf.Clamp(distance, minDistance, maxDistance);

            modelToControl.position = playerCamera.position + direction * distance;
        }
    }
}