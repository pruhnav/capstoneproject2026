using UnityEngine;
using TMPro;

public class GlobalAnatomyManager : MonoBehaviour
{
    void Update()
    {
        // 1. Detect what the professor just clicked in VR
        GameObject clickedObject = UnityEditor.Selection.activeGameObject;

        if (clickedObject != null)
        {
            // 2. See if what he clicked has text (a label)
            var renderer = clickedObject.GetComponentInChildren<Renderer>();
            
            if (renderer != null)
            {
                // 3. Make it glow through the hand bones automatically
                // 4000 is the "X-Ray" layer that sees through everything
                renderer.material.renderQueue = 4000;
            }
        }
    }
}