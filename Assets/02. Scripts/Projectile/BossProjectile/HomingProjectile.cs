using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingProjectile : BasePoolable
{
    bool inited;

    int _damage;
    Transform _target;
    float _speed;
    float _delayFireTime;
    float _homingPower;

    private void Update()
    {
        if (inited)
        {
            Move();
            Rotate();
        }
    }

    public override void Init()
    {
    }
    public void Init(int damage, Vector3 position, Vector3 rotate, Transform target,float speed, float delayFireTime = 0f, float homingPower = 1f)
    {
        _damage = damage;
        _target = target;
        _speed = speed;
        _delayFireTime = delayFireTime;
        _homingPower = homingPower;
    }

    void Move()
    {

    }
    void Rotate()
    {

    }
}
