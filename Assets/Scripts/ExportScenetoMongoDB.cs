using System;
using System.Collections;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

public class ExportSceneToMongoButton : MonoBehaviour
{
    [Header("Mongo / Endpoint (use Atlas Data API or your own REST-to-Mongo proxy)")]
    [Tooltip("HTTP endpoint that accepts JSON { \"filename\": string, \"contentBase64\": string }")]
    public string mongoUploadUrl = "https://your-mongo-rest-endpoint.example.com/upload";

    [Tooltip("Optional API key header name/value - set apiKeyHeaderName empty when not used")]
    public string apiKeyHeaderName = "api-key";
    public string apiKey = "";

    // Call this method from your UI Button OnClick
    public void OnPressed_ShowRenameAndExportPopup()
    {
#if UNITY_EDITOR
        ShowRenamePopup();
#else
        Debug.LogWarning("Export to Mongo is editor-only. Attach and run inside the Unity Editor.");
#endif
    }

#if UNITY_EDITOR
    void ShowRenamePopup()
    {
        // Build a small runtime popup UI so it's one-file and easy to wire up.
        // Create Canvas
        var canvasGO = new GameObject("ExportScenePopup_Canvas");
        var canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasGO.AddComponent<CanvasScaler>();
        canvasGO.AddComponent<GraphicRaycaster>();

        // Blocker background
        var blocker = CreateUIObject("Blocker", canvasGO.transform);
        var blockerImg = blocker.AddComponent<Image>();
        blockerImg.color = new Color(0, 0, 0, 0.45f);
        SetRect(blocker, Vector2.zero, Vector2.one, Vector2.zero);
        blocker.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

        // Panel
        var panel = CreateUIObject("Panel", canvasGO.transform);
        var panelImg = panel.AddComponent<Image>();
        panelImg.color = new Color(0.96f, 0.96f, 0.96f, 1f);
        var panelRT = panel.GetComponent<RectTransform>();
        panelRT.sizeDelta = new Vector2(520, 180);
        panelRT.anchorMin = new Vector2(0.5f, 0.5f);
        panelRT.anchorMax = panelRT.anchorMin;
        panelRT.pivot = new Vector2(0.5f, 0.5f);
        panelRT.anchoredPosition = Vector2.zero;

        // Title
        var title = CreateText("Title", panel.transform, "Please Rename File before Exporting", 18, TextAnchor.UpperCenter);
        var titleRT = title.GetComponent<RectTransform>();
        titleRT.anchoredPosition = new Vector2(0, -12);
        titleRT.sizeDelta = new Vector2(480, 28);

        // Input label
        var label = CreateText("Label", panel.transform, "New scene name:", 14, TextAnchor.MiddleLeft);
        var labelRT = label.GetComponent<RectTransform>();
        labelRT.anchoredPosition = new Vector2(-160, -50);
        labelRT.sizeDelta = new Vector2(300, 24);

        // Input Field
        var inputGO = CreateUIObject("NameInput", panel.transform);
        var inputImg = inputGO.AddComponent<Image>();
        inputImg.color = Color.white;
        var inputRT = inputGO.GetComponent<RectTransform>();
        inputRT.sizeDelta = new Vector2(420, 32);
        inputRT.anchoredPosition = new Vector2(0, -50);

        var inputField = inputGO.AddComponent<InputField>();
        inputField.textComponent = CreateText("InputText", inputGO.transform, "", 14, TextAnchor.MiddleLeft).GetComponent<Text>();
        inputField.placeholder = CreateText("Placeholder", inputGO.transform, "Enter new scene name", 14, TextAnchor.MiddleLeft).GetComponent<Text>();
        var inputTextRT = inputField.textComponent.GetComponent<RectTransform>();
        inputTextRT.anchorMin = new Vector2(0, 0);
        inputTextRT.anchorMax = new Vector2(1, 1);
        inputTextRT.sizeDelta = Vector2.zero;
        inputTextRT.anchoredPosition = Vector2.zero;

        // Prefill with current scene name (without extension)
        var activeScene = EditorSceneManager.GetActiveScene();
        var currentName = string.IsNullOrEmpty(activeScene.path) ? "NewScene" : Path.GetFileNameWithoutExtension(activeScene.path);
        inputField.text = currentName;

        // Save Button
        var saveBtn = CreateButton("Save", panel.transform, "Save");
        var saveRT = saveBtn.GetComponent<RectTransform>();
        saveRT.anchoredPosition = new Vector2(-80, -100);
        saveRT.sizeDelta = new Vector2(140, 36);

        // Cancel Button
        var cancelBtn = CreateButton("Cancel", panel.transform, "Cancel");
        var cancelRT = cancelBtn.GetComponent<RectTransform>();
        cancelRT.anchoredPosition = new Vector2(120, -100);
        cancelRT.sizeDelta = new Vector2(140, 36);

        // Status Text
        var status = CreateText("Status", panel.transform, "", 13, TextAnchor.MiddleCenter);
        var statusRT = status.GetComponent<RectTransform>();
        statusRT.anchoredPosition = new Vector2(0, -140);
        statusRT.sizeDelta = new Vector2(480, 20);

        // Hook actions
        saveBtn.onClick.AddListener(() =>
        {
            var newName = inputField.text?.Trim();
            if (string.IsNullOrWhiteSpace(newName))
            {
                status.text = "Name cannot be empty.";
                return;
            }

            saveBtn.interactable = false;
            cancelBtn.interactable = false;
            status.text = "Renaming and uploading...";

            // Perform rename/save then upload
            EditorApplication.delayCall += () =>
            {
                try
                {
                    string finalPath = RenameAndSaveActiveScene(newName, out string renameMessage);
                    status.text = renameMessage + " Preparing upload...";
                    // Read file bytes and start upload coroutine from EditorApplication update via a helper GameObject
                    var helper = new GameObject("ExportHelper");
                    var uploader = helper.AddComponent<EditorUploadHelper>();
                    uploader.Init(finalPath, Path.GetFileName(finalPath), mongoUploadUrl, apiKeyHeaderName, apiKey, (success, msg) =>
                    {
                        status.text = msg;
                        if (success)
                        {
                            EditorApplication.delayCall += () => UnityEngine.Object.DestroyImmediate(canvasGO);
                            EditorApplication.delayCall += () => UnityEngine.Object.DestroyImmediate(helper);
                        }
                        else
                        {
                            saveBtn.interactable = true;
                            cancelBtn.interactable = true;
                        }
                    });
                }
                catch (Exception ex)
                {
                    status.text = "Error: " + ex.Message;
                    saveBtn.interactable = true;
                    cancelBtn.interactable = true;
                }
            };
        });

        cancelBtn.onClick.AddListener(() =>
        {
            UnityEngine.Object.DestroyImmediate(canvasGO);
        });
    }

    // Renames (or saves) the active scene asset on disk and returns the absolute path to the saved scene file.
    string RenameAndSaveActiveScene(string newName, out string message)
    {
        var scene = EditorSceneManager.GetActiveScene();
        var oldPath = scene.path; // may be empty for unsaved scene
        var rootFolder = Directory.GetParent(Application.dataPath).FullName; // project root full path

        if (string.IsNullOrEmpty(oldPath))
        {
            // unsaved scene: save to Assets/
            var newRelative = $"Assets/{newName}.unity";
            if (AssetDatabase.LoadAssetAtPath<SceneAsset>(newRelative) != null)
            {
                if (!EditorUtility.DisplayDialog("Overwrite Scene?", $"A scene exists at {newRelative}. Overwrite?", "Overwrite", "Cancel"))
                {
                    message = "Cancelled by user.";
                    return null;
                }
            }

            bool saved = EditorSceneManager.SaveScene(scene, newRelative);
            AssetDatabase.Refresh();
            message = saved ? $"Saved new scene as {newRelative}." : $"Failed to save scene to {newRelative}.";
            return saved ? Path.Combine(rootFolder, newRelative).Replace("\\", "/") : null;
        }

        var dir = Path.GetDirectoryName(oldPath).Replace("\\", "/");
        var newRelativePath = Path.Combine(dir, newName + ".unity").Replace("\\", "/");

        if (string.Equals(oldPath, newRelativePath, StringComparison.OrdinalIgnoreCase))
        {
            // just save current scene
            bool ok = EditorSceneManager.SaveScene(scene);
            message = ok ? $"Scene saved ({oldPath})." : $"Failed to save ({oldPath}).";
            return ok ? Path.Combine(rootFolder, oldPath).Replace("\\", "/") : null;
        }

        // If target exists ask for overwrite
        if (AssetDatabase.LoadAssetAtPath<SceneAsset>(newRelativePath) != null)
        {
            if (!EditorUtility.DisplayDialog("Overwrite Scene Asset?", $"A scene already exists at {newRelativePath}. Overwrite?", "Overwrite", "Cancel"))
            {
                message = "Cancelled by user.";
                return null;
            }
        }

        string error = AssetDatabase.MoveAsset(oldPath, newRelativePath);
        if (!string.IsNullOrEmpty(error))
        {
            message = "Failed to rename scene: " + error;
            return null;
        }

        // Reopen to ensure Editor uses new path
        var reopened = EditorSceneManager.OpenScene(newRelativePath, OpenSceneMode.Single);
        bool savedOk = EditorSceneManager.SaveScene(reopened);
        AssetDatabase.Refresh();
        message = savedOk ? $"Scene renamed and saved as {newRelativePath}." : $"Renamed but failed to save as {newRelativePath}.";
        return savedOk ? Path.Combine(rootFolder, newRelativePath).Replace("\\", "/") : null;
    }

    // Small utility: create UI GameObject with RectTransform
    static GameObject CreateUIObject(string name, Transform parent)
    {
        var go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        return go;
    }

    static Text CreateText(string name, Transform parent, string text, int fontSize, TextAnchor anchor)
    {
        var go = CreateUIObject(name, parent);
        var txt = go.AddComponent<Text>();
        txt.text = text;
        txt.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        txt.fontSize = fontSize;
        txt.alignment = anchor;
        txt.color = Color.black;
        var rt = go.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(400, 24);
        return txt;
    }

    static Button CreateButton(string name, Transform parent, string caption)
    {
        var go = CreateUIObject(name, parent);
        var img = go.AddComponent<Image>();
        img.color = new Color(0.22f, 0.5f, 0.9f, 1f);
        var btn = go.AddComponent<Button>();
        var txt = CreateText("Text", go.transform, caption, 14, TextAnchor.MiddleCenter);
        txt.color = Color.white;
        return btn;
    }

    static void SetRect(GameObject go, Vector2 anchorMin, Vector2 anchorMax, Vector2 anchoredPos)
    {
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
        rt.anchoredPosition = anchoredPos;
    }

    // Helper MonoBehaviour to run coroutine from Editor context (still runs in play mode in editor)
    class EditorUploadHelper : MonoBehaviour
    {
        string absoluteFilePath;
        string fileName;
        string uploadUrl;
        string headerName;
        string headerValue;
        Action<bool, string> callback;

        public void Init(string absolutePath, string fileName, string uploadUrl, string headerName, string headerValue, Action<bool, string> finished)
        {
            this.absoluteFilePath = absolutePath;
            this.fileName = fileName;
            this.uploadUrl = uploadUrl;
            this.headerName = headerName;
            this.headerValue = headerValue;
            this.callback = finished;
            DontDestroyOnLoad(gameObject);
            StartCoroutine(DoReadAndUpload());
        }

        IEnumerator DoReadAndUpload()
        {
            if (string.IsNullOrEmpty(absoluteFilePath) || !File.Exists(absoluteFilePath))
            {
                callback?.Invoke(false, "Scene file not found: " + absoluteFilePath);
                yield break;
            }

            byte[] bytes;
            try
            {
                bytes = File.ReadAllBytes(absoluteFilePath);
            }
            catch (Exception ex)
            {
                callback?.Invoke(false, "Failed to read scene file: " + ex.Message);
                yield break;
            }

            string base64 = Convert.ToBase64String(bytes);
            var payload = new JSONObject();
            payload.AddField("filename", fileName);
            payload.AddField("contentBase64", base64);

            var json = payload.ToString();

            using (var uwr = new UnityWebRequest(uploadUrl, "POST"))
            {
                byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
                uwr.uploadHandler = new UploadHandlerRaw(bodyRaw);
                uwr.downloadHandler = new DownloadHandlerBuffer();
                uwr.SetRequestHeader("Content-Type", "application/json");
                if (!string.IsNullOrEmpty(headerName) && !string.IsNullOrEmpty(headerValue))
                    uwr.SetRequestHeader(headerName, headerValue);

                yield return uwr.SendWebRequest();

#if UNITY_2020_1_OR_NEWER
                bool isNetworkError = uwr.result == UnityWebRequest.Result.ConnectionError;
                bool isHttpError = uwr.result == UnityWebRequest.Result.ProtocolError;
#else
                bool isNetworkError = uwr.isNetworkError;
                bool isHttpError = uwr.isHttpError;
#endif

                if (isNetworkError || isHttpError)
                {
                    callback?.Invoke(false, $"Upload failed: {uwr.error} (code {(int)uwr.responseCode})");
                }
                else
                {
                    callback?.Invoke(true, $"Upload succeeded: {uwr.downloadHandler.text}");
                }
            }
        }
    }

    // Minimal JSON builder to avoid external deps (keeps file self-contained).
    // Very small helper to produce JSON with two string fields.
    class JSONObject
    {
        System.Collections.Generic.Dictionary<string, string> _fields = new System.Collections.Generic.Dictionary<string, string>();

        public void AddField(string key, string val)
        {
            _fields[key] = val ?? "";
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append('{');
            bool first = true;
            foreach (var kv in _fields)
            {
                if (!first) sb.Append(',');
                first = false;
                sb.Append('\"').Append(Escape(kv.Key)).Append("\":");
                sb.Append('\"').Append(Escape(kv.Value)).Append('\"');
            }
            sb.Append('}');
            return sb.ToString();
        }

        static string Escape(string s)
        {
            if (s == null) return "";
            return s.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\n", "\\n").Replace("\r", "\\r");
        }
    }
#endif
}