using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "DictionaryData", menuName = "WordsWithDice/DictionaryData")]
public class DictionaryData : ScriptableObject
{
    [SerializeField] private TextAsset DictionaryCSV;
    private readonly Dictionary<string, string> _words = new();
    public bool IsLoaded { get; private set; }
    public Trie Trie { get; private set; } = new(string.Empty);

    public string this[string word] => _words[Sanitize(word)];

    private string Sanitize(string word) => word.Trim().ToUpper();

    public IEnumerator LoadAsync(int wordsPerFrame = 10_000, Action<DictionaryDataEvent> observerCallback = null)
    {
        if (IsLoaded) 
        { 
            observerCallback?.Invoke(DictionaryLoadComplete.Instance);
            yield break; 
        }
        observerCallback?.Invoke(DictionaryLoadStarted.Instance);
        observerCallback?.Invoke(new DictionaryLogEvent("Loading Dictionary..."));
        _words.Clear();
        string[] rows = DictionaryCSV.text.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
        foreach (string row in rows)
        {
            string word = Sanitize(string.Join("", row.TakeWhile(char.IsLetter)));
            if (word.Length < 3) { continue; }
            _words[word] = $"{word}: {row[word.Length..].Trim()}";
            Trie.AddWord(word);
            if (_words.Count % wordsPerFrame == 0)
            {
                observerCallback?.Invoke(new DictionaryLoadProgress(Mathf.Round((float)_words.Count / (float)rows.Length*100)));
                yield return null;
            }
        }

        observerCallback?.Invoke(DictionaryLoadComplete.Instance);
        IsLoaded = true;
    }

    public void LoadSync()
    {
        IEnumerator load = LoadAsync(1_000_000);
        while(load.MoveNext());
    }

    public bool IsValid(string word)
    {
        if (!IsLoaded) { LoadSync(); }
        return _words.ContainsKey(Sanitize(word));
    }

    public bool TryGetDefinition(string word, out string definition) => _words.TryGetValue(_words[Sanitize(word)], out definition);
}



