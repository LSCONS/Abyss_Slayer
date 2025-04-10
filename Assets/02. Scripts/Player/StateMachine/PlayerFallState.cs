using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFallState : PlayerAirState
{
    public StoppableAction MoveAction = new();
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
        StartAnimation(playerStateMachine.Player.playerAnimationData.fallParameterHash);

#if StateMachineDebug
        Debug.Log("FallState 진입");
#endif
    }

    public override void Exit()
    {
        base.Exit();
        StopAnimation(playerStateMachine.Player.playerAnimationData.fallParameterHash);

#if StateMachineDebug
        Debug.Log("FallState 해제");
#endif
    }

    public override void Update()
    {
        base.Update();
        MoveAction?.Invoke();
    }
}
