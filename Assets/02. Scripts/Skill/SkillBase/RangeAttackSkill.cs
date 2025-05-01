using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeAttackSkill : Skill
{
    [field: Header("스킬 데미지")]
    [field: SerializeField] public int Damage { get; private set; } // 데미지
    [field: Header("스킬을 적용할 타겟 레이어")]
    [field: SerializeField] public LayerMask TargetLayer { get; private set; } // 타겟 레이어
}
