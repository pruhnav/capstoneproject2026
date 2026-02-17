using UnityEngine;

public class LabelInteract : MonoBehaviour
{
    [Header("Settings")]
    public Renderer targetRenderer; // The part that changes color (The Sphere)
    public Color highlightColor = Color.yellow;
    
    private Color originalColor;

    void Start()
    {
        // Remember the starting color so we can switch back
        if (targetRenderer != null) 
            originalColor = targetRenderer.material.color;
    }

    public void OnHoverEnter()
    {
        if (targetRenderer != null) 
            targetRenderer.material.color = highlightColor;
    }

    public void OnHoverExit()
    {
        if (targetRenderer != null) 
            targetRenderer.material.color = originalColor;
    }
}
