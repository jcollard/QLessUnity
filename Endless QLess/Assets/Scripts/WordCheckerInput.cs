using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class WordCheckerInput : MonoBehaviour
{
    public Image Background;
    public Color Valid = Color.green;
    public Color Invalid = Color.red;
    public TextAsset Dictionary;
    private HashSet<string> _validWords;

    void Awake()
    {
        _validWords = new HashSet<string>();
        _validWords = Dictionary.text.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim().ToLower()).ToHashSet();
    }

    public bool IsValid(string value) => value.Length > 2 && _validWords.Contains(value.ToLower());

    public void Validate(string value)
    {
        string word = value.Trim().ToLower();
        Color color = word switch {
            "" => Color.white,
            _ when _validWords.Contains(word) => Valid,
            _ => Invalid
        };
        Background.color = color;
    }
}
