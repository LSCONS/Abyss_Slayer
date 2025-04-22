using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeAttackSkill : Skill
{
    [field: SerializeField] public int Damage { get; private set; } // 데미지
    [field: SerializeField] public float Range { get; private set; } // 사거리
    [field: SerializeField] public LayerMask TargetLayer { get; private set; } // 타겟 레이어
}