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
        //TODO: 애니메이션 파라미터 @Ground True 변경
    }

    public override void Exit()
    {
        base.Exit();
        //TODO: 애니매이션 파라미터 @Ground True 변경
    }
}
