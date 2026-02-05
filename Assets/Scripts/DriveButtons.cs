using UnityEngine;

public class DriveButtons : MonoBehaviour
{
    [Header("Hard-coded model links")]
    public string googleDriveModelLink =
        "https://drive.google.com/uc?id=FILE_ID&export=download";

    public string oneDriveModelLink =
        "https://pennstateoffice365-my.sharepoint.com/:f:/g/personal/lvg5541_psu_edu/IgB4BhIq0yw6Q59GORY2Hyc0AbWbZ-pRHtShUZbTfUK0Odg?e=6Ha0ue";

    public CloudModelLoader modelLoader;

    public void LoadFromGoogleDrive()
    {
        Debug.Log("Loading model from Google Drive");
        modelLoader.LoadFromLink(googleDriveModelLink);
    }

    public void LoadFromOneDrive()
    {
        Debug.Log("Loading model from OneDrive");
        modelLoader.LoadFromLink(oneDriveModelLink);
    }
}

