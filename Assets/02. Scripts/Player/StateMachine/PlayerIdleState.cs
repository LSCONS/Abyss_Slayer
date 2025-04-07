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
        if(playerStateMachine.Player.input.MoveDir.x != 0f)
        {
            playerStateMachine.ChangeState(playerStateMachine.WalkState);
            return;
        }

        if (playerStateMachine.Player.input.IsJump && 
            playerStateMachine.Player.playerCheckGround.CanJump &&
            Mathf.Approximately(playerStateMachine.Player.playerRigidbody.velocity.y, 0))
        {
            playerStateMachine.ChangeState(playerStateMachine.JumpState);
            return;
        }

        if (!(playerStateMachine.Player.playerCheckGround.CanJump))
        {
            playerStateMachine.ChangeState(playerStateMachine.FallState);
            return;
        }

        AttackAction?.Invoke();
    }
}
