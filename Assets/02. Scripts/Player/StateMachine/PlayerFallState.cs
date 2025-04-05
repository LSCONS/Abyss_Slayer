using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFallState : PlayerAirState
{
    public PlayerFallState(PlayerStateMachine playerStateMachine) : base(playerStateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        //TODO: Fall애니매이션으로 교체
        Debug.Log("FallState 진입");
    }

    public override void Exit()
    {
        base.Exit();
        //TODO: Fall애니메이션 해제
        Debug.Log("FallState 해제");
    }

    public override void Update()
    {
        base.Update();
        if (playerStateMachine.Player.playerCheckGround.CanJump &&
            Mathf.Approximately(playerStateMachine.Player.playerRigidbody.velocity.y, 0))
        {
            playerStateMachine.ChangeState(playerStateMachine.IdleState);
        }
    }
}
