using System;
using System.Collections;
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
    [SerializeField] private TextMeshProUGUI _loadingText;
    [SerializeField] private GameObject _loadingPanel;

    void Awake()
    {
        StartCoroutine(LoadDictionary());
    }

    public IEnumerator LoadDictionary()
    {
        yield return null;
        _validWords = new Dictionary<string, string>();
        string[] rows = Dictionary.text.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
        foreach (string row in rows)
        {
            string word = string.Join("", row.TakeWhile(char.IsLetter).Select(char.ToLowerInvariant));
            string definition = row[word.Length..].Trim();
            if (word.Length < 3) { continue; }
            _validWords[word] = definition;
            Trie.AddWord(word);
            if (_validWords.Count % 1000 == 0)
            {
                _loadingText.text = $"{Mathf.Round((float)_validWords.Count / (float)rows.Length*100)}%";
                yield return null;
            }
        }
        _loadingPanel.gameObject.SetActive(false);
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
            DefinitionText.text = $"{word.ToUpper()}: {definition}";
        }
        else
        {
            Background.color = Invalid;
            DefinitionText.text = string.Empty;
        }
    }

    public string GetDefinition(string word) => $"{word.ToUpper()}: {_validWords[word.Trim().ToLower()]}";

    internal void ShowDefinition(PlacedWord placedWord)
    {
        if (_validWords.TryGetValue(placedWord.Word.Trim().ToLower(), out string definition))
        {
            InputField.text = placedWord.Word.ToLower();
            Background.color = Valid;
            DefinitionText.text = $"{placedWord.Word}: {definition}";
        }
    }
}
