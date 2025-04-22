using System;
using System.Collections.Generic;
using UnityEngine;

public class MagicalBladerSkillAnimationTrigger : MonoBehaviour, IStopCoroutine
{
    public Dictionary<SkillSlotKey, Skill> SkillDictionary { get; set; }
    public Player Player { get; set; }
    public Coroutine HoldSkillCoroutine { get; set; }


    public List<CooldownReductionRule> cooldownRules = new();
    public SkillSlotKey? currentSkillSlotKey { get; set; }

    private void Start()
    {
        cooldownRules.Add(new CooldownReductionRule
        {
            sourceSlot = SkillSlotKey.X,
            targetSlot = SkillSlotKey.D,
            reductionAmount = 2f
        });
    }

    public void UseSkillA()
    {
        currentSkillSlotKey = SkillSlotKey.A;
        Skill skill = SkillDictionary[SkillSlotKey.A];
        skill.UseSkill();
    }

    public void UseSkillS()
    {
        currentSkillSlotKey = SkillSlotKey.S;
        Skill skill = SkillDictionary[SkillSlotKey.S];
        skill.UseSkill();
    }

    public void UseSkillD()
    {
        currentSkillSlotKey = SkillSlotKey.D;
        Skill skill = SkillDictionary[SkillSlotKey.D];
        skill.UseSkill();
    }

    public void UseSkillZ()
    {
        currentSkillSlotKey = SkillSlotKey.Z;
        Skill skill = SkillDictionary[SkillSlotKey.Z];
        skill.UseSkill();
    }

    public void UseSkillX()
    {
        currentSkillSlotKey = SkillSlotKey.X;
        Skill skill = SkillDictionary[SkillSlotKey.X];
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

    public void NotifySkillHit(SkillSlotKey slot)
    {
        foreach (var rule in cooldownRules)
        {
            if (rule.sourceSlot == slot &&
                Player.equippedSkills.TryGetValue(rule.targetSlot, out var target))
            {
                target.ReduceCooldown(rule.reductionAmount);
            }
        }
    }

}

[Serializable]
public class CooldownReductionRule
{
    public SkillSlotKey sourceSlot; // 이 스킬이 적중할 때
    public SkillSlotKey targetSlot; // 이 스킬의 쿨타임을
    public float reductionAmount;   // 이만큼 줄인다
}
