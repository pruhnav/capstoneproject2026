using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class DeletePin : MonoBehaviour
{
    [Header("Setup")]
    public InputActionProperty deleteAction;
    public XRRayInteractor leftLaser;

    void Update()
    {
        // Listens for the Grip Button
        if (deleteAction.action != null && deleteAction.action.WasPressedThisFrame())
        {
            CheckLaserTarget();
        }
    }

    void CheckLaserTarget()
    {
        if (leftLaser.TryGetCurrentUIRaycastResult(out var uiHit))
        {
            DestroyPin(uiHit.gameObject);
        }
        else if (leftLaser.TryGetCurrent3DRaycastHit(out RaycastHit hit))
        {
            DestroyPin(hit.collider.gameObject);
        }
    }

    void DestroyPin(GameObject highlightedObject)
    {
        // SAFETY LOCK: Never delete the player's body/controllers
        if (highlightedObject.transform.IsChildOf(this.transform) || highlightedObject.transform == this.transform)
        {
            return;
        }

        Transform current = highlightedObject.transform;

        while (current != null)
        {
            // THE FIX: We don't care about tags anymore. 
            // We just look for the exact name of your prefab (or its Clone).
            if (current.name.Contains("SRS_Label") || current.CompareTag("Label"))
            {
                Destroy(current.gameObject);
                return;
            }
            current = current.parent;
        }
    }
}