using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class ArcherSkillAnimationTrigger : MonoBehaviour, IStopCoroutine
{
    public Dictionary<SkillSlotKey, SkillData> skills {  get; set; }
    public Player player {  get; set; }
    public Coroutine skillCoroutine;

    public ArcherSkillAnimationTrigger(Dictionary<SkillSlotKey, SkillData> skills, Player player)
    {
        this.skills = skills;
        this.player = player;
    }

    public void UseSkillA()
    {
        SkillData skillData = skills[SkillSlotKey.A];
        skillData.Execute(player, null);
    }

    public void UseSkillS()
    {
        SkillData skillData = skills[SkillSlotKey.S];
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

        Vector2 DashVector = player.input.MoveDir.normalized;
        DashVector *= player.playerData.PlayerAirData.DashForce;
        if (DashVector.x > 0) player.SpriteRenderer.flipX = false;
        else if (DashVector.x < 0) player.SpriteRenderer.flipX = true;
        player.playerRigidbody.AddForce(DashVector, ForceMode2D.Impulse);
        player.playerData.PlayerAirData.CurDashCount--;
    }

    public void UseSkillX()
    {
        SkillData skillData = skills[SkillSlotKey.X];
        skillData.Execute(player, null);
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
