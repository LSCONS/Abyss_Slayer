using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class Boss : MonoBehaviour, IHasHealth
{
    public BossController bossController;
    public ReactiveProperty<int> Hp { get; } = new ReactiveProperty<int>(1000);
    public ReactiveProperty<int> MaxHp { get; } = new ReactiveProperty<int> (1000);
    public bool isDead;
    Action bossDeath;
    Animator animator;

    [SerializeField] int maxHP;
    public void ChangeHP(int value)
    {
        Hp.Value = Mathf.Clamp(Hp.Value + value, 0, MaxHp.Value);
        if (Hp.Value == 0)
        {
            isDead = true;
            bossDeath?.Invoke();
        }
    }
    public void AddBossDeathAction(Action deatAction)
    {
        bossDeath += deatAction;
    }
    /// <summary>
    /// 데미지(양수) 입히기
    /// </summary>
    /// <param name="damage">입힐 데미지</param>
    /// <param name="attackPosX">데미지 입히는 주체의 위치X값</param>
    public void Damage(int damage, float attackPosX = -1000)
    {
        if (isDead)
        {
            return;
        }
        if (attackPosX == -1000 || (attackPosX - transform.position.x < 0) == bossController.isLeft)
        {
            ChangeHP(-damage);
            animator.SetTrigger("Damaged");
        }
        else
        {
            ChangeHP((int)(-damage * 1.1f));
        }
        Debug.Log(Hp.Value);
        //피해입을때 효과,소리
    }

    private void Awake()
    {
        MaxHp.Value = maxHP;
        Hp.Value = MaxHp.Value;
        isDead = false;
        bossController = GetComponent<BossController>();
        animator = GetComponent<Animator>();
        AddBossDeathAction(bossController.OnDead);
    }
}
