using UnityEngine;

public class Billboard : MonoBehaviour
{
    void Update()
    {
        // Find the main VR camera (your head)
        if (Camera.main != null)
        {
            // Rotate the text to look at the camera
            // We use this specific math to prevent it from flipping upside down
            transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward,
                             Camera.main.transform.rotation * Vector3.up);
        }
    }
}