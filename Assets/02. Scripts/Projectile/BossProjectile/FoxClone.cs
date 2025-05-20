using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class FoxClone : BasePoolable,IHasHealth
{
    [SerializeField] Animator animator;
    public ReactiveProperty<int> Hp { get; } = new ReactiveProperty<int>(1);
    public ReactiveProperty<int> MaxHp { get; } = new ReactiveProperty<int>(1);

    int _deadDamage;
    float _deadExplosionScale;
    [SerializeField] DotDamageCollider dotCollidier;
    [SerializeField] SpriteRenderer cloneSprite;
    public delegate void Dead(FoxClone foxClone);
    Dead dead;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    public void Damage(int damage, float attackPosX = -1000)
    {
        Hp.Value = Mathf.Clamp(Hp.Value - damage,0,MaxHp.Value);
        if (Hp.Value<=0)
        {
            animator.SetTrigger("Damaged");
        }
    }
    public override void Init()
    {
    }
    public void Init(Vector3 position, int deadDamage, int explosionDamage, Dead cloneDead, float deadExplosionSize = 1f, int cloneHP = 1)
    {
        Hp.Value = MaxHp.Value;
        transform.position = position;
        _deadDamage = deadDamage;
        dotCollidier.Init(explosionDamage,5);
        dead = cloneDead;
        _deadExplosionScale = deadExplosionSize;
        cloneSprite.flipX = UnityEngine.Random.value < 0.5f;
        MaxHp.Value = cloneHP;
        Hp.Value = MaxHp.Value;
    }
    public void Explosion()
    {
        if(Hp.Value > 0)
        {
            animator.SetTrigger("Explosion");
        }
    }
    public void DeadExplosionDamage()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 5.5f * _deadExplosionScale, LayerData.PlayerLayerMask);
        for(int i = 0; i < hits.Length; i++)
        {
            if(hits[i].TryGetComponent<Player>(out Player hitPlayer))
            {
                hitPlayer.Damage(_deadDamage);
            }
            
        }
    }
    public override void ReturnToPool()
    {
        dead?.Invoke(this);
        base.ReturnToPool();
    }
}
