using UnityEngine;
using UnityEngine.UI;

public class ARToggler : MonoBehaviour
{
    public OVRPassthroughLayer passthroughLayer;
    public Toggle arToggle;

    void Start()
    {
        // Default OFF (VR mode)
        passthroughLayer.enabled = false;

        // Listen to toggle changes
        arToggle.onValueChanged.AddListener(OnToggleChanged);
    }

    void OnToggleChanged(bool isOn)
    {
        passthroughLayer.enabled = isOn;
    }
}