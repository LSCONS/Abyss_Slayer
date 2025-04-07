using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDashState : PlayerAttackState
{
    public PlayerDashState(PlayerStateMachine playerStateMachine) : base(playerStateMachine)
    {
    }


    public override void Enter()
    {
        base.Enter();
        //TODO: Dash애니메이션 파라미터 활성화
    }


    public override void Exit()
    {
        base.Exit();
        //TODO: Dash애니메이션 파라미터 비활성화
    }


    public override void Update()
    {
        base.Update();
    }
}
