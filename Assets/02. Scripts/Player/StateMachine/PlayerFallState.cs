using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFallState : PlayerAirState
{
    public StoppableAction MoveAction = new();
    public StoppableAction AttackAction = new();
    public PlayerFallState(PlayerStateMachine playerStateMachine) : base(playerStateMachine)
    {
        //Idle State 진입 가능 여부 확인
        MoveAction.AddListener(playerStateMachine.ConnectIdleState);
        //Walk State 진입 가능 여부 확인
        MoveAction.AddListener(playerStateMachine.ConnectWalkState);
        //Dash State 진입 가능 여부 확인
        MoveAction.AddListener(playerStateMachine.ConnectDashState);
    }

    public override void Enter()
    {
        base.Enter();
        //TODO: Fall애니매이션으로 교체
#if StateMachineDebug
        Debug.Log("FallState 진입");
#endif
    }

    public override void Exit()
    {
        base.Exit();
        //TODO: Fall애니메이션 해제
#if StateMachineDebug
        Debug.Log("FallState 해제");
#endif
    }

    public override void Update()
    {
        base.Update();
        MoveAction?.Invoke();
        AttackAction?.Invoke();
    }
}
