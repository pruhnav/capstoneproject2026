using UnityEngine;
using SimpleFileBrowser;

public class OBJLoaderUI : MonoBehaviour
{
    public void OpenFileBrowser()
    {
        FileBrowser.SetFilters(true, new FileBrowser.Filter("OBJ Models", ".obj"));
        FileBrowser.ShowLoadDialog(OnFileSelected, null, FileBrowser.PickMode.Files);
    }

    void OnFileSelected(string[] paths)
    {
        string objPath = paths[0];
        LoadOBJ(objPath);
    }
    void LoadOBJ(string path)
    {
        GameObject obj = SimpleOBJLoader.LoadOBJ(path);
        obj.transform.position = Vector3.zero;
    }

}
