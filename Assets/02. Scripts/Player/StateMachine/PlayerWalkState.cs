using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWalkState : PlayerGroundState
{
    public Action AttackAction;
    public PlayerWalkState(PlayerStateMachine playerStateMachine) : base(playerStateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        //TODO: Walk 애니매이터 활성화 필요
        Debug.Log("WalkState 진입");
    }


    public override void Exit()
    {
        base.Exit();
        //TODO: Walk 애니매이터 비활성화 필요
        Debug.Log("WalkState 진입");
    }

    public override void Update()
    {
        base.Update();

        if (playerStateMachine.Player.input.IsJump &&
            playerStateMachine.Player.playerCheckGround.CanJump &&
            Mathf.Approximately(playerStateMachine.Player.playerRigidbody.velocity.y, 0))
        {
            playerStateMachine.ChangeState(playerStateMachine.JumpState);
            return;
        }

        if (playerStateMachine.Player.input.MoveDir == Vector2.zero)
        {
            playerStateMachine.ChangeState(playerStateMachine.IdleState);
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
