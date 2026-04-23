using System.Collections.Generic;

public class TrieNode
{
    public Dictionary<char, TrieNode> children = new Dictionary<char, TrieNode>();
    public bool isEndOfWord = false;
    public string fullWord = "";
}

public class Trie
{
    private TrieNode root = new TrieNode();
    private List<string> allWords = new List<string>(); // for abbreviation search

    public void Insert(string word)
    {
        if (string.IsNullOrWhiteSpace(word)) return;

        string original = word.Trim();
        string lower = original.ToLowerInvariant();

        // Insert into trie normally
        TrieNode current = root;
        foreach (char c in lower)
        {
            if (!current.children.ContainsKey(c))
                current.children[c] = new TrieNode();
            current = current.children[c];
        }
        current.isEndOfWord = true;
        current.fullWord = original;

        // Save for abbreviation search
        allWords.Add(original);
    }

    public List<string> GetSuggestions(string prefix)
    {
        List<string> results = new List<string>();
        if (string.IsNullOrWhiteSpace(prefix)) return results;

        prefix = prefix.Trim().ToLowerInvariant();

        // 1. Normal prefix search (e.g. "pat" → "Patella")
        TrieNode current = root;
        foreach (char c in prefix)
        {
            if (!current.children.ContainsKey(c))
                break;
            current = current.children[c];
        }

        // Only collect if we walked the full prefix
        bool fullPrefixFound = true;
        current = root;
        foreach (char c in prefix)
        {
            if (!current.children.ContainsKey(c))
            {
                fullPrefixFound = false;
                break;
            }
            current = current.children[c];
        }
        if (fullPrefixFound)
            CollectWords(current, results);

        // 2. Abbreviation search (e.g. "jc" → "Joint Capsule")
        List<string> abbrMatches = GetAbbreviationMatches(prefix);
        foreach (string match in abbrMatches)
        {
            if (!results.Contains(match))
                results.Add(match);
        }

        return results;
    }

    private void CollectWords(TrieNode node, List<string> results)
    {
        if (node.isEndOfWord)
            results.Add(node.fullWord);

        foreach (var child in node.children.Values)
            CollectWords(child, results);
    }

    // "jc" → matches "Joint Capsule" because first letters are J and C
    private List<string> GetAbbreviationMatches(string input)
    {
        List<string> matches = new List<string>();

        foreach (string word in allWords)
        {
            string abbr = GetAbbreviation(word);
            if (abbr.StartsWith(input))
                matches.Add(word);
        }

        return matches;
    }

    // "Joint Capsule" → "jc"
    private string GetAbbreviation(string label)
    {
        string abbr = "";
        string[] words = label.Split(' ');
        foreach (string w in words)
        {
            if (!string.IsNullOrWhiteSpace(w))
                abbr += w[0];
        }
        return abbr.ToLower();
    }
}
