using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WordCheckerInput : MonoBehaviour
{
    [SerializeField] private DictionaryData _dictionary;
    [SerializeField] private TextMeshProUGUI _loadingText;
    [SerializeField] private TextMeshProUGUI _percentageText;
    [SerializeField] private GameObject _loadingPanel;
    [SerializeField] private int _wordsPerFrame = 10_000;
    public Image Background;
    public Color Valid = Color.green;
    public Color Invalid = Color.red;
    public TMP_InputField InputField;
    public TextMeshProUGUI DefinitionText;


    void Awake() => LoadDictionary();

    public void LoadDictionary()
    {
        StartCoroutine(_dictionary.LoadAsync(_wordsPerFrame, HandleLoadInfo));
        // _loadingText.text = "Loading Dictionary...";
        // yield return null;
        // _validWords = new Dictionary<string, string>();
        // string[] rows = Dictionary.text.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
        // foreach (string row in rows)
        // {
        //     string word = string.Join("", row.TakeWhile(char.IsLetter).Select(char.ToLowerInvariant));
        //     if (word.Length < 3) { continue; }
        //     _validWords[word] = row[word.Length..].Trim();
        //     Trie.AddWord(word);
        //     if (_validWords.Count % _wordsPerFrame == 0)
        //     {
        //         _percentageText.text = $"{Mathf.Round((float)_validWords.Count / (float)rows.Length*100)}%";
        //         yield return null;
        //     }
        // }
        // _percentageText.text = $"100%";
        // yield return null;
        // _dictionaryLoaded = true;
        // _trieLoaded = true;
        // _loadingPanel.gameObject.SetActive(false);

    }

    private void HandleLoadInfo(DictionaryDataEvent @event)
    {
        switch (@event)
        {
            case DictionaryLoadStarted:
                _loadingPanel.gameObject.SetActive(true);
                break;
            case DictionaryLogEvent(string message):
                _loadingText.text = message;
                break;
            case DictionaryLoadProgress(float progress):
                _percentageText.text = $"{progress}%";
                break;
            case DictionaryLoadComplete:
                _loadingPanel.gameObject.SetActive(false);
                break;
        }
    }

    public bool IsValid(string value) => _dictionary.IsValid(value);

    public void Validate(string value)
    {
        string word = value.Trim().ToLower();

        if (value == string.Empty)
        {
            Background.color = Color.white;
            DefinitionText.text = string.Empty;
        }
        else if (_dictionary.TryGetDefinition(word, out string definition))
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

    public string GetDefinition(string word) => _dictionary[word];

    internal void ShowDefinition(PlacedWord placedWord)
    {
        if (_dictionary.TryGetDefinition(placedWord.Word, out string definition))
        {
            InputField.text = placedWord.Word.ToLower();
            Background.color = Valid;
            DefinitionText.text = definition;
        }
    }
}
