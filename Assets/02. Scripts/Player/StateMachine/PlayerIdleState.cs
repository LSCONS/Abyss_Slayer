using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdleState : PlayerGroundState
{
    public Action AttackAction;
    public PlayerIdleState(PlayerStateMachine playerStateMachine) : base(playerStateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        playerStateMachine.MovementSpeed = 0f;
        Debug.Log("IdleState 진입");
    }

    public override void Exit()
    {
        base.Exit();
        playerStateMachine.MovementSpeed = playerStateMachine.Player.playerData.PlayerGroundData.BaseSpeed;
        Debug.Log("IdleState 해제");
    }

    public override void Update()
    {
        base.Update();

        //Walk 스테이트 진입 가능 여부 확인
        if(playerStateMachine.Player.input.MoveDir.x != 0f)
        {
            playerStateMachine.ChangeState(playerStateMachine.WalkState);
            return;
        }

        //Jump 스테이트 진입 가능 여부 확인
        if (playerStateMachine.Player.input.IsJump &&
            playerStateMachine.Player.playerCheckGround.CanJump &&
            Mathf.Approximately(playerStateMachine.Player.playerRigidbody.velocity.y, 0))
        {
            playerStateMachine.ChangeState(playerStateMachine.JumpState);
            return;
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
            playerStateMachine.Player.playerData.PlayerAirData.CurDashCount > 0 &&
            playerStateMachine.Player.input.MoveDir != Vector2.zero)
        {
            playerStateMachine.ChangeState(playerStateMachine.DashState);
            return;
        }

        AttackAction?.Invoke();
    }
}
