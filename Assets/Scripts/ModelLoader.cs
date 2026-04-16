using UnityEngine;
using System.IO;
using Dummiesman;

public class ModelLoader : MonoBehaviour
{
    [Header("Where the model will appear")]
    public Transform spawnPoint;

    private GameObject loadedModel;

    public void OnLoadButtonPressed()
    {
        string folderPath = Path.Combine(Application.persistentDataPath, "Models");
        string objPath = Path.Combine(folderPath, "model.obj");

        Debug.Log("Looking for OBJ at: " + objPath);

        if (!File.Exists(objPath))
        {
            Debug.LogError("OBJ file not found at: " + objPath);
            return;
        }

        LoadOBJ(objPath);
    }

    void LoadOBJ(string path)
    {
        if (loadedModel != null)
            Destroy(loadedModel);

        loadedModel = new OBJLoader().Load(path);

        loadedModel.transform.position = spawnPoint.position;
        loadedModel.transform.rotation = spawnPoint.rotation;
        loadedModel.transform.localScale = Vector3.one;

        Debug.Log("OBJ Loaded Successfully!");
    }
}
