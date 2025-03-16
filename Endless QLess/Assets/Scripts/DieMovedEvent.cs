using UnityEngine;

public record struct DieMovedEvent(DieController Die, Vector2Int From, Vector2Int To);