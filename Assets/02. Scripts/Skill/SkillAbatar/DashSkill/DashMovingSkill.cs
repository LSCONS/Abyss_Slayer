using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 플레이어가 입력한 방향으로 Dash로 이동하는 기능
/// </summary>
[CreateAssetMenu(fileName = "NewDashMovingSkill", menuName = "Skill/Moving/Dash")]
public class DashMovingSkill : MovingSkill
{
    public override void Init()
    {
        base.Init();
    }
    public override void UseSkill()
    {
        base.UseSkill();
        Vector2 DashVector = player.NetworkInput.MoveDir.normalized;
        player.FlipRenderer(DashVector.x);
        if (DashVector == Vector2.zero) DashVector = new Vector2(PlayerFrontXNormalized(), 0).normalized;
        DashVector *= MovingForce;
        player.PlayerRigidbody.AddForce(DashVector, ForceMode2D.Impulse);
        player.PlayerData.PlayerAirData.CurDashCount--;
    }
}
