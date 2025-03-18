using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class DieController : MonoBehaviour
{
    public TextMeshPro Label;
    public SpriteRenderer Border;
    public SpriteRenderer SquareRenderer;
    public UnityEvent<DieMovedEvent> OnMove;
    private Vector2Int _lastPosition;
    private Vector2Int _position;
    private Vector3 _startPosition;
    public char Face
    {
        get => Label.text[0];
        set
        {
            Label.text = value.ToString().ToUpper();
            _position = Vector2Int.RoundToInt(_startPosition);
            UpdatePosition();
        }
    }

    public Color Color
    {
        get => SquareRenderer.color;
        set => SquareRenderer.color = value;
    }

    public void UpdatePosition()
    {
        Vector2Int newPosition = Vector2Int.RoundToInt(transform.position);
        if (newPosition == _position) { return; }
        _lastPosition = _position;
        DieMovedEvent movedEvent = new(this, _position, newPosition);
        _position = newPosition;
        OnMove.Invoke(movedEvent);
    }

    public void CancelMove()
    {
        _position = _lastPosition;
        transform.position = (Vector2)_position;
    }

    void Awake()
    {
        _startPosition = transform.position;
    }
}
