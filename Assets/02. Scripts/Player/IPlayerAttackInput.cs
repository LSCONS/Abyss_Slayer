using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayerAttackInput
{
    public bool GetIsInputKey();

    public SkillData GetSkillData();
}
