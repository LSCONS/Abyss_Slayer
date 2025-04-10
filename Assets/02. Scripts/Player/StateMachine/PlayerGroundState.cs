using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundState : PlayerBaseState
{
    public PlayerGroundState(PlayerStateMachine playerStateMachine) : base(playerStateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        StartAnimation(playerStateMachine.Player.playerAnimationData.groundParameterHash);
        playerStateMachine.Player.playerData.PlayerAirData.ResetDashCount();
        playerStateMachine.Player.playerData.PlayerAirData.ResetJumpCount();
    }

    public override void Exit()
    {
        base.Exit();
        StopAnimation(playerStateMachine.Player.playerAnimationData.groundParameterHash);
    }

    public override void Update()
    {
        base.Update();
    }
}
