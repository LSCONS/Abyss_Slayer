using System;
using System.Collections.Generic;
using UnityEngine;

public class MagicalBladerSkillAnimationTrigger : MonoBehaviour, IStopCoroutine
{
    public Dictionary<SkillSlotKey, Skill> SkillDictionary { get; set; }
    public Player Player { get; set; }
    public Coroutine HoldSkillCoroutine { get; set; }

    [Serializable]
    public class CooldownReductionRule
    {
        public Skill sourceSkill; // 이 스킬이 적중할 때
        public Skill targetSkill; // 이 스킬의 쿨타임을
        public float reductionAmount;   // 이만큼 줄인다
    }

    public List<CooldownReductionRule> cooldownRules = new();

    private void Start()
    {
        cooldownRules.Add(new CooldownReductionRule
        {
            sourceSkill = SkillDictionary[SkillSlotKey.X],
            targetSkill = SkillDictionary[SkillSlotKey.D],
            reductionAmount = 5f
        });

        if (Player != null)
        {
            Player.OnSkillHit += OnSkillHit;    // 이벤트 등록
        }
    }

    private void OnDisable()
    {
        if (Player != null)
        {
            Player.OnSkillHit -= OnSkillHit;
        }
    }

    // 데미지 체크 되면 이 메서드로
    private void OnSkillHit(Skill skill)
    {
        NotifySkillHit(skill);
    }

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

    public void NotifySkillHit(Skill skill)
    {
        foreach (var rule in cooldownRules)
        {
            if(rule.sourceSkill == skill)
                rule.targetSkill.ReduceCooldown(rule.reductionAmount);
        }
    }

}


