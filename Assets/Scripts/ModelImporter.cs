using UnityEngine;
using System.Collections;
using SimpleFileBrowser;
public class ModelImporter : MonoBehaviour
{
    // This MUST be public and parameterless
    public void OnImportButtonPressed()
    {
        StartCoroutine(OpenFileBrowser());
    }

    IEnumerator OpenFileBrowser()
    {
        FileBrowser.SetFilters(true, new FileBrowser.Filter("OBJ Files", ".obj"));
        FileBrowser.SetDefaultFilter(".obj");

        yield return FileBrowser.WaitForLoadDialog(
            FileBrowser.PickMode.Files,
            false,
            null,
            null,
            "Select OBJ Model",
            "Import"
        );

        if (FileBrowser.Success)
        {
            string objPath = FileBrowser.Result[0];
            LoadOBJ(objPath);
        }
    }

    void LoadOBJ(string path)
    {
        // Dummiesman loader automatically loads:
        // OBJ + MTL + textures (if in same folder)
        GameObject model = new OBJLoader().Load(path);

        model.transform.position = Camera.main.transform.position + Camera.main.transform.forward * 1.5f;
        model.transform.rotation = Quaternion.identity;
        model.transform.localScale = Vector3.one;
    }
}
