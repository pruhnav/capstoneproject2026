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

    public void Insert(string word)
    {
        if (string.IsNullOrWhiteSpace(word))
            return;

        word = word.Trim().ToLower();
        TrieNode current = root;

        foreach (char c in word)
        {
            if (!current.children.ContainsKey(c))
            {
                current.children[c] = new TrieNode();
            }

            current = current.children[c];
        }

        current.isEndOfWord = true;
        current.fullWord = word;
    }

    public List<string> GetSuggestions(string prefix)
    {
        List<string> results = new List<string>();

        if (string.IsNullOrWhiteSpace(prefix))
            return results;

        prefix = prefix.Trim().ToLower();
        TrieNode current = root;

        foreach (char c in prefix)
        {
            if (!current.children.ContainsKey(c))
            {
                return results;
            }

            current = current.children[c];
        }

        CollectWords(current, results);
        return results;
    }

    private void CollectWords(TrieNode node, List<string> results)
    {
        if (node.isEndOfWord)
        {
            results.Add(node.fullWord);
        }

        foreach (var child in node.children.Values)
        {
            CollectWords(child, results);
        }
    }
}