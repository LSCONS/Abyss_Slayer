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

    public IDebuff debuff;      // 실제 디버프 정보
    public string Name => debuff?.Info.Name ?? "알 수 없음";
    public string Description => debuff?.Info.Description ?? "정보 없음";
}

public struct DebuffInfo
{
    public string Name;
    public string Description;

    public DebuffInfo(string name, string description)
    {
        Name = name;
        Description = description;
    }
}

/// <summary>
/// 디버프 효과 인터페이스
/// </summary>
public interface IDebuff
{
    DebuffInfo Info { get; }    // 디버프의 이름과 설명

    void Apply(Boss boss, float amount);
    void Expire(Boss boss);
}

public class WeakenDebuff : IDebuff
{
    public DebuffInfo Info => new DebuffInfo("약화", "받는 피해량이 증가합니다.");
    public void Apply(Boss boss, float amount)
    {
        // 보스 받는 데미지 배율 증가
        boss.DamageMultiplier = amount;
    }

    public void Expire(Boss boss)
    {
        // 보스 받는 데미지 배율 되돌리기
        boss.DamageMultiplier = 1.0f;
    }
}
