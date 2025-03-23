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
    public Image Background;
    public Color Valid = Color.green;
    public Color Invalid = Color.red;
    public TMP_InputField InputField;
    public TextMeshProUGUI DefinitionText;

    public void Validate(string value)
    {
        if (value.Trim() == string.Empty)
        {
            Background.color = Color.white;
            DefinitionText.text = string.Empty;
        }
        else if (_dictionary.TryGetDefinition(value, out string definition))
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
}
