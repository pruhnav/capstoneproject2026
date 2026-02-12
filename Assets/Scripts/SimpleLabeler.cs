using UnityEngine;
using UnityEngine.InputSystem;

public class SimpleLabeler : MonoBehaviour
{
    [Header("Settings")]
    public GameObject labelPrefab;
    public InputActionProperty triggerAction;

    // Internal flag to track if we are holding the button
    private bool _isPressed = false;

    void Update()
    {
        // 1. Get the raw value of the trigger (0.0 to 1.0)
        float triggerValue = triggerAction.action.ReadValue<float>();

        // 2. FIRE LOGIC: If pulled down hard (> 0.9) AND we aren't already holding it
        if (triggerValue > 0.9f && !_isPressed)
        {
            ShootRay();     // Fire once!
            _isPressed = true; // Lock it so we don't machine-gun fire
        }

        // 3. RESET LOGIC: If released (< 0.1), unlock it for the next click
        else if (triggerValue < 0.1f)
        {
            _isPressed = false;
        }
    }

    void ShootRay()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            Debug.Log("Hit: " + hit.collider.name);

            GameObject newLabel = Instantiate(labelPrefab, hit.point, Quaternion.identity);

            // Align with surface normal
            newLabel.transform.rotation = Quaternion.LookRotation(hit.normal);

            // Flip it 180 degrees (Your fix)
            newLabel.transform.Rotate(0, 180, 0);

            newLabel.transform.SetParent(hit.transform);
        }
    }
}