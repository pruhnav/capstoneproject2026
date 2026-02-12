using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

[Serializable]
public class Vec3
{
    public float x, y, z;
    public Vec3(Vector3 v) { x = v.x; y = v.y; z = v.z; }
}

[Serializable]
public class Payload
{
    public string modelId;
    public string markerName;
    public Vec3 pos;
}

public class BallMarkerExporter : MonoBehaviour
{
    public string apiUrl = "http://localhost:3000/api/ball-points";
    public string modelId = "CT_Model";

    void Start()
    {
        ExportMarkers();
    }

    public void ExportMarkers()
    {
        GameObject[] allObjects = FindObjectsOfType<GameObject>();

        foreach (GameObject obj in allObjects)
        {
            if (obj.name.Contains("ballMarker"))
            {
                StartCoroutine(SendMarker(obj));
            }
        }

        Debug.Log("Export started");
    }

    IEnumerator SendMarker(GameObject marker)
    {
        Payload payload = new Payload
        {
            modelId = modelId,
            markerName = marker.name,
            pos = new Vec3(marker.transform.position)
        };

        string json = JsonUtility.ToJson(payload);

        using (UnityWebRequest req = new UnityWebRequest(apiUrl, "POST"))
        {
            byte[] body = Encoding.UTF8.GetBytes(json);

            req.uploadHandler = new UploadHandlerRaw(body);
            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader("Content-Type", "application/json");

            yield return req.SendWebRequest();

            if (req.result != UnityWebRequest.Result.Success)
                Debug.LogError("Failed: " + marker.name);
            else
                Debug.Log("Saved: " + marker.name);
        }
    }
}
