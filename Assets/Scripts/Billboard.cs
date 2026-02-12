using UnityEngine;

public class Billboard : MonoBehaviour
{
    void Update()
    {
        // Find the main VR camera
        if (Camera.main != null)
        {
            // Rotate ONLY the text to look at the player
            transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward,
                             Camera.main.transform.rotation * Vector3.up);
        }
    }
}