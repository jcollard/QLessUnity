using UnityEngine;
using UnityEngine.EventSystems;

public class Draggable : MonoBehaviour
{
    private float _startX;

    public Vector2 MousePosition => Camera.main.ScreenToWorldPoint(Input.mousePosition);
    public Vector2 Offset;

    void Start()
    {
        // Snap();
    }

    void OnMouseDown()
    {
        Offset = (Vector2)transform.position - MousePosition;
    }

    void OnMouseDrag()
    {
        transform.position = MousePosition + Offset;
    }

    public void Snap()
    {
        Vector3 position = transform.position;

        position.x = Mathf.Round(position.x);
        position.y = Mathf.Round(position.y);
        transform.position = position;
    }

    void OnMouseUp() => Snap();
}
