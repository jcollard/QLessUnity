using UnityEngine;
using UnityEngine.EventSystems;

public class Draggable : MonoBehaviour
{
    private float _startX;

    public Vector2 MousePosition => Camera.main.ScreenToWorldPoint(Input.mousePosition);
    public Vector2 Offset;

    void OnMouseDown()
    {
        Offset = (Vector2)transform.position - MousePosition;
    }

    void OnMouseDrag()
    {
        transform.position = MousePosition + Offset;
    }
}
