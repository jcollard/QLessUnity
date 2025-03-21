using UnityEngine;

public class SlideMenu : MonoBehaviour
{

    [SerializeField]
    private Animator _animator;

    public bool IsShown { get; private set; } = false;

    public void Show()
    {
        IsShown = true;
        _animator.SetTrigger("SlideOut");
    }

    public void Hide()
    {
        IsShown = false;
        _animator.SetTrigger("SlideIn");
    }

    public void Toggle()
    {
        if (IsShown) { Hide(); }
        else { Show(); }
    }

}