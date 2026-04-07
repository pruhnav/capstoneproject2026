using UnityEngine;

public class ModelSelectorRotate : MonoBehaviour
{
    [Header("Controller")]
    [SerializeField] private Transform rightController;
    [SerializeField] private float rayDistance = 10f;

    [Header("Rotation")]
    [SerializeField] private float rotationSpeed = 120f;
    [SerializeField] private float deadzone = 0.15f;

    private Transform selectedModel;

    void Update()
    {
        // Press A button to select the model you are pointing at
        if (OVRInput.GetDown(OVRInput.Button.One))
        {
            TrySelectModel();
        }

        // Rotate selected model with right thumbstick
        if (selectedModel != null)
        {
            Vector2 stickInput = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick);
            float xInput = stickInput.x;

            if (Mathf.Abs(xInput) > deadzone)
            {
                selectedModel.Rotate(0f, -xInput * rotationSpeed * Time.deltaTime, 0f, Space.Self);
            }
        }
    }

    void TrySelectModel()
    {
        Ray ray = new Ray(rightController.position, rightController.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, rayDistance))
        {
            selectedModel = hit.transform;

            Debug.Log("Selected model: " + selectedModel.name);
        }
    }
}

