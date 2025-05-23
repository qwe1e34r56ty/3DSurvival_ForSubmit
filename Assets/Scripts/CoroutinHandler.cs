using System.Collections;
using UnityEngine;

// Monobehaviour 없는 Entity들이 Coroutine을 호출하기 위해 접근 
public class CoroutineHandler : MonoBehaviour
{
    public Coroutine RunCoroutine(IEnumerator routine)
    {
        return StartCoroutine(routine);
    }

    public void StopRunningCoroutine(Coroutine coroutine)
    {
        if (coroutine != null)
            StopCoroutine(coroutine);
    }
}
