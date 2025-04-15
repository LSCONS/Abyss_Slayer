using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UniRx;


public class HPTest : MonoBehaviour, IHasHealth
{   
    public ReactiveProperty<int> Hp { get; private set; } = new ReactiveProperty<int>(100);
    public ReactiveProperty<int> MaxHp { get; private set; } = new ReactiveProperty<int>(100);

    private void Update() {
        if (Input.GetKeyDown(KeyCode.H))
        {
            Hp.Value = Mathf.Max(Hp.Value - 5, 0);
        }
    }
}
