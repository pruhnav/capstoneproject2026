using UnityEngine;

public class LabelVisibility : MonoBehaviour
{
    public CanvasGroup canvasGroup; // Drag your Label's CanvasGroup here
    public float fadeSpeed = 5f;
    
    private Transform camTransform;

    void Start()
    {
        camTransform = Camera.main.transform;
        if (canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>();
    }

    void Update()
    {
        // 1. Get the direction from the label to the camera
        Vector3 directionToCamera = (camTransform.position - transform.position).normalized;

        // 2. Calculate Dot Product
        // transform.forward is the direction the label is "facing"
        float dot = Vector3.Dot(transform.forward, directionToCamera);

        // 3. If dot > 0, the camera is in front of the label.
        // We use a small threshold (like 0.2) so it doesn't flicker at the edges.
        float targetAlpha = (dot > 0.2f) ? 1.0f : 0.0f;

        // 4. Smoothly fade the label in or out
        canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, targetAlpha, Time.deltaTime * fadeSpeed);
    }
}