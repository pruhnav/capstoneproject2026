using System.Collections.Generic;
using UnityEngine;

public class SearchManager : MonoBehaviour
{
    private Trie trie = new Trie();

    void Start()
    {
        LoadNamesFromCSV();
        TestSearch("k");
    }

    void LoadNamesFromCSV()
    {
        TextAsset csvFile = Resources.Load<TextAsset>("full structures list");

        if (csvFile == null)
        {
            Debug.LogError("CSV file not found in Assets/Resources/full structures list.csv");
            return;
        }

        string[] lines = csvFile.text.Split('\n');

        foreach (string line in lines)
        {
            string cleanedLine = line.Trim();

            if (string.IsNullOrEmpty(cleanedLine))
                continue;

            if (cleanedLine.ToLower() == "partname")
                continue;

            string[] columns = cleanedLine.Split(',');
            string partName = columns[0].Trim().Trim('"');

            if (!string.IsNullOrEmpty(partName))
            {
                trie.Insert(partName);
            }
        }

        Debug.Log("Names loaded into Trie successfully.");
    }

    public void TestSearch(string input)
    {
        List<string> suggestions = trie.GetSuggestions(input);

        Debug.Log("Suggestions for: " + input);

        foreach (string s in suggestions)
        {
            Debug.Log(s);
        }
    }
}