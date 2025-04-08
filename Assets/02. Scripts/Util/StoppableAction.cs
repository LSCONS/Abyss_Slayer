using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoppableAction
{
    private List<Func<bool>> listeners = new();

    public void AddListener(Func<bool> method)
    {
        listeners.Add(method);
    }

    public void Invoke()
    {
        foreach (var listener in listeners)
        {
            bool isContinue = listener.Invoke();
            if (!(isContinue)) break;
        }
    }
}
