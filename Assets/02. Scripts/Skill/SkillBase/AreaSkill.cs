using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AreaSkill : Skill
{
    [field: Header("범위 (원 반지름)")]
    [SerializeField] protected float radius = 5f; // 범위 반경
    [field: Header("대상 레이어")]
    [SerializeField] protected LayerMask targetLayer; // 대상 레이어
    [field: Header("스킬이 적용할 효과 수치 (디버프의 강도 등)")]
    [SerializeField] protected float BaseEffectAmount = 1.2f;
    [SerializeField] protected float effectAmount = 1.2f;

    /// <summary>
    /// 범위 내 타겟 추출
    /// </summary>
    protected Collider2D[] GetTargetsInArea()
    {
        return Physics2D.OverlapCircleAll(PlayerPosition(), radius, targetLayer);
    }

    public override void SkillUpgrade()
    {
        base.SkillUpgrade();
        effectAmount = BaseEffectAmount * (1.0f + (Level.Value -1) * Magnification);
    }
}
