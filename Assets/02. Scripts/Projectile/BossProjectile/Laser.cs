using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : BasePoolable
{
    int _damage;
    Vector3 _direction;
    float _warningTime;
    float _width;

    public override void Init()
    {
    }
    public void Init(int damage, Vector3 position, Vector3 direction,float width =1f, float warningTime = 0.5f)
    {
        _damage = damage;
        transform.position = position;
        float angle = Vector3.Angle(Vector3.right, direction);
        transform.rotation = Quaternion.Euler(0,0,angle);
    }
}
