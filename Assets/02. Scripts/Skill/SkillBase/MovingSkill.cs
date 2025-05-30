using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingSkill : Skill
{
    [field: Header("대시에 줄 힘")]
    [field: SerializeField] public float MovingForce { get; private set; } = 30f; // 움직이는 힘
}
