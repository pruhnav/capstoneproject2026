using UnityEngine;
using UnityEngine.InputSystem;

public class RotateCurrentModel : MonoBehaviour
{
    [SerializeField] private InputActionProperty rightJoystick;
    [SerializeField] private float rotationSpeed = 120f;
    [SerializeField] private float deadzone = 0.1f;

    private Transform currentModel;

    void OnEnable()
    {
        rightJoystick.action.Enable();
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

        if (currentModel == null) return;

        Vector2 input = rightJoystick.action.ReadValue<Vector2>();
        float x = input.x;

        if (Mathf.Abs(x) > deadzone)
        {
            Vector3 center = GetModelCenter(currentModel);

            currentModel.RotateAround(
                center,
                Vector3.up,
                -x * rotationSpeed * Time.deltaTime
            );
        }
    }

    Vector3 GetModelCenter(Transform model)
    {
        Renderer[] renderers = model.GetComponentsInChildren<Renderer>();

        if (renderers.Length == 0)
            return model.position;

        Bounds bounds = renderers[0].bounds;

        foreach (Renderer r in renderers)
        {
            bounds.Encapsulate(r.bounds);
        }

        return bounds.center;
    }
}