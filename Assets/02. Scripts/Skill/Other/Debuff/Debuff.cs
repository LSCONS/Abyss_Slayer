using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DebuffType
{
    Stun,
    Weaken,
    Slow
}

// 디버프 데이터 클래스
public class DebuffData
{
    public float Duration;      // 디버프 지속 시간
    public float StartTime;     // 디버프 시작 시간
    public Action OnApply;      // 디버프 처음 적용되면 실행될 액션
    public Action OnExpire;     // 디버프 해제되면 실행될 액션
}

/// <summary>
/// 디버프 효과 인터페이스
/// </summary>
public interface IDebuff
{
    void Apply(Boss boss, float amount);
    void Expire(Boss boss);
}

public class WeakenDebuff : IDebuff
{
    public void Apply(Boss boss, float amount)
    {
        // 보스 받는 데미지 배율 증가
        boss.DamageMultiplier = amount;
        Debug.Log($"amount: {amount}");
    }

    public void Expire(Boss boss)
    {
        // 보스 받는 데미지 배율 되돌리기
        boss.DamageMultiplier = 1.0f;
        Debug.Log($"디버프 끝!");

    }
}
