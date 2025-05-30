using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttackSkill : Skill
{
    [field: Header("기존 데미지")]
    [field: SerializeField] public float BaseDamage { get; private set; } = 45f;

    [field: Header("틱당 데미지")]
    [field: SerializeField] public float Damage { get; private set; } = 45f;

    [field: Header("콜라이더 유지 시간")]
    [field: SerializeField] public float ColliderDuration { get; private set; } = 0.2f;

    [field: Header("타겟팅할 레이어")]
    [field: SerializeField] public LayerMask TargetMask { get; private set; }

    [field: Header("히트 이펙트 타입")]
    [field: SerializeField] public Type EffectType { get; private set; }

    public override void SkillUpgrade()
    {
        base.SkillUpgrade();
        Damage = BaseDamage * (1.0f + (Level.Value - 1) * Magnification);
    }
}
