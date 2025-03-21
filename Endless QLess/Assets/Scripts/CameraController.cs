using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float ScrollSpeed = 0.25f;
    public float MinWidth = 7;
    private float _aspect;
    public bool CancelDrag = false;
    private Vector3 _offSet;

    void Start()
    {
        UpdateAspect();
    }

    public void UpdateAspect()
    {
        if (Camera.main.aspect * Camera.main.orthographicSize * 2 < MinWidth)
        {
            Camera.main.orthographicSize = MinWidth / (Camera.main.aspect * 2);
        }
        _aspect = Camera.main.aspect;
    }

    void Update()
    {
        if (_aspect != Camera.main.aspect)
        {
            UpdateAspect();
        }
        float delta = Input.mouseScrollDelta.y;
        if (Mathf.Abs(delta) > Mathf.Epsilon)
        {
            Camera.main.orthographicSize -= delta * ScrollSpeed;
        }

        if (!CancelDrag && Input.GetMouseButtonDown(0))
        {
            _offSet = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }

        if (!CancelDrag && Input.GetMouseButton(0))
        {
            Vector3 newPosition = Camera.main.transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition) + _offSet;
            newPosition.z = Camera.main.transform.position.z;
            Camera.main.transform.position = newPosition;
            _offSet = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
    }
}