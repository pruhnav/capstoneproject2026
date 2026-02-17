Assets\Scripts\MongoDBExport.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Combines MongoDB connection and model export UI into a single file.
/// Attach this MonoBehaviour to a scene GameObject and assign UI fields in the Inspector:
/// - exportButton: Button named "ExporttoDB"
/// - popupPanel: a modal panel GameObject (initially inactive)
/// - renameInput: InputField inside the modal
/// - confirmButton / cancelButton: buttons inside the modal
/// - modelRoot: root GameObject of the model to export
/// </summary>
public class MongoDBExport : MonoBehaviour
{
    [Header("UI (assign in Inspector)")]
    public Button exportButton;         // ExporttoDB button
    public GameObject popupPanel;       // Modal panel (set inactive by default)
    public InputField renameInput;      // Input field to rename before saving
    public Button confirmButton;        // Confirm in popup
    public Button cancelButton;         // Cancel in popup

    [Header("Model")]
    public GameObject modelRoot;        // Root GameObject of the model to export

    [Header("MongoDB")]
    public string connectionUri = "mongodb+srv://vrcapstone2026:group212@vrcapstone2026.beb08r5.mongodb.net/?appName=VRCapstone2026";
    public string databaseName = "capstone_db";
    public string collectionName = "models";

    private MongoDBClient _client;

    void Awake()
    {
        _client = new MongoDBClient(connectionUri, databaseName, collectionName);
    }

    void Start()
    {
        if (exportButton == null || popupPanel == null || renameInput == null || confirmButton == null || cancelButton == null || modelRoot == null)
        {
            Debug.LogError("MongoDBExport: One or more required references are not assigned in the Inspector.");
            enabled = false;
            return;
        }

        popupPanel.SetActive(false);

        exportButton.onClick.AddListener(OnExportButtonClicked);
        confirmButton.onClick.AddListener(OnConfirmExportClicked);
        cancelButton.onClick.AddListener(() => popupPanel.SetActive(false));
    }

    void OnDestroy()
    {
        if (exportButton != null) exportButton.onClick.RemoveListener(OnExportButtonClicked);
        if (confirmButton != null) confirmButton.onClick.RemoveListener(OnConfirmExportClicked);
        if (cancelButton != null) cancelButton.onClick.RemoveAllListeners();
    }

    void OnExportButtonClicked()
    {
        // Pre-fill the input with the model root name and show popup
        renameInput.text = modelRoot.name;
        popupPanel.SetActive(true);

#if UNITY_EDITOR || UNITY_STANDALONE
        renameInput.Select();
        renameInput.ActivateInputField();
#endif
    }

    async void OnConfirmExportClicked()
    {
        popupPanel.SetActive(false);

        var storedName = renameInput.text?.Trim();
        if (string.IsNullOrEmpty(storedName))
            storedName = $"{modelRoot.name}_{DateTime.UtcNow:yyyyMMdd_HHmmss}";

        try
        {
            // Run serialization off the main thread
            var doc = await Task.Run(() => BuildModelBson(modelRoot, storedName));
            await _client.InsertModelAsync(doc);
            Debug.Log($"Model '{storedName}' exported to MongoDB collection '{collectionName}' successfully.");
        }
        catch (Exception ex)
        {
            Debug.LogError($"MongoDBExport: Failed to export model: {ex}");
        }
    }

    // Serializes the model hierarchy into a BSON document. Keep heavy work off the main thread.
    BsonDocument BuildModelBson(GameObject root, string storedName)
    {
        var rootDoc = new BsonDocument
        {
            { "name", storedName },
            { "sourceName", root.name },
            { "createdAtUtc", DateTime.UtcNow }
        };

        var meshesArray = new BsonArray();

        var meshFilters = root.GetComponentsInChildren<MeshFilter>(true);
        foreach (var mf in meshFilters)
        {
            var mesh = mf.sharedMesh;
            if (mesh == null) continue;

            var meshDoc = new BsonDocument
            {
                { "objectName", mf.gameObject.name },
                { "vertexCount", mesh.vertexCount },
            };

            // Vertices
            var vertices = new BsonArray();
            foreach (var v in mesh.vertices)
                vertices.Add(new BsonArray { v.x, v.y, v.z });
            meshDoc.Add("vertices", vertices);

            // Normals
            if (mesh.normals != null && mesh.normals.Length == mesh.vertexCount)
            {
                var normals = new BsonArray();
                foreach (var n in mesh.normals)
                    normals.Add(new BsonArray { n.x, n.y, n.z });
                meshDoc.Add("normals", normals);
            }

            // UVs (channel 0)
            if (mesh.uv != null && mesh.uv.Length == mesh.vertexCount)
            {
                var uvs = new BsonArray();
                foreach (var uv in mesh.uv)
                    uvs.Add(new BsonArray { uv.x, uv.y });
                meshDoc.Add("uvs", uvs);
            }

            // Triangles
            var triangles = new BsonArray();
            foreach (var t in mesh.triangles)
                triangles.Add(t);
            meshDoc.Add("triangles", triangles);

            // Materials & readable textures if any
            var renderer = mf.GetComponent<Renderer>();
            if (renderer != null)
            {
                var mats = renderer.sharedMaterials;
                var matArray = new BsonArray();
                foreach (var mat in mats)
                {
                    if (mat == null) continue;
                    var matDoc = new BsonDocument
                    {
                        { "shader", mat.shader != null ? mat.shader.name : BsonNull.Value }
                    };

                    if (mat.HasProperty("_Color"))
                    {
                        var col = mat.color;
                        matDoc.Add("color", new BsonArray { col.r, col.g, col.b, col.a });
                    }

                    if (mat.mainTexture is Texture2D tex)
                    {
                        try
                        {
                            var texBytes = ExportTextureToPNG(tex);
                            matDoc.Add("mainTexture", new BsonDocument
                            {
                                { "fileName", tex.name },
                                { "data", new BsonBinaryData(texBytes) },
                                { "format", "png" }
                            });
                        }
                        catch (Exception)
                        {
                            matDoc.Add("mainTexture", new BsonDocument
                            {
                                { "fileName", tex.name },
                                { "data", BsonNull.Value },
                                { "format", "png" },
                                { "note", "texture not readable or failed to encode" }
                            });
                        }
                    }

                    matArray.Add(matDoc);
                }

                meshDoc.Add("materials", matArray);
            }

            meshesArray.Add(meshDoc);
        }

        rootDoc.Add("meshes", meshesArray);

        // Root transform info
        rootDoc.Add("rootTransform", new BsonDocument
        {
            { "position", new BsonArray { root.transform.position.x, root.transform.position.y, root.transform.position.z } },
            { "rotation", new BsonArray { root.transform.rotation.eulerAngles.x, root.transform.rotation.eulerAngles.y, root.transform.rotation.eulerAngles.z } },
            { "scale", new BsonArray { root.transform.localScale.x, root.transform.localScale.y, root.transform.localScale.z } }
        });

        return rootDoc;
    }

    // Attempts to export a Texture2D to PNG bytes. If texture isn't readable, create a readable copy via RenderTexture.
    byte[] ExportTextureToPNG(Texture2D src)
    {
        if (src == null) throw new ArgumentNullException(nameof(src));
        try
        {
            return src.EncodeToPNG();
        }
        catch (Exception)
        {
            // Try make a readable copy
            var copy = new Texture2D(src.width, src.height, TextureFormat.ARGB32, false);
            var prev = RenderTexture.active;
            var rt = RenderTexture.GetTemporary(src.width, src.height);
            Graphics.Blit(src, rt);
            RenderTexture.active = rt;
            copy.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
            copy.Apply();
            RenderTexture.active = prev;
            RenderTexture.ReleaseTemporary(rt);

            try
            {
                var bytes = copy.EncodeToPNG();
                UnityEngine.Object.DestroyImmediate(copy);
                return bytes;
            }
            catch (Exception ex)
            {
                UnityEngine.Object.DestroyImmediate(copy);
                throw new Exception("Texture export failed", ex);
            }
        }
    }

    // Minimal MongoDB client wrapper used by this exporter
    class MongoDBClient
    {
        private readonly IMongoCollection<BsonDocument> _collection;

        public MongoDBClient(string uri, string dbName, string collName)
        {
            var settings = MongoClientSettings.FromConnectionString(uri);
            settings.ServerApi = new ServerApi(ServerApiVersion.V1);
            var client = new MongoClient(settings);
            var db = client.GetDatabase(dbName);
            _collection = db.GetCollection<BsonDocument>(collName);

            // Basic ping to validate connection (runs synchronously here; it's lightweight)
            try
            {
                var result = db.RunCommand<BsonDocument>(new BsonDocument("ping", 1));
                Debug.Log("MongoDB ping succeeded: " + result);
            }
            catch (Exception ex)
            {
                Debug.LogError("MongoDB ping failed: " + ex);
            }
        }

        public Task InsertModelAsync(BsonDocument doc)
        {
            return _collection.InsertOneAsync(doc);
        }
    }
}