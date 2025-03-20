using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private readonly Dictionary<Vector2Int, DieController> _boardData = new();
    public WordCheckerInput WordChecker;
    public DieController[] Dice;
    public DicePoolData DicePool;
    public Color UnusedLetter = Color.gray;
    public Color ValidLetter = Color.white;
    public Color InvalidLetter = Color.red;
    public Color WarningColor = Color.yellow;
    public void FindAllDieControllers()
    {
        Dice = FindObjectsByType<DieController>(FindObjectsSortMode.InstanceID);
        foreach (DieController die in Dice)
        {
            die.OnMove.AddListener(OnDieMoved);
        }
    }

    private void OnDieMoved(DieMovedEvent @event)
    {
        if (_boardData.ContainsKey(@event.To))
        {
            @event.Die.CancelMove();
            return;
        }
        _boardData.Remove(@event.From);
        _boardData[@event.To] = @event.Die;
        ValidateBoard(@event.Die);
    }

    public readonly Vector2Int UpDelta = new(0, 1);
    public readonly Vector2Int DownDelta = new(0, -1);
    public readonly Vector2Int LeftDelta = new(-1, 0);
    public readonly Vector2Int RightDelta = new(1, 0);
    private readonly HashSet<DieController> _validLetters = new();
    private readonly HashSet<DieController> _invalidLetters = new();
    private HashSet<PlacedWord> _placedWords = new();

    private PlacedWord ValidateBoard(DieController lastPlaced)
    {
        ValidateBoard();
        if (_validLetters.Contains(lastPlaced))
        {
            // FindWord(lastPlaced.)
        }
        return default;

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
        foreach (DieController die in Dice)
        {
            die.Face = DicePool.Next();
        }
        _boardData.Clear();
        ValidateBoard();
    }

    void Start()
    {
        FindAllDieControllers();
        Roll();
    }
}
