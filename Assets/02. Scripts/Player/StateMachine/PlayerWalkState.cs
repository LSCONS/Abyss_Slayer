using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWalkState : PlayerGroundState
{
    public StoppableAction AttackAction = new();
    public PlayerWalkState(PlayerStateMachine playerStateMachine) : base(playerStateMachine)
    {
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

        //Idle 스테이트 진입 가능 여부 확인
        if (playerStateMachine.Player.input.MoveDir == Vector2.zero)
        {
            playerStateMachine.ChangeState(playerStateMachine.IdleState);
            return;
        }

        //Jump 스테이트 진입 가능 여부 확인
        if (playerStateMachine.Player.input.IsJump &&
            playerStateMachine.Player.playerCheckGround.CanJump &&
            Mathf.Approximately(playerStateMachine.Player.playerRigidbody.velocity.y, 0))
        {
            if (playerStateMachine.Player.input.MoveDir.y < 0 &&
                playerStateMachine.Player.playerCheckGround.GroundPlaneCount == 0)
            {
                //TODO: 다운점프 로직 시작
                playerStateMachine.Player.playerGroundCollider.isTrigger = true;
                playerStateMachine.ChangeState(playerStateMachine.FallState);
                return;
            }
            else if (playerStateMachine.Player.input.MoveDir.y >= 0)
            {
                playerStateMachine.ChangeState(playerStateMachine.JumpState);
                return;
            }
        }

        //Fall 스테이트 진입 가능 여부 확인
        if (!(playerStateMachine.Player.playerCheckGround.CanJump))
        {
            playerStateMachine.ChangeState(playerStateMachine.FallState);
            return;
        }

        //Dash 스테이트 진입 가능 여부 확인
        if (playerStateMachine.Player.playerData.PlayerAirData.CanDash &&
            playerStateMachine.Player.input.IsDash &&
            (playerStateMachine.Player.input.MoveDir.x != 0 ||
            playerStateMachine.Player.input.MoveDir.y > 0) &&
            playerStateMachine.Player.playerData.PlayerAirData.CurDashCount > 0 &&
            playerStateMachine.Player.input.MoveDir != Vector2.zero)
        {
            playerStateMachine.ChangeState(playerStateMachine.DashState);
            return;
        }

        AttackAction?.Invoke();
    }
}
