using UnityEngine;

public class DriveButtons : MonoBehaviour
{
    public void OpenGoogleDrive()
    {
        Application.OpenURL("https://drive.google.com");
    }

    public void OpenOneDrive()
    {
        Application.OpenURL("https://onedrive.live.com");
    }
}

