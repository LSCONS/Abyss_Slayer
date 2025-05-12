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
    [SerializeField] SpriteRenderer sprite;
    Animator animator;

    private Dictionary<DebuffType, DebuffData> activeDebuffs = new();   // 디버프 상태를 저장
    public float DamageMultiplier { get; set; } = 1f;                   // 보스가 받는 데미지 배율 (기본은 1.0)

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

        float finalDamage = damage * DamageMultiplier;  // 데미지 배율 적용

        if (attackPosX == -1000 || (attackPosX - transform.position.x < 0) == bossController.isLeft)
        {
            ChangeHP(-(int)finalDamage);
            Damaged();
            animator.SetTrigger("Damaged");
        }
        else
        {
            ChangeHP((int)(-finalDamage * 1.1f));
            Damaged();
            animator.SetTrigger("Damaged");
        }
        Debug.Log(Hp.Value);
        //피해입을때 효과,소리

        // attackPosX를 기준으로 화면에 텍스트 띄우기
        Vector3 worldPos = new Vector3(attackPosX, transform.position.y + 1f, transform.position.z);
        DamageTextSpawner.Show((int)finalDamage, worldPos);
    }
    void Damaged()
    {
        CancelInvoke("DamagedEnd");
        sprite.color = Color.red;
        Invoke("DamagedEnd", 0.1f);
    }
    void DamagedEnd()
    {
        sprite.color = Color.white;
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

    /// <summary>
    /// 보스에게 디버프를 적용하는 메서드
    /// </summary>
    /// <param name="type">적용할 디버프 종류</param>
    /// <param name="duration">디버프 지속 시간</param>
    /// <param name="onApply">적용 시 실행할 행동</param>
    /// <param name="onExpire">디버프 끝날 시 실행할 행동</param>
    public void ApplyDebuff(DebuffType type, float duration, Action onApply = null, Action onExpire = null)
    {
        // 이미 존재하는 디버프만 시간 갱신
        if (activeDebuffs.ContainsKey(type))
        {
            activeDebuffs[type].StartTime = Time.time;
            activeDebuffs[type].Duration = duration;
            return;
        }

        // 새 디버프 데이터 생성 및 등록
        var debuff = new DebuffData
        {
            Duration = duration,
            StartTime = Time.time,
            OnApply = onApply,
            OnExpire = onExpire
        };
        activeDebuffs[type] = debuff;

        // 디버프 시작할 때 로직 실행
        debuff.OnApply?.Invoke();

        // 디버프 자동으로 끝내기
        StartCoroutine(RemoveDebuffAfterTime(type, duration));
    }

    /// <summary>
    /// 디버프 끝날 때 자동으로 해제
    /// </summary>
    /// <param name="type">자동으로 해제시킬 디버프 타입</param>
    /// <param name="duration">디버프 지속 시간</param>
    /// <returns></returns>
    private IEnumerator RemoveDebuffAfterTime(DebuffType type, float duration)
    {
        yield return new WaitForSeconds(duration);
        if (activeDebuffs.ContainsKey(type))
        {
            activeDebuffs[type].OnExpire?.Invoke();
            activeDebuffs.Remove(type);
        }
    }

    /// <summary>
    /// 특정 디버프가 지금 적용중인지 확인하는 메서드
    /// </summary>
    /// <param name="type">확인할 디버프 타입</param>
    /// <returns></returns>
    public bool HasDebuff(DebuffType type)
    {
        return activeDebuffs.ContainsKey(type);
    }

    /// <summary>
    /// 디버프 데이터 반환
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public DebuffData GetDebuffData(DebuffType type)
    {
        return activeDebuffs.TryGetValue(type, out var data) ? data : null;
    }
}
