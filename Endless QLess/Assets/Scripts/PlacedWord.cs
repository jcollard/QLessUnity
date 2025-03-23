using System;
using UnityEngine;

public record struct PlacedWord(string Word, Vector2Int StartPosition, Vector2Int Delta, DieController[] Dice, bool IsValid) : IEquatable<PlacedWord>
{
    public bool Equals(PlacedWord other) => Word == other.Word && StartPosition == other.StartPosition && Delta == other.Delta;
    public override int GetHashCode() => HashCode.Combine(Word, StartPosition.x, StartPosition.y, Delta.x, Delta.y);
}