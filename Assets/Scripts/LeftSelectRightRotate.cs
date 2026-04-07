using UnityEngine;

public class LeftSelectRightRotate : MonoBehaviour
{
    [Header("Controllers")]
    [SerializeField] private Transform leftController;
    [SerializeField] private float rayDistance = 50f;

    [Header("Rotation")]
    [SerializeField] private float rotationSpeed = 120f;
    [SerializeField] private float deadzone = 0.15f;

    [Header("Debug")]
    [SerializeField] private bool showDebugRay = true;

    private Transform selectedModel;

    void Update()
    {
        HandleSelection();
        HandleRotation();
    }

    void HandleSelection()
    {
        // Left index trigger pressed once
        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger))
        {
            if (leftController == null)
            {
                Debug.Log("Left controller not assigned.");
                return;
            }

            Vector3 origin = leftController.position;
            Vector3 direction = leftController.forward;

            if (showDebugRay)
            {
                Debug.DrawRay(origin, direction * rayDistance, Color.green, 2f);
            }

            RaycastHit hit;
            if (Physics.Raycast(origin, direction, out hit, rayDistance))
            {
                // Use root so whole model rotates, not just one mesh piece
                selectedModel = hit.transform.root;

                Debug.Log("Selected model: " + selectedModel.name);
            }
            else
            {
                Debug.Log("No model hit from left controller.");
            }
        }
    }

    void HandleRotation()
    {
        if (selectedModel == null) return;

        Vector2 stickInput = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick);
        float xInput = stickInput.x;

        if (Mathf.Abs(xInput) > deadzone)
        {
            selectedModel.Rotate(
                0f,
                -xInput * rotationSpeed * Time.deltaTime,
                0f,
                Space.World
            );
        }
    }
}