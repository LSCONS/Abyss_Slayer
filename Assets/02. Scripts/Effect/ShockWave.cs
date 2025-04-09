using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShockWave : BasePoolable
{
    float _height;
    int _damage;
    public override void Init()
    {
    }
    public void Init(float height, int damage)
    {
        _height = height;
        _damage = damage;
    }
}
