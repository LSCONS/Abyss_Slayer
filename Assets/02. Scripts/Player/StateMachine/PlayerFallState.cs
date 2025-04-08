using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFallState : PlayerAirState
{
    public StoppableAction AttackAction = new();
    public PlayerFallState(PlayerStateMachine playerStateMachine) : base(playerStateMachine)
    {
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
        //Idle 스테이트 진입 가능 여부 확인
        if (playerStateMachine.Player.playerCheckGround.CanJump &&
            !(playerStateMachine.Player.playerGroundCollider.isTrigger)&&
            Mathf.Approximately(playerStateMachine.Player.playerRigidbody.velocity.y, 0))
        {
            playerStateMachine.ChangeState(playerStateMachine.IdleState);
            return;
        }

        //Dash 스테이트 진입 가능 여부 확인
        if (playerStateMachine.Player.playerData.PlayerAirData.CanDash &&
            playerStateMachine.Player.input.IsDash &&
            playerStateMachine.Player.playerData.PlayerAirData.CurDashCount > 0 &&
            playerStateMachine.Player.input.MoveDir != Vector2.zero)
        {
            playerStateMachine.ChangeState(playerStateMachine.DashState);
            return;
        }

        AttackAction?.Invoke();
    }
}
