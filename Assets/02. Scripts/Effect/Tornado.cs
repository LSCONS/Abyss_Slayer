using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tornado : BasePoolable
{
    Animator _animator;
    Collider2D _collider;

    int _damage;
    float _durationTime;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _collider = GetComponentInChildren<Collider2D>();
    }
    public override void Init()
    {
        
    }
    public void Init(Vector3 position,int damage, float durationTime, float warningTime = 1f, float width = 1f)
    {
        transform.position = position;
        _damage = damage;
        _durationTime = durationTime;
        _animator.SetFloat("WarningTime", 1 / warningTime);
        transform.localScale = new Vector3(width, 1f, 1f);
    }
    public void StartAttack()
    {
        _collider.enabled = true;
        Invoke("EndTornado",_durationTime);
    }

    public void EndTornado()
    {
        _animator.SetTrigger("TornadoEnd");
    }
    public void EndAttack()
    {
        _collider.enabled = false;
    }
}
