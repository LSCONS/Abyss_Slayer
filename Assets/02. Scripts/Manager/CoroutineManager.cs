using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoroutineManager : Singleton<CoroutineManager>
{
    protected override void Awake()
    {
        base.Awake();
    }


    public void StartCoroutineManager(Coroutine coroutine, IEnumerator enumerator)
    {
        coroutine = StartCoroutine(enumerator);
    }
}
