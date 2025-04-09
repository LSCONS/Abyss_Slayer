using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWalkState : PlayerGroundState
{
    public StoppableAction MoveAction = new();
    public StoppableAction AttackAction = new();
    public PlayerWalkState(PlayerStateMachine playerStateMachine) : base(playerStateMachine)
    {
        //Idle 스테이트 진입 가능 여부 확인
        MoveAction.AddListener(playerStateMachine.ConnectIdleState);
        //Jump 스테이트 진입 가능 여부 확인
        MoveAction.AddListener(playerStateMachine.ConnectJumpState);
        //Fall 스테이트 진입 가능 여부 확인
        MoveAction.AddListener(playerStateMachine.ConnectFallState);
        //Dash 스테이트 진입 가능 여부 확인
        MoveAction.AddListener(playerStateMachine.ConnectDashState);
    }

    public override void Enter()
    {
        base.Enter();
        //TODO: Walk 애니매이터 활성화 필요
#if StateMachineDebug
        Debug.Log("WalkState 진입");
#endif
    }


    public override void Exit()
    {
        base.Exit();
        //TODO: Walk 애니매이터 비활성화 필요
#if StateMachineDebug
        Debug.Log("WalkState 진입");
#endif
    }

    public override void Update()
    {
        base.Update();
        MoveAction?.Invoke();
        AttackAction?.Invoke();
    }
}
