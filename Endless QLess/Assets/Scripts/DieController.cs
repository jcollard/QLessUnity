using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class DieController : MonoBehaviour
{
    public TextMeshPro Label;
    public SpriteRenderer Border;
    public SpriteRenderer SquareRenderer;
    public UnityEvent<DieMovedEvent> OnMove;
    private Vector2Int _position;
    private Vector3 _startPosition;
    public char Face 
    {
        get => Label.text[0];
        set
        {
            Label.text = value.ToString().ToUpper();
            transform.position = _startPosition;
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
        DieMovedEvent movedEvent = new (this, _position, newPosition);
        _position = newPosition;
        OnMove.Invoke(movedEvent);
    }

    void Start()
    {
        _startPosition = transform.position;
    }
}
