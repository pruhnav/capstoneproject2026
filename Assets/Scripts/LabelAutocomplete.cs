using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.IO;

public class LabelAutocomplete : MonoBehaviour
{
    [Header("-- Drag these in from your scene --")]
    public TMP_InputField searchInputField;       // drag SearchInput here
    public GameObject suggestionButtonPrefab;      // drag your button prefab here
    public Transform suggestionsContainer;         // drag SuggestionsPanel here

    private Trie trie = new Trie();
    private List<GameObject> spawnedButtons = new List<GameObject>();

    void Start()
    {
        LoadCSV();
        searchInputField.onValueChanged.AddListener(OnTyping);
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

        // Skip the first line (header: label_name,category)
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
        // Clear old suggestion buttons
        foreach (GameObject btn in spawnedButtons)
            Destroy(btn);
        spawnedButtons.Clear();

        if (string.IsNullOrWhiteSpace(input) || input.Length < 1)
            return;

        List<string> suggestions = trie.GetSuggestions(input);

        // Show max 5 suggestions
        int count = Mathf.Min(suggestions.Count, 5);
        for (int i = 0; i < count; i++)
        {
            string word = suggestions[i];

            // Create a button for each suggestion
            GameObject btnObj = Instantiate(suggestionButtonPrefab, suggestionsContainer);
            spawnedButtons.Add(btnObj);

            // Set the button text
            btnObj.GetComponentInChildren<TMP_Text>().text = word;

            // When clicked, fill the input field with this word
            Button btn = btnObj.GetComponent<Button>();
            btn.onClick.AddListener(() =>
            {
                searchInputField.text = word;
                // Clear suggestions after picking
                foreach (GameObject b in spawnedButtons) Destroy(b);
                spawnedButtons.Clear();
            });
        }
    }
}