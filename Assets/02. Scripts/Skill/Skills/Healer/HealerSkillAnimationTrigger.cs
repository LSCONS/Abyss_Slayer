using System.Collections.Generic;
using UnityEngine;

public class HealerSkillAnimationTrigger : MonoBehaviour, IStopCoroutine
{
    public Dictionary<SkillSlotKey, SkillData> skills { get; set; }
    public Player player { get; set; }
    public Coroutine skillCoroutine;

    public void UseSkillA()
    {
        SkillData skillData = skills[SkillSlotKey.A];
        skillData.Execute(player, null);
    }

    public void UseSkillS()
    {
        SkillData skillData = skills[SkillSlotKey.S];
        skillData.Execute(player, null);
    }

    public void UseSkillD()
    {
        SkillData skillData = skills[SkillSlotKey.D];
        skillData.Execute(player, null);
    }

    public void UseSkillZ()
    {
        SkillData skillData = skills[SkillSlotKey.Z];
        skillData.Execute(player, null);
    }

    public void UseSkillX()
    {
        SkillData skillData = skills[SkillSlotKey.X];
        skillData.Execute(player, null);
    }

    public void StopCoroutine()
    {
        if (skillCoroutine != null)
        {
            StopCoroutine(skillCoroutine);
            skillCoroutine = null;
        }
    }
}
