using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShockWave : BasePoolable
{
    int _damage;
    [SerializeField] Collider2D _collider;
    [SerializeField] NormalDamageCollider _bossDamageCollider;
    public override void Init()
    {
    }
    public void Init(Vector3 position, int damage, float height = 1f, float width = 1f)
    {
        transform.position = position;
        _damage = damage;
        transform.localScale = new Vector3(width, height, 1);

        _bossDamageCollider.Init(_damage, null, int.MaxValue);
    }

    public void StartAnimation()
    {
        _collider.enabled = true;
    }
    public void StopDamage()
    {
        _collider.enabled = false;
    }

}
