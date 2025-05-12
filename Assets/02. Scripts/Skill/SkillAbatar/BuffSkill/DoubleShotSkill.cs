using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewDoubleShotSkill", menuName = "Skill/Buff/DoubleShot")]
public class DoubleShotSkill : BuffSkill
{
    [SerializeField] private ProjectileAttackSkill projectileAttackSkill; // 임시 참조

    public override void UseSkill()
    {
        base.UseSkill();
        player.SetBuff(this);
    }

    public override void SkillUpgrade()
    {
        base.SkillUpgrade();
        Level.Value++;
        projectileAttackSkill.DamageMultiple = projectileAttackSkill.BaseDamageMultiple + (Level.Value - 1) * 0.1f;
        Debug.LogAssertion($"배율 업그레이드: {projectileAttackSkill.DamageMultiple}");
    }
}
