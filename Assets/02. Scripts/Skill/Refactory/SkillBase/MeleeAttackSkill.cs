using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttackSkill : Skill
{
    [field: SerializeField] public float Damage { get; private set; } = 10f;
    [field: SerializeField] public float ColliderDuration { get; private set; } = 0.5f;
}
