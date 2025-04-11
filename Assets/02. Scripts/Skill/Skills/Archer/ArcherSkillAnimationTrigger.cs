using System.Collections;
using System.Collections.Generic;
using System.Globalization;
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
        skillData.canUse = false;
    }

    public void UseSkillS()
    {
        SkillData skillData = skills[SkillSlotKey.S];
        skillData.canMove = false;
        skillCoroutine = StartCoroutine(((ArcherSkill_s)skillData.executer).FireArrows(player, null, skillData));
        skillData.canUse = false;
    }

    public void UseSkillD()
    {
        SkillData skillData = skills[SkillSlotKey.D];
        skillData.Execute(player, null);
        skillData.canUse = false;
    }

    public void UseSkillZ()
    {
        SkillData skillData = skills[SkillSlotKey.Z];
        skillData.Execute(player, null);
        //Vector2 DashVector = player.input.MoveDir.normalized;
        //DashVector *= player.playerData.PlayerAirData.DashForce;
        //ResetZeroVelocity();
        //ResetZeroGravityForce();
        //FlipRenderer(DashVector.x);
        //player.playerRigidbody.AddForce(DashVector, ForceMode2D.Impulse);
        //player.playerData.PlayerAirData.CanDash = false;
        //player.SkillCoolTimeUpdate(SkillSlotKey.Z);
        //player.playerData.PlayerAirData.CurDashCount--;
        skillData.canUse = false;
    }

    public void UseSkillX()
    {
        SkillData skillData = skills[SkillSlotKey.X];
        skillData.Execute(player, null);
        skillData.canUse = false;
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
