using UnityEngine;

public class AutoRotateAnyModel : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 120f;
    [SerializeField] private float deadzone = 0.15f;

    private Transform currentModel;

    void Update()
    {
        if (currentModel == null)
        {
            FindModel();
            return;
        }

        Vector2 rightStick = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick);
        float xInput = rightStick.x;

        if (Mathf.Abs(xInput) > deadzone)
        {
            currentModel.Rotate(
                0f,
                -xInput * rotationSpeed * Time.deltaTime,
                0f,
                Space.Self
            );
        }
    }

    void FindModel()
    {
        GameObject modelAnchor = GameObject.Find("ModelAnchor");

        if (modelAnchor == null) return;

        if (modelAnchor.transform.childCount > 0)
        {
            currentModel = modelAnchor.transform.GetChild(0);
            Debug.Log("Rotating model: " + currentModel.name);
        }
    }
}