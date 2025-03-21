using UnityEngine;
using UnityEngine.Events;

public class Draggable : MonoBehaviour
{

    public Vector2 MousePosition => Camera.main.ScreenToWorldPoint(Input.mousePosition);
    public Vector2 Offset;
    public UnityEvent OnDragStarted;
    public UnityEvent OnSnap;

    void OnMouseDown()
    {
        Offset = (Vector2)transform.position - MousePosition;
        OnDragStarted.Invoke();
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
        OnSnap.Invoke();
    }

    void OnMouseUp() => Snap();
}
