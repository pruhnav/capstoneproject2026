using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class GlobalAnatomyManager : MonoBehaviour
{
    // This specific "SelectEnterEventArgs" version is what the VR controller needs
    public void MakeGlow(SelectEnterEventArgs args)
    {
        // This finds the specific pin the professor just clicked
        GameObject clickedObject = args.interactableObject.transform.gameObject;

        if (clickedObject != null)
        {
            var renderer = clickedObject.GetComponentInChildren<Renderer>();
            if (renderer != null)
            {
                // Forces it to stay visible through the anatomy
                renderer.material.renderQueue = 4000;
            }
        }
    }
}