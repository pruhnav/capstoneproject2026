using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class FoveationControl : MonoBehaviour
{
    XRDisplaySubsystem displaySubsystem;

    IEnumerator Start()
    {
        for (int i = 0; i < 3; i++)
        {
            yield return null;
        }

        List<XRDisplaySubsystem> displaySubsystems = new List<XRDisplaySubsystem>();
        SubsystemManager.GetSubsystems(displaySubsystems);

        if (displaySubsystems.Count > 0)
        {
            displaySubsystem = displaySubsystems[0];

            displaySubsystem.foveatedRenderingLevel = 0.0f;
        }
    }

    public void SetFoveationLevel(float level)
    {
        if (displaySubsystem != null)
        {
            displaySubsystem.foveatedRenderingLevel = Mathf.Clamp01(level);
        }
    }
}