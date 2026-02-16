using UnityEngine;
using UnityEngine.InputSystem; // Needed for VR buttons
using TMPro; // Needed for text

public class LabelController : MonoBehaviour
{
    [Header("Setup")]
    public GameObject labelPrefab; // Drag your Blue SRS_Label here
    public InputActionProperty createAction; // The trigger button
    public LayerMask targetLayer; // What layers can we click on?

    void Update()
    {
        if (createAction.action != null && createAction.action.WasPressedThisFrame())
        {
            ShootRay();
        }
    }

    void ShootRay()
    {
        // Create a ray starting from the controller, pointing forward
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        // Shoot the ray. "10f" is the max distance.
        if (Physics.Raycast(ray, out hit, 10f, targetLayer))
        {
            CreateLabel(hit);
        }
    }

    void CreateLabel(RaycastHit hit)
    {
        GameObject newLabel = Instantiate(labelPrefab, hit.point, Quaternion.identity);
        newLabel.transform.up = hit.normal;
        newLabel.transform.SetParent(hit.transform);
        TMP_InputField input = newLabel.GetComponentInChildren<TMP_InputField>();
        if (input != null)
        {
            input.Select();
            input.ActivateInputField();
        }
    }
}
