using UnityEngine;
using UnityEngine.InputSystem;

public class JoystickModelRotate : MonoBehaviour
{
    [SerializeField] private Transform modelToRotate;
    [SerializeField] private InputActionProperty rightJoystickAction;

    [SerializeField] private float rotationSpeed = 120f;
    [SerializeField] private float deadzone = 0.15f;

    void OnEnable()
    {
        rightJoystickAction.action.Enable();
    }

    void Update()
    {
        if (modelToRotate == null) return;

        Vector2 input = rightJoystickAction.action.ReadValue<Vector2>();
        float xInput = input.x;

        if (Mathf.Abs(xInput) > deadzone)
        {
            Vector3 visualCenter = GetVisualCenter(modelToRotate);

            modelToRotate.RotateAround(
                visualCenter,
                Vector3.up,
                -xInput * rotationSpeed * Time.deltaTime
            );
        }
    }

    Vector3 GetVisualCenter(Transform target)
    {
        Renderer[] renderers = target.GetComponentsInChildren<Renderer>();

        if (renderers.Length == 0)
            return target.position;

        Bounds bounds = renderers[0].bounds;

        for (int i = 1; i < renderers.Length; i++)
        {
            bounds.Encapsulate(renderers[i].bounds);
        }

        return bounds.center;
    }
}