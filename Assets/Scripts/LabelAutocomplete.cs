using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.IO;

public class LabelAutocomplete : MonoBehaviour
{
    [Header("-- Drag these in from your scene --")]
    public TMP_InputField searchInputField;
    public GameObject suggestionButtonPrefab;
    public Transform suggestionsContainer;

    private Trie trie = new Trie();
    private List<GameObject> spawnedButtons = new List<GameObject>();

    void Start()
    {
        LoadCSV();
        suggestionsContainer.gameObject.SetActive(false); // hide on start
        searchInputField.onValueChanged.AddListener(OnTyping); // hide when done
    }

    void LoadCSV()
    {
        string path = Path.Combine(Application.streamingAssetsPath, "anatomy_labels.csv");

        if (!File.Exists(path))
        {
            Debug.LogError("CSV not found at: " + path);
            return;
        }

        string[] lines = File.ReadAllLines(path);

        for (int i = 1; i < lines.Length; i++)
        {
            string[] columns = lines[i].Split(',');
            if (columns.Length > 0 && !string.IsNullOrWhiteSpace(columns[0]))
            {
                trie.Insert(columns[0].Trim());
            }
        }

        Debug.Log("Trie loaded from CSV!");
    }

    void OnTyping(string input)
    {
        // Clear old buttons
        foreach (GameObject btn in spawnedButtons)
            Destroy(btn);
        spawnedButtons.Clear();

        // Hide if empty
        if (string.IsNullOrWhiteSpace(input) || input.Length < 1)
        {
            suggestionsContainer.gameObject.SetActive(false);
            return;
        }

        List<string> suggestions = trie.GetSuggestions(input);

        // Hide if no matches
        if (suggestions.Count == 0)
        {
            suggestionsContainer.gameObject.SetActive(false);
            return;
        }

        // Show panel
        suggestionsContainer.gameObject.SetActive(true);

        // Show max 5 suggestions
        int count = Mathf.Min(suggestions.Count, 5);
        for (int i = 0; i < count; i++)
        {
            string word = suggestions[i];

            GameObject btnObj = Instantiate(suggestionButtonPrefab, suggestionsContainer);
            spawnedButtons.Add(btnObj);

            btnObj.AddComponent<TrackedDeviceGraphicRaycaster>();
            btnObj.GetComponentInChildren<TMP_Text>().text = word;

            Button btn = btnObj.GetComponent<Button>();
            btn.onClick.AddListener(() =>
            {
                searchInputField.text = word;
                HideSuggestions();
            });
        }
    }

    // Hide suggestions when user finishes typing (presses Enter or clicks away)
    void OnFinishedTyping(string input)
    {
        HideSuggestions();
    }

    void HideSuggestions()
    {
        foreach (GameObject btn in spawnedButtons)
            Destroy(btn);
        spawnedButtons.Clear();
        suggestionsContainer.gameObject.SetActive(false);
    }
}
