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
    [SerializeField] private TextMeshProUGUI _percentageText;
    [SerializeField] private GameObject _loadingPanel;
    private bool _dictionaryLoaded = false;
    private bool _trieLoaded = false;
    [SerializeField] private int _loadsPerFrame = 10_000;

    void Awake()
    {
        StartCoroutine(LoadDictionary());
    }

    public IEnumerator LoadDictionary()
    {
        _loadingText.text = "Loading Dictionary...";
        yield return null;
        _validWords = new Dictionary<string, string>();
        string[] rows = Dictionary.text.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
        foreach (string row in rows)
        {
            string word = string.Join("", row.TakeWhile(char.IsLetter).Select(char.ToLowerInvariant));
            if (word.Length < 3) { continue; }
            _validWords[word] = row[word.Length..].Trim();
            Trie.AddWord(word);
            if (_validWords.Count % _loadsPerFrame == 0)
            {
                _percentageText.text = $"{Mathf.Round((float)_validWords.Count / (float)rows.Length*100)}%";
                yield return null;
            }
        }
        _percentageText.text = $"100%";
        yield return null;
        _dictionaryLoaded = true;
        _trieLoaded = true;
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
