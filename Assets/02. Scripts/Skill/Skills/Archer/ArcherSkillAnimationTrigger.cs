using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcherSkillAnimationTrigger : MonoBehaviour
{
    public Dictionary<SkillSlotKey, SkillData> skills;
    public Player player;
    public Coroutine curCoroutine;

    public void UseSkillA()
    {
        SkillData skillData = skills[SkillSlotKey.A];
        skillData.Execute(player, null);
    }

    public void UseSkillS()
    {
        SkillData skillData = skills[SkillSlotKey.S];
        skillData.canMove = false;
        curCoroutine = StartCoroutine(((ArcherSkill_s)skillData.executer).FireArrows(player, null, skillData));
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




}
