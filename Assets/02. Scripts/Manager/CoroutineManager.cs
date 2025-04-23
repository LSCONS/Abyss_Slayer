using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoroutineManager : Singleton<CoroutineManager>
{
    protected override void Awake()
    {
        base.Awake();
    }


    public Coroutine StartCoroutineEnter(IEnumerator enumerator)
    {
        return StartCoroutine(enumerator);
    }


    public void StopCoroutineExit(Coroutine coroutine)
    {
        if(coroutine != null)
        {
            StopCoroutine(coroutine);
        }
    }
}
