using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewDoubleShotSkill", menuName = "Skill/Buff/DoubleShot")]
public class DoubleShotSkill : BuffSkill
{
    // 스킬 강화 메서드
    public override void SkillUpgrade()
    {
        base.SkillUpgrade();
        // 레벨에 따라 모든 투사체 스킬의 추가 투사체 데미지 배율 증가 
        if (projectileAttackSkills != null)
        {
            foreach (var skill in projectileAttackSkills)
            {
                skill.DamageMultiple = skill.BaseDamageMultiple + (Level.Value - 1) * Magnification;
            }
        }
    }
    public override void UseSkill()
    {
        base.UseSkill();
        player.Rpc_SetBuff((int)slotKey);
    }
}
