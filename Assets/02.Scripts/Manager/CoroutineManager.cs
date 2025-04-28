using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoroutineManager : Singleton<CoroutineManager>
{
    public Coroutine StartManagedCoroutine(IEnumerator coroutine)
    {
        return StartCoroutine(coroutine);
    }
}
