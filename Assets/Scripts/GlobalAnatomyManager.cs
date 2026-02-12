using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit; // This is the most important line!

public class GlobalAnatomyManager : MonoBehaviour
{
    // The "SelectEnterEventArgs" tells Unity this is for VR clicking
    public void MakeGlow(SelectEnterEventArgs args)
    {
        GameObject clickedObject = args.interactableObject.transform.gameObject;

        if (clickedObject != null)
        {
            var renderer = clickedObject.GetComponentInChildren<Renderer>();
            if (renderer != null)
            {
                // Forces the pin to be seen through the hand model
                renderer.material.renderQueue = 4000;
            }
        }
    }
}