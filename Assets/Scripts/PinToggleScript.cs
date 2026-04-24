using UnityEngine;

public class PinToggler : MonoBehaviour
{
    // The folder we want to hide/show
    public GameObject uiToToggle;

    public void Toggle()
    {
        // If it's off, turn it on. If it's on, turn it off.
        if (uiToToggle != null)
        {
            uiToToggle.SetActive(!uiToToggle.activeSelf);
        }
    }
}
