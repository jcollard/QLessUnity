using System.Collections;
using UnityEngine;
public class CoroutineRunner : MonoBehaviour
{

    public Coroutine Execute(IEnumerator runner) => StartCoroutine(runner);
    public void Halt(Coroutine coroutine) => StopCoroutine(coroutine);

}