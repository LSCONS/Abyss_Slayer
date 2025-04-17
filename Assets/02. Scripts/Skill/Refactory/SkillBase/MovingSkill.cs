using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingSkill : Skill
{
    [field: SerializeField] public float MovingForce { get; private set; } = 30f; //움직이는 힘
}
