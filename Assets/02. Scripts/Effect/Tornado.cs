using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tornado : BasePoolable
{
    Animator _animator;
    Collider2D _collider;
    [SerializeField] DotDamageCollider _damageColliderScript;

    int _damage;
    float _durationTime;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _collider = GetComponentInChildren<Collider2D>();
        
    }
    public override void Rpc_Init()
    {

    }
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void Rpc_Init(Vector3 position,int damage, float durationTime,float attackPerSec, float warningTime = 1f, float width = 4f, float hight = 20)
    {
        gameObject.SetActive(true);
        transform.position = position;
        _damage = damage;
        _durationTime = durationTime;
        _animator.SetFloat("WarningTime", 1 / warningTime);
        transform.localScale = new Vector3(width/4, hight/20, 1f);

        _damageColliderScript.Init(_damage, attackPerSec);
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
