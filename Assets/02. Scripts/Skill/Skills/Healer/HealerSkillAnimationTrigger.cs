using System.Collections.Generic;
using UnityEngine;

public class HealerSkillAnimationTrigger : MonoBehaviour, IStopCoroutineS
{
    public Dictionary<SkillSlotKey, Skill> skills { get; set; }
    public Player player { get; set; }
    public Coroutine HoldSkillCoroutine {  get; set; }

    public void UseSkillA()
    {
        Skill skill = skills[SkillSlotKey.A];
        skill.UseSkill();
    }

    public void UseSkillS()
    {
        Skill skill = skills[SkillSlotKey.S];
        skill.UseSkill();
    }

    public void UseSkillD()
    {
        Skill skill = skills[SkillSlotKey.D];
        skill.UseSkill();
    }

    public void UseSkillZ()
    {
        Skill skill = skills[SkillSlotKey.Z];
        skill.UseSkill();
    }

    public void UseSkillX()
    {
        Skill skill = skills[SkillSlotKey.X];
        skill.UseSkill();
    }

    public void StopCoroutine()
    {
        if (HoldSkillCoroutine != null)
        {
            StopCoroutine(HoldSkillCoroutine);
            HoldSkillCoroutine = null;
        }
    }
}
