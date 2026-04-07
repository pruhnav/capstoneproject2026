using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Text;

public class SaveModelWithLabels : MonoBehaviour
{
    public GameObject modelRoot;       // Your OBJ model in the scene
    public Transform labelsParent;     // Parent object containing all label objects

    public void Save()
    {
        string path = "/storage/emulated/0/Download/SavedModel";

        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);

        string objPath = Path.Combine(path, "model.obj");
        string labelPath = Path.Combine(path, "labels.json");

        ExportOBJ(modelRoot, objPath);
        ExportLabels(labelPath);

        Debug.Log("Model + labels saved to: " + path);
    }

    void ExportLabels(string filePath)
    {
        var labelList = new System.Collections.Generic.List<LabelData>();

        foreach (Transform label in labelsParent)
        {
            var data = new LabelData()
            {
                text = label.name,
                position = label.position,
                rotation = label.rotation
            };
            labelList.Add(data);
        }

        string json = JsonUtility.ToJson(new LabelCollection(labelList), true);
        File.WriteAllText(filePath, json);
    }

    void ExportOBJ(GameObject obj, string filePath)
    {
        // Use any runtime OBJ exporter here
        var exporter = new RuntimeOBJExporter();
        string objText = exporter.Export(obj);
        File.WriteAllText(filePath, objText);
        string mtlPath = Path.ChangeExtension(filePath, ".mtl");
        File.WriteAllText(mtlPath, "");

    }
}

[System.Serializable]
public class LabelData
{
    public string text;
    public Vector3 position;
    public Quaternion rotation;
}

[System.Serializable]
public class LabelCollection
{
    public System.Collections.Generic.List<LabelData> labels;
    public LabelCollection(System.Collections.Generic.List<LabelData> list) => labels = list;
}
