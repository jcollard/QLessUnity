using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;


#if UNITY_WEBGL
#pragma warning disable IDE0005 // Using directive is unnecessary.
using System.Runtime.InteropServices;
#pragma warning restore IDE0005 // Using directive is unnecessary.
#endif

public class GameManager : MonoBehaviour
{
    [SerializeField] private CameraController _cameraController;
    private readonly Dictionary<Vector2Int, DieController> _boardData = new();
    [SerializeField] private TMP_InputField _seedInput;
    [SerializeField] private TextMeshProUGUI _definition;
    public WordCheckerInput WordChecker;
    public DieController[] Dice;
    public DicePoolData DicePool;
    public Color UnusedLetter = Color.gray;
    public Color ValidLetter = Color.white;
    public Color InvalidLetter = Color.red;
    public Color WarningColor = Color.yellow;
    public readonly List<string> _possibleWords = new();
    public readonly Vector2Int UpDelta = new(0, 1);
    public readonly Vector2Int DownDelta = new(0, -1);
    public readonly Vector2Int LeftDelta = new(-1, 0);
    public readonly Vector2Int RightDelta = new(1, 0);
    private readonly HashSet<DieController> _validLetters = new();
    private readonly HashSet<DieController> _invalidLetters = new();
    private HashSet<PlacedWord> _placedWords = new();

#if UNITY_WEBGL && !UNITY_EDITOR
    //
    // WebGL
    //
    [DllImport("__Internal")]
    //(gameObject, callbackMethod)
    private static extern void GetSeedParam(string gameObjectName, string methodName);
    [DllImport("__Internal")]
    private static extern void SetSeedParam(string seed);

    // Broser plugin should be called in OnPointerDown.
    public bool LoadSeed() {
        GetSeedParam(gameObject.name, nameof(UpdateSeed));
        return true;
    }

    // Called from browser
    public void SetSeed() {
        string seed = _seedInput.text;
        SetSeedParam(seed);
    }
#else
    public bool LoadSeed() => false;
    public void SetSeed() { }
#endif 

    public void UpdateSeed(string seed)
    {
        if (seed == null || seed.Trim() == string.Empty)
        {
            Roll();
            return;
        }
        Roll(seed);
    }

    public void FindAllDieControllers()
    {
        Dice = FindObjectsByType<DieController>(FindObjectsSortMode.InstanceID);
        foreach (DieController die in Dice)
        {
            die.OnMove.AddListener(OnDieMoved);
            Draggable draggable = die.GetComponent<Draggable>();
            draggable.OnDragStarted.AddListener(DisableCameraDrag);
            draggable.OnSnap.AddListener(EnableCameraDrag);
        }
    }

    private void DisableCameraDrag() => _cameraController.CancelDrag = true;
    private void EnableCameraDrag() => _cameraController.CancelDrag = false;

    private void OnDieMoved(DieMovedEvent @event)
    {
        if (_boardData.ContainsKey(@event.To))
        {
            @event.Die.CancelMove();
            return;
        }
        _boardData.Remove(@event.From);
        _boardData[@event.To] = @event.Die;
        ValidateBoard();
    }

    private void ValidateBoard()
    {
        _validLetters.Clear();
        _invalidLetters.Clear();
        HashSet<PlacedWord> previousWords = _placedWords.ToHashSet();
        _placedWords.Clear();
        foreach ((Vector2Int position, DieController die) in _boardData)
        {
            if (!_boardData.ContainsKey(position + LeftDelta) && _boardData.ContainsKey(position + RightDelta))
            {
                var word = FindWord(position, RightDelta);
                if (word.IsValid) { _validLetters.UnionWith(word.Dice); }
                else { _invalidLetters.UnionWith(word.Dice); }
                _placedWords.Add(word);
            }
            if (!_boardData.ContainsKey(position + UpDelta) && _boardData.ContainsKey(position + DownDelta))
            {
                var word = FindWord(position, DownDelta);
                if (word.IsValid) { _validLetters.UnionWith(word.Dice); }
                else { _invalidLetters.UnionWith(word.Dice); }
                _placedWords.Add(word);
            }
        }

        IEnumerable<PlacedWord> newWords = _placedWords.Where(w => !previousWords.Contains(w) && w.IsValid);
        if (newWords.Any())
        {
            WordChecker.ShowDefinition(newWords.First());
            _definition.text = WordChecker.GetDefinition(newWords.First().Word);
        }

        foreach (DieController die in _boardData.Values)
        {
            die.Color = (_validLetters.Contains(die), _invalidLetters.Contains(die)) switch
            {
                (true, true) => WarningColor,
                (true, _) => ValidLetter,
                (_, true) => InvalidLetter,
                _ => UnusedLetter,
            };
        }

    }

    private readonly StringBuilder _builder = new();

    private PlacedWord FindWord(Vector2Int position, Vector2Int delta)
    {
        Vector2Int start = position;
        List<DieController> dice = new();
        _builder.Clear();
        while (_boardData.TryGetValue(position, out DieController die))
        {
            _builder.Append(die.Face);
            dice.Add(die);
            position += delta;
        }
        string word = _builder.ToString();
        return new PlacedWord(word, start, delta, dice.ToArray(), WordChecker.IsValid(word));
    }

    public void Roll()
    {
        System.Random rng = new System.Random();
        _builder.Clear();
        for (int i = 0; i < 16; i++)
        {
            if (i > 0 && i % 4 == 0) { _builder.Append('-'); }
            _builder.Append((char)('A' + rng.Next(26)));
        }
        _seedInput.text = _builder.ToString();
        Roll(_seedInput.text);
        _cameraController.Reset();
        // _possibleWords.Clear();
        // _possibleWords.AddRange(WordChecker.Trie.FindWords(Dice.Select(d => d.Face)).OrderByDescending(s => s.Length));
        // Debug.Log($"{_possibleWords.Count} words: {string.Join(", ", _possibleWords)}");
    }

    public void RollSeed() => Roll(_seedInput.text);

    private void SetDiceTo(string values)
    {
        for (int i = 0; i < values.Length; i++)
        {
            Dice[i].Face = values[i];
            Dice[i].Color = UnusedLetter;
        }
    }

    public void Roll(string seed)
    {
        _seedInput.text = seed.ToString();
        if (seed.Length == 12 && seed.Where(char.IsLetter).Count() == 12)
        {
            SetDiceTo(seed);
        }
        else
        {
            UnityEngine.Random.InitState(seed.GetHashCode());
            foreach (DieController die in Dice)
            {
                die.Face = DicePool.Next();
                die.Color = UnusedLetter;
            }
        }
        _boardData.Clear();
        ValidateBoard();
        _definition.text = string.Empty;
        SetSeed();
    }

    void Start()
    {
        FindAllDieControllers();
        if (!LoadSeed())
        {
            Roll();
        }
    }

    [SerializeField]
    private TMP_InputField _trieLetters;

    public void FindLetters(string input)
    {
        input = input.Trim().ToLower();
        var nextWords = WordChecker.Trie.NextWords(input);
        if (!nextWords.Any())
        {
            Debug.Log("No words");
        }
        else
        {
            Debug.Log(string.Join(", ", nextWords));
        }
    }
}
