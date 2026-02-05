using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class CloudModelLoader : MonoBehaviour
{
    public void LoadFromLink(string url)
    {
        StartCoroutine(DownloadModel(url));
    }

    IEnumerator DownloadModel(string url)
    {
        Debug.Log("Starting download: " + url);

        string folderPath = Path.Combine(Application.persistentDataPath, "DownloadedModels");
        Directory.CreateDirectory(folderPath);

        string filePath = Path.Combine(folderPath, "model.obj");

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Download failed: " + request.error);
                yield break;
            }

            File.WriteAllBytes(filePath, request.downloadHandler.data);
        }

        Debug.Log("Model downloaded to: " + filePath);
    }
}
