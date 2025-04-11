using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcherSkillAnimationTrigger : MonoBehaviour
{
    public Dictionary<SkillSlotKey, SkillData> skills;
    public Player player;
    public Coroutine skillCoroutine;

    public void UseSkillA()
    {
        SkillData skillData = skills[SkillSlotKey.A];
        skillData.Execute(player, null);
    }

    public void UseSkillS()
    {
        SkillData skillData = skills[SkillSlotKey.S];

        // 임시 추가 (아직 UseSkillS 호출이 안되는 상태인듯 해서 아직 실행 자체는 ArcherSkill_s에서 담당중)
        skillData.canMove = false;
        skillCoroutine = StartCoroutine(((ArcherSkill_s)skillData.executer).FireArrows(player, null, skillData));
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


    public void StopSkillCoroutine()
    {
        if(skillCoroutine != null)
        {
            StopCoroutine(skillCoroutine);
            skillCoroutine = null;
        }
    }
}
