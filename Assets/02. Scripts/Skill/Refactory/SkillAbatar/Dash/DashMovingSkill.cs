using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MovingDashSkill", menuName = "SkillRefactory/Moving/DashSkill")]
public class DashMovingSkill : MovingSkill
{
    public override void UseSkill()
    {
        base.UseSkill();
        Vector2 DashVector = player.Input.MoveDir.normalized;
        DashVector *= MovingForce;
        player.SetFlipX(DashVector.x);
        player.PlayerRigidbody.AddForce(DashVector, ForceMode2D.Impulse);
        player.PlayerData.PlayerAirData.CurDashCount--;
    }
}
