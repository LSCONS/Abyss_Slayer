using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAbnomalState : PlayerBaseState
{
    public PlayerAbnomalState(PlayerStateMachine playerStateMachine) : base(playerStateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        StartAnimation(playerStateMachine.Player.playerAnimationData.AbnomalParameterHash);
    }

    public override void Exit()
    {
        base.Exit();
        StopAnimation(playerStateMachine.Player.playerAnimationData.AbnomalParameterHash);
    }

    public override void Update()
    {
        base.Update();
    }
}
