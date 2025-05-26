using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New AnimatorController", menuName = "Data/AnimatorController")]
public class AnimatorControllerDataGather : ScriptableObject
{
    [field: SerializeField] public List<AnimatorControllerData> ListAnimatorController;
}


[Serializable]
public class AnimatorControllerData
{
    [field: SerializeField] public EAnimatorController EAnimatorContoller { get; private set; } = EAnimatorController.None;
    [field: SerializeField] public RuntimeAnimatorController AnimatorController { get; private set; } = null;
}


public enum EAnimatorController
{
    None = 0,

    RogueSkill_X = 1,
    RogueSkill_A,
    RogueSkill_S,
    RogueSkill_D,

    MageSkill_X = 101,
    MageSkill_A,
    MageSkill_S,
    MageSkill_D,

    MagicalBlasterSkill_X = 201,
    MagicalBlasterSkill_A,
    MagicalBlasterSkill_S,
    MagicalBlasterSkill_D,

    HealerSkill_X = 301,
    HealerSkill_A,
    HealerSkill_S,
    HealerSkill_D,

    TankerSkill_X = 401,
    TankerSkill_A,
    TankerSkill_S,
    TankerSkill_D,
}
