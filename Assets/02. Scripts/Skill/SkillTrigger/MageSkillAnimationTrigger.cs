using System.Collections.Generic;
using UnityEngine;

public class MageSkillAnimationTrigger : MonoBehaviour, IStopCoroutine
{
    public Dictionary<SkillSlotKey, Skill> SkillDictionary { get; set; }
    public Player Player { get; set; }
    public Coroutine HoldSkillCoroutine { get; set; }

    public void UseSkillA()
    {
        Skill skill = SkillDictionary[SkillSlotKey.A];
        skill.UseSkill();
    }

    public void UseSkillS()
    {
        Skill skill = SkillDictionary[SkillSlotKey.S];
        skill.UseSkill();
    }

    public void UseSkillD()
    {
        Skill skill = SkillDictionary[SkillSlotKey.D];
        skill.UseSkill();
    }

    public void UseSkillZ()
    {
        Skill skill = SkillDictionary[SkillSlotKey.Z];
        skill.UseSkill();
    }

    public void UseSkillX()
    {
        Skill skill = SkillDictionary[SkillSlotKey.X];
        skill.UseSkill();
    }

    public void StopCoroutine()
    {
        if (HoldSkillCoroutine != null)
        {
            CoroutineManager.Instance.StopCoroutineExit(HoldSkillCoroutine);
            HoldSkillCoroutine = null;
        }
    }
}
