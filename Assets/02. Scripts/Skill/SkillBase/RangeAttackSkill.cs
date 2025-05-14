using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeAttackSkill : Skill
{
    [field: Header("초기 데미지")]
    [field: SerializeField] public float BaseDamage { get; private set; } // 초기 데미지

    [field: Header("최종 데미지")]
    [field: SerializeField] public float Damage { get; private set; } // 데미지

    [field: Header("스킬을 적용할 타겟 레이어")]
    [field: SerializeField] public LayerMask TargetLayer { get; private set; } // 타겟 레이어

    public override void SkillUpgrade()
    {
        base.SkillUpgrade();
        Damage = BaseDamage * (1.0f + (Level.Value - 1) * Magnification);
    }
}
