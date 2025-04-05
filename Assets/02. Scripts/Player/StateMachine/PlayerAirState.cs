using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAirState : PlayerBaseState
{
    public PlayerAirState(PlayerStateMachine playerStateMachine) : base(playerStateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        //TODO: Air파라미터 활성화 필요
        Debug.Log("AirState 진입");
    }

    public override void Exit()
    {
        base.Exit();
        //TODO: Air파라미터 비활성화 필요
        Debug.Log("AirState 해제");
    }
}
