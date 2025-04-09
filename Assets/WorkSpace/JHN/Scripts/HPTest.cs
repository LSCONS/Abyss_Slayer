using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;



public class HPTest : MonoBehaviour, IHasHealth
{   
    public int hp = 100;
    public int maxHp = 100;
    public event Action<int, int> OnHpChanged;

    private void Update() {
        if (Input.GetKeyDown(KeyCode.H))
        {
            hp = Mathf.Max(hp - 5, 0);
            SetHp(hp, maxHp);
        }

    }
    public void SetHp(int hp, int maxHp)
    {
        this.hp = hp;
        this.maxHp = maxHp;
        OnHpChanged?.Invoke(hp, maxHp);
    }
}
