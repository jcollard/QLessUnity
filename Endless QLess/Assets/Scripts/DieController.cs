using TMPro;
using UnityEngine;

public class DieController : MonoBehaviour
{
    public TextMeshPro Label;
    public char Face 
    {
        get => Label.text[0];
        set => Label.text = value.ToString().ToUpper();
    }
}
