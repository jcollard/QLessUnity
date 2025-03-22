using UnityEngine;

public class HUDController : MonoBehaviour
{
    public Canvas NonMobileCanvas;
    public Canvas MobileCanvas;
    void Start()
    {
        #if UNITY_ANDROID
        transform.SetParent(MobileCanvas.transform);
        RectTransform rect = (RectTransform)transform;
        rect.localScale = new (1, 1, 1);
        rect.anchorMin = new(0, 0);
        rect.anchorMax = new(1, 1);
        rect.pivot = new (0.5f, 0.5f);
        rect.offsetMin = new (0, 0);
        rect.offsetMax = new (0, 0);
        NonMobileCanvas.gameObject.SetActive(false);
        #endif
    }
}
