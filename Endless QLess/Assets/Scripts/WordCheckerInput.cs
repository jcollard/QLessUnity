using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WordCheckerInput : MonoBehaviour
{
    public Image Background;
    public Color Valid = Color.green;
    public Color Invalid = Color.red;
    public TextAsset Dictionary;
    private Dictionary<string, string> _validWords;
    public TMP_InputField InputField;
    public TextMeshProUGUI DefinitionText;
    public Trie Trie { get; private set; } = new(string.Empty);

    void Awake()
    {
        _validWords = new Dictionary<string, string>();
        foreach (string row in Dictionary.text.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries))
        {
            string word = row.Split(new char[0])[0].Trim().ToLower();
            if (word.Length < 3) { continue; }
            _validWords[word] = row;
            // Trie.AddWord(word);
        }
    }

    public bool IsValid(string value) => value.Length > 2 && _validWords.ContainsKey(value.ToLower());

    public void Validate(string value)
    {
        string word = value.Trim().ToLower();
        
        if (value == string.Empty)
        {
            Background.color = Color.white;
            DefinitionText.text = string.Empty;
        }
        else if (_validWords.TryGetValue(word, out string definition))
        {
            Background.color = Valid;
            DefinitionText.text = definition;
        }
        else
        {
            Background.color = Invalid;
            DefinitionText.text = string.Empty;
        }
    }

    public string GetDefinition(string word) => _validWords[word.Trim().ToLower()];

    internal void ShowDefinition(PlacedWord placedWord)
    {
        if (_validWords.TryGetValue(placedWord.Word.Trim().ToLower(), out string definition))
        {
            InputField.text = placedWord.Word.ToLower();
            Background.color = Valid;
            DefinitionText.text = definition;
        }
    }
}
