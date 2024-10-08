using TMPro;
using UnityEngine;

public class DieController : MonoBehaviour
{
    public TextMeshPro Label;
    public SpriteRenderer Border;
    public char Face 
    {
        get => Label.text[0];
        set => Label.text = value.ToString().ToUpper();
    }
}
