using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MovingDashSkill", menuName = "SkillRefactory/Moving/DashSkill")]
public class DashMovingSkill : MovingSkill
{
    public override void UseSkill()
    {
        base.UseSkill();
        Vector2 DashVector = player.input.MoveDir.normalized;
        DashVector *= MovingForce;
        if (DashVector.x > 0) player.SpriteRenderer.flipX = false;
        else if (DashVector.x < 0) player.SpriteRenderer.flipX = true;
        player.playerRigidbody.AddForce(DashVector, ForceMode2D.Impulse);
        player.playerData.PlayerAirData.CurDashCount--;
    }
}
