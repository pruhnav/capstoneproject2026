using UnityEngine;
using UnityEngine.XR;
using System.Collections.Generic;

public class AButtonDebug : MonoBehaviour
{
    private InputDevice rightHandDevice;
    private bool lastAState = false;

    void Start()
    {
        FindRightHand();
    }

    void Update()
    {
        if (!rightHandDevice.isValid)
        {
            FindRightHand();
        }

        bool aPressed;
        if (rightHandDevice.TryGetFeatureValue(CommonUsages.primaryButton, out aPressed))
        {
            if (aPressed && !lastAState)
            {
                Debug.Log("A button pressed!");
            }

            lastAState = aPressed;
        }
    }

    void FindRightHand()
    {
        List<InputDevice> devices = new List<InputDevice>();
        InputDevices.GetDevicesAtXRNode(XRNode.RightHand, devices);

        if (devices.Count > 0)
        {
            rightHandDevice = devices[0];
            Debug.Log("Right hand device found: " + rightHandDevice.name);
        }
        else
        {
            Debug.Log("No right hand device found.");
        }
    }
}