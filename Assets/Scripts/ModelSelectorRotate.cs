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
        if (OVRInput.GetDown(OVRInput.Button.One))
        {
            Debug.Log("A button pressed");
            TrySelectModel();
        }

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
        Vector3 origin = rightController.position;
        Vector3 direction = rightController.TransformDirection(Vector3.forward);

        Ray ray = new Ray(origin, direction);
        Debug.DrawRay(origin, direction * rayDistance, Color.red, 2f);

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, rayDistance))
        {
            Debug.Log("Ray hit: " + hit.transform.name);
            selectedModel = hit.transform;
            Debug.Log("Selected model: " + selectedModel.name);
        }
        else
        {
            Debug.Log("Ray hit nothing");
        }
    }
}


