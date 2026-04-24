using UnityEngine;
using Dummiesman;
using System.IO;

public class LocalOBJLoader : MonoBehaviour
{
    public Transform modelAnchor;

    private string folderPath = "/storage/emulated/0/Download/SavedModel/";

    public void LoadFirstOBJ()
    {
        if (!Directory.Exists(folderPath))
        {
            Debug.LogError("Folder not found: " + folderPath);
            return;
        }

        string[] objFiles = Directory.GetFiles(folderPath, "*.obj");

        if (objFiles.Length == 0)
        {
            Debug.LogError("No OBJ files found.");
            return;
        }

        LoadOBJ(objFiles[0]);
    }

    private void LoadOBJ(string path)
    {
        foreach (Transform child in modelAnchor)
        {
            Destroy(child.gameObject);
        }

        GameObject model = new OBJLoader().Load(path);
        model.name = Path.GetFileNameWithoutExtension(path);

        model.transform.SetParent(modelAnchor);
        model.transform.localPosition = Vector3.zero;
        model.transform.localRotation = Quaternion.identity;
        model.transform.localScale = Vector3.one;

        Debug.Log("Loaded: " + model.name);
    }
}
