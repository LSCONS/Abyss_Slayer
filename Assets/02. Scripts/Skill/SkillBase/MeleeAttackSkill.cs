using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttackSkill : Skill
{
    [field: Header("틱당 데미지")]
    [field: SerializeField] public float Damage { get; private set; } = 10f;
    [field: Header("콜라이더 유지 시간")]
    [field: SerializeField] public float ColliderDuration { get; private set; } = 0.5f;
}
