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
    public bool IsLoaded
    {
        get; private set;
    }
    public Trie Trie { get; private set; } = new(string.Empty);
    public string this[string word]
    {
        get
        {
            if (!IsLoaded) { LoadSync(); }
            return _words[Sanitize(word)];
        }
    }

    private string Sanitize(string word) => word.Trim().ToUpper();
    private Coroutine _loading;
    private CoroutineRunner _runner;
    private Action<DictionaryDataEvent> _onLoadEvent;
    private readonly List<DictionaryDataEvent> _eventPlayback = new();

    void OnEnable()
    {
        IsLoaded = false; 
        _onLoadEvent = _eventPlayback.Add;
        _eventPlayback.Clear();
    }

    private CoroutineRunner GetRunner()
    {
        _runner ??= FindFirstObjectByType<CoroutineRunner>();
        if (_runner == null)
        {
            GameObject runner = new("CoroutineRunner", typeof(CoroutineRunner));
            _runner = runner.GetComponent<CoroutineRunner>();
        }
        return _runner;
    }

    public Coroutine LoadAsync(int wordsPerFrame = 10_000, Action<DictionaryDataEvent> observerCallback = null) => LoadAsync(GetRunner(), wordsPerFrame, observerCallback);

    public Coroutine LoadAsync(CoroutineRunner runner, int wordsPerFrame = 10_000, Action<DictionaryDataEvent> observerCallback = null)
    {
        if (observerCallback != null)
        {
            _onLoadEvent += observerCallback;
            foreach (var @event in _eventPlayback) { observerCallback.Invoke(@event); }
        }
        return _loading ??= runner.Execute(LoadAsyncRoutine(10_000));
    }

    private IEnumerator LoadAsyncRoutine(int wordsPerFrame = 10_000)
    {
        if (IsLoaded)
        {
            _onLoadEvent?.Invoke(DictionaryLoadComplete.Instance);
            yield break;
        }
        _onLoadEvent?.Invoke(DictionaryLoadStarted.Instance);
        _onLoadEvent?.Invoke(new DictionaryLogEvent("Loading Dictionary..."));
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
                _onLoadEvent?.Invoke(new DictionaryLoadProgress(Mathf.Round((float)_words.Count / (float)rows.Length * 100)));
                yield return null;
            }
        }

        _onLoadEvent?.Invoke(DictionaryLoadComplete.Instance);
        IsLoaded = true;
    }

    public void LoadSync()
    {
        IEnumerator load = LoadAsyncRoutine(1_000_000);
        while (load.MoveNext()) ;
    }

    public bool IsValid(string word)
    {
        if (!IsLoaded) { LoadSync(); }
        return _words.ContainsKey(Sanitize(word));
    }

    public bool TryGetDefinition(string word, out string definition)
    {
        if (!IsLoaded) { LoadSync(); }
        return _words.TryGetValue(Sanitize(word), out definition);
    }

    private WaitUntil _waitUntilLoaded;
    public WaitUntil WaitUntilLoaded()
    {
        LoadAsync();
        _onLoadEvent.Invoke(DictionaryStillLoading.Instance);
        return _waitUntilLoaded ??= new WaitUntil(() => IsLoaded);;
    }
}
