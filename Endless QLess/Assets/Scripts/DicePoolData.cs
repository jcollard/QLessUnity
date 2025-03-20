using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DieData", menuName = "Scriptable Objects/DieData")]
public class DicePoolData : ScriptableObject
{
    public string[] Faces;
    private int _ix = 0;

    public char Next()
    {        
        _ix %= Faces.Length;
        char ch = Faces[_ix][Random.Range(0, Faces[_ix].Length)];
        _ix++;
        return ch;
    }
}