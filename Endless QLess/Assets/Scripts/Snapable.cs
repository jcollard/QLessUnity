using UnityEngine;

public class Snapable : MonoBehaviour
{
    public Color SnapColor = Color.red;
    public DieController Die;
    public string SnapsTo;
    public bool IsConnected = false;
    public Snapable _snapping;
    private Color _resetColor;

    public void OnTriggerEnter2D(Collider2D other)
    {
        if((other.GetComponent<Snapable>() is Snapable otherSnap) && other.tag == SnapsTo && otherSnap.IsConnected == false)
        {
            _resetColor = Die.Border.color;
            Die.Border.color = SnapColor;
            otherSnap.Die.Border.color = SnapColor;
            _snapping = otherSnap;
            IsConnected = true;
            otherSnap.IsConnected = true;
        }
    }

    public void OnTriggerExit2D(Collider2D other)
    {
        Debug.Log("Exit");
        if ((other.GetComponent<Snapable>() is Snapable otherSnap) && otherSnap == _snapping && IsConnected)
        {
            Die.Border.color = _resetColor;
            otherSnap.Die.Border.color = _resetColor;
            _snapping = null;
            IsConnected = false;
            otherSnap.IsConnected = false;
        }
        
    }
   
}
