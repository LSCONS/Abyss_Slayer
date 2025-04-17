using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class ArcherSkillAnimationTrigger : MonoBehaviour, IStopCoroutineS
{
    public Dictionary<SkillSlotKey, Skill> skills {  get; set; }
    public Player player {  get; set; }
    public Coroutine skillCoroutine;

    public void UseSkillA()
    {
        Skill skill = skills[SkillSlotKey.A];
        skill.UseSkill();
    }

    public void UseSkillS()
    {
        Skill skill = skills[SkillSlotKey.S];
        skill.CanMove = false;
        //skillCoroutine = StartCoroutine(((ArcherSkill_s)skillData.executer).FireArrows(player, null, skillData));
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

        Vector2 DashVector = player.input.MoveDir.normalized;
        DashVector *= player.playerData.PlayerAirData.DashForce;
        if (DashVector.x > 0) player.SpriteRenderer.flipX = false;
        else if (DashVector.x < 0) player.SpriteRenderer.flipX = true;
        player.playerRigidbody.AddForce(DashVector, ForceMode2D.Impulse);
        player.playerData.PlayerAirData.CurDashCount--;
    }

    public void UseSkillX()
    {
        Skill skill = skills[SkillSlotKey.X];
        skill.UseSkill();
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
