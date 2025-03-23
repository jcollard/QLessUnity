using TMPro;
using UnityEngine;

public class LoadingPanel : MonoBehaviour
{
    [SerializeField] private DictionaryData _dictionary;
    [SerializeField] private TextMeshProUGUI _loadingText;
    [SerializeField] private TextMeshProUGUI _percentageText;
    [SerializeField] private int _wordsPerFrame = 10_000;
    [SerializeField] private GameObject _panel;

    void Awake() => _dictionary.LoadAsync(_wordsPerFrame, HandleLoadInfo);

    private void HandleLoadInfo(DictionaryDataEvent @event)
    {
        switch (@event)
        {
            case DictionaryStillLoading:
                _panel.SetActive(true);
                break;
            case DictionaryLogEvent(string message):
                _loadingText.text = message;
                break;
            case DictionaryLoadProgress(float progress):
                _percentageText.text = $"{progress}%";
                break;
            case DictionaryLoadComplete:
                gameObject.SetActive(false);
                break;
        }
    }

}