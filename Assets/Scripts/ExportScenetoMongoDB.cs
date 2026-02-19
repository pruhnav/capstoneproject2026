using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ExportScenetoMongoDB : MonoBehaviour
{
    [Header("Optional - assign in Inspector")]
    public Button exportButton; // Optional - will try to find "Button_ExportDB" if null
    public GameObject popupPanel; // Optional - will be created at runtime if null
    public InputField nameInput;
    public Button saveButton;
    public Button cancelButton;

    void Start()
    {
        // Find the export trigger button by name if not assigned
        if (exportButton == null)
        {
            var go = GameObject.Find("Button_ExportDB");
            if (go != null) exportButton = go.GetComponent<Button>();
        }

        if (exportButton == null)
        {
            Debug.LogWarning("ExportScenetoMongoDB: No export button found. Assign 'exportButton' or add a GameObject named 'Button_ExportDB' with a Button component.");
        }
        else
        {
            exportButton.onClick.AddListener(OnExportButtonClicked);
        }

        // If popup controls are missing, try to find them under a popup GameObject
        if (popupPanel == null)
        {
            var foundPanel = GameObject.Find("Popup_ExportDB");
            if (foundPanel != null) popupPanel = foundPanel;
        }

        if (popupPanel != null)
        {
            // try to locate child components if not assigned
            if (nameInput == null) nameInput = popupPanel.GetComponentInChildren<InputField>();
            if (saveButton == null) saveButton = popupPanel.GetComponentInChildren<Button>(true); // may pick first button; below we rewire explicitly if multiple
            // Better to find by names if present
            var saveGo = popupPanel.transform.Find("SaveButton");
            if (saveGo != null) saveButton = saveGo.GetComponent<Button>();
            var cancelGo = popupPanel.transform.Find("CancelButton");
            if (cancelGo != null) cancelButton = cancelGo.GetComponent<Button>();
        }

        // If still missing UI, create a minimal popup UI under existing Canvas
        if (popupPanel == null || nameInput == null || saveButton == null || cancelButton == null)
        {
            CreatePopupUI();
        }

        // Configure popup initial state and listeners
        popupPanel.SetActive(false);
        nameInput.onValueChanged.AddListener(OnNameValueChanged);
        saveButton.onClick.AddListener(OnSaveClicked);
        if (cancelButton != null) cancelButton.onClick.AddListener(ClosePopup);
        ValidateSaveButton();
    }

    void OnExportButtonClicked()
    {
        // Pre-fill input with scene name to help user
        try
        {
            nameInput.text = SceneManager.GetActiveScene().name;
        }
        catch
        {
            nameInput.text = "ExportedScene";
        }

        popupPanel.SetActive(true);
        nameInput.Select();
        nameInput.ActivateInputField();
    }

    void OnNameValueChanged(string s)
    {
        ValidateSaveButton();
    }

    void ValidateSaveButton()
    {
        saveButton.interactable = !string.IsNullOrWhiteSpace(nameInput.text);
    }

    void OnSaveClicked()
    {
        var exportName = nameInput.text.Trim();
        if (string.IsNullOrEmpty(exportName))
        {
            Debug.LogWarning("Export name is empty. Save aborted.");
            return;
        }

        // Call the export logic (placeholder). Replace this with actual MongoDB export implementation.
        ExportToMongoDB(exportName);

        ClosePopup();
    }

    void ClosePopup()
    {
        popupPanel.SetActive(false);
    }

    void ExportToMongoDB(string name)
    {
        // TODO: integrate real export code here.
        // This is a placeholder to show where you'd call your MongoDB export routine.
        Debug.Log($"Exporting scene as '{name}' to MongoDB (placeholder).");
    }

    // Creates a simple popup UI at runtime when not provided in the scene.
    void CreatePopupUI()
    {
        // Find or create a Canvas
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            var canvasGO = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            canvas = canvasGO.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        }

        // Create panel
        popupPanel = new GameObject("Popup_ExportDB", typeof(RectTransform), typeof(Image));
        popupPanel.transform.SetParent(canvas.transform, false);
        var panelImage = popupPanel.GetComponent<Image>();
        panelImage.color = new Color(0f, 0f, 0f, 0.75f);

        var panelRect = popupPanel.GetComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.5f, 0.5f);
        panelRect.anchorMax = new Vector2(0.5f, 0.5f);
        panelRect.sizeDelta = new Vector2(360, 160);
        panelRect.anchoredPosition = Vector2.zero;

        // Title text
        var titleGO = new GameObject("Title", typeof(RectTransform), typeof(Text));
        titleGO.transform.SetParent(popupPanel.transform, false);
        var titleText = titleGO.GetComponent<Text>();
        titleText.text = "Export - Enter name";
        titleText.alignment = TextAnchor.UpperCenter;
        titleText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        titleText.color = Color.white;
        var titleRect = titleGO.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0f, 1f);
        titleRect.anchorMax = new Vector2(1f, 1f);
        titleRect.pivot = new Vector2(0.5f, 1f);
        titleRect.anchoredPosition = new Vector2(0, -10);
        titleRect.sizeDelta = new Vector2(0, 24);

        // Input field background
        var inputBg = new GameObject("InputBG", typeof(RectTransform), typeof(Image));
        inputBg.transform.SetParent(popupPanel.transform, false);
        var bgImage = inputBg.GetComponent<Image>();
        bgImage.color = Color.white * 0.1f;
        var inputBgRect = inputBg.GetComponent<RectTransform>();
        inputBgRect.anchorMin = new Vector2(0.05f, 0.55f);
        inputBgRect.anchorMax = new Vector2(0.95f, 0.75f);
        inputBgRect.anchoredPosition = Vector2.zero;
        inputBgRect.sizeDelta = Vector2.zero;

        // InputField (requires a child Text for the placeholder/text)
        var inputGO = new GameObject("NameInput", typeof(RectTransform), typeof(Image), typeof(InputField));
        inputGO.transform.SetParent(inputBg.transform, false);
        var inputImage = inputGO.GetComponent<Image>();
        inputImage.color = Color.white * 0.0f; // transparent so background shows
        var inputRect = inputGO.GetComponent<RectTransform>();
        inputRect.anchorMin = new Vector2(0f, 0f);
        inputRect.anchorMax = new Vector2(1f, 1f);
        inputRect.sizeDelta = Vector2.zero;

        nameInput = inputGO.GetComponent<InputField>();
        nameInput.textComponent = CreateText("InputText", inputGO.transform, "", TextAnchor.MiddleLeft, 14);
        nameInput.placeholder = CreateText("Placeholder", inputGO.transform, "Enter export name...", TextAnchor.MiddleLeft, 14, Color.gray);
        nameInput.transition = Selectable.Transition.None;

        // Buttons container
        var buttonsGO = new GameObject("Buttons", typeof(RectTransform));
        buttonsGO.transform.SetParent(popupPanel.transform, false);
        var buttonsRect = buttonsGO.GetComponent<RectTransform>();
        buttonsRect.anchorMin = new Vector2(0f, 0f);
        buttonsRect.anchorMax = new Vector2(1f, 0.4f);
        buttonsRect.pivot = new Vector2(0.5f, 0f);
        buttonsRect.anchoredPosition = new Vector2(0, 10);
        buttonsRect.sizeDelta = Vector2.zero;

        // Save Button
        var saveGO = CreateButton("SaveButton", buttonsGO.transform, "Save");
        saveButton = saveGO.GetComponent<Button>();
        var saveRect = saveGO.GetComponent<RectTransform>();
        saveRect.anchorMin = new Vector2(0.55f, 0.1f);
        saveRect.anchorMax = new Vector2(0.95f, 0.9f);
        saveRect.sizeDelta = Vector2.zero;

        // Cancel Button
        var cancelGO = CreateButton("CancelButton", buttonsGO.transform, "Cancel");
        cancelButton = cancelGO.GetComponent<Button>();
        var cancelRect = cancelGO.GetComponent<RectTransform>();
        cancelRect.anchorMin = new Vector2(0.05f, 0.1f);
        cancelRect.anchorMax = new Vector2(0.45f, 0.9f);
        cancelRect.sizeDelta = Vector2.zero;

        // Visual polish: colors
        ApplyButtonColors(saveButton, new Color(0.2f, 0.6f, 0.2f));
        ApplyButtonColors(cancelButton, new Color(0.6f, 0.2f, 0.2f));
    }

    Text CreateText(string name, Transform parent, string text, TextAnchor align, int fontSize, Color? color = null)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(Text));
        go.transform.SetParent(parent, false);
        var t = go.GetComponent<Text>();
        t.text = text;
        t.alignment = align;
        t.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        t.fontSize = fontSize;
        t.color = color ?? Color.white;
        var rect = go.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0f, 0f);
        rect.anchorMax = new Vector2(1f, 1f);
        rect.sizeDelta = Vector2.zero;
        rect.anchoredPosition = Vector2.zero;
        return t;
    }

    GameObject CreateButton(string name, Transform parent, string label)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button));
        go.transform.SetParent(parent, false);
        var img = go.GetComponent<Image>();
        img.color = new Color(1f, 1f, 1f, 0.12f);

        var btn = go.GetComponent<Button>();
        btn.transition = Selectable.Transition.ColorTint;

        var txt = CreateText("Text", go.transform, label, TextAnchor.MiddleCenter, 16);
        txt.color = Color.white;

        return go;
    }

    void ApplyButtonColors(Button btn, Color baseColor)
    {
        var colors = btn.colors;
        colors.normalColor = baseColor;
        colors.highlightedColor = baseColor + new Color(0.1f, 0.1f, 0.1f);
        colors.pressedColor = baseColor - new Color(0.1f, 0.1f, 0.1f);
        btn.colors = colors;
    }

    void OnDestroy()
    {
        if (exportButton != null)
            exportButton.onClick.RemoveListener(OnExportButtonClicked);
        if (saveButton != null)
            saveButton.onClick.RemoveListener(OnSaveClicked);
        if (cancelButton != null)
            cancelButton.onClick.RemoveListener(ClosePopup);
        if (nameInput != null)
            nameInput.onValueChanged.RemoveListener(OnNameValueChanged);
    }
}