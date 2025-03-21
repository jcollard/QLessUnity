using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public class Trie
{
    public readonly string Prefix;
    public bool IsWord { get; private set; }
    private readonly Dictionary<char, Trie> _children = new();

    public void AddWord(string word) => AddWord(word, 0);

    public Trie(string prefix)
    {
        Prefix = prefix;
    }

    private void AddWord(string word, int ix)
    {
        if (ix == word.Length)
        {
            IsWord = true;
            return;
        }

        if (!_children.TryGetValue(word[ix], out Trie child))
        {
           child = new Trie(word[0..(ix+1)]);
           _children[word[ix]] = child;
        }

        child.AddWord(word, ix+1);
    }

    public IEnumerable<char> NextCharacters(string prefix) => NextCharacters(prefix, 0);

    private IEnumerable<char> NextCharacters(string prefix, int ix)
    {
        if (prefix == Prefix) { return _children.Keys; }
        if (ix >= prefix.Length) { return Enumerable.Empty<char>(); }
        if (_children.TryGetValue(prefix[ix], out Trie child))
        {
            return child.NextCharacters(prefix, ix + 1);
        }
        return Enumerable.Empty<char>();
    }

    public IEnumerable<string> AllWords()
    {
        if (IsWord) { yield return Prefix; }
        foreach ((char ch, Trie child) in _children)
        {
            foreach (string word in child.AllWords())
            {
                yield return word;
            }
        }
    }

    public IEnumerable<string> FindWords(IEnumerable<char> letters) => FindWordsClean(letters.Select(ch => char.ToLower(ch)));

    private IEnumerable<string> FindWordsClean(IEnumerable<char> letters)
    {
        if (!letters.Any()) { yield break; }
        if (IsWord) { yield return Prefix; }
        foreach ((char ch, Trie child) in _children.Where(kv => letters.Contains(kv.Key)))
        {
            List<char> nextList = letters.ToList();
            nextList.Remove(ch);
            foreach (string word in child.FindWords(nextList))
            {
                yield return word;
            }
        }
    }

    public IEnumerable<string> NextWords(string prefix) => NextWords(prefix, 0);

    private IEnumerable<string> NextWords(string prefix, int ix)
    {
        if (prefix == Prefix) { return AllWords(); }
        if (_children.TryGetValue(prefix[ix], out Trie child))
        {
            return child.NextWords(prefix, ix + 1);
        }
        return Enumerable.Empty<string>();
    }
}