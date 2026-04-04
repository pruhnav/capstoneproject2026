using UnityEngine;
public class PassthroughToggle : MonoBehaviour
{
[Header("Setup")]
public OVRPassthroughLayer passthroughLayer;
public Camera mainCamera;
[Header("VR Background Color")]
// This is the color the room goes back to when AR is turned off.
// Usually a dark grey looks good for VR.
public Color vrBackgroundColor = new Color(0.1f, 0.1f, 0.1f, 1f);
// Pure transparent black for the AR mode
private Color arTransparentColor = new Color(0f, 0f, 0f, 0f);
// Let's assume you start in VR mode (AR is off)
private bool isPassthroughOn = false;
void Start()
{
// Ensure we start with Passthrough hidden and VR background on
passthroughLayer.hidden = true;
mainCamera.backgroundColor = vrBackgroundColor;
}
// You will link your UI Button's "On Click()" event to this function
public void ToggleAR()
{
isPassthroughOn = !isPassthroughOn;
if (isPassthroughOn)
{
// Turn ON the real world
passthroughLayer.hidden = false;
mainCamera.backgroundColor = arTransparentColor;
Debug.Log("AR Mode Activated");
}
else
{
// Turn OFF the real world
passthroughLayer.hidden = true;
mainCamera.backgroundColor = vrBackgroundColor;
Debug.Log("VR Mode Activated");
}
}
}
