using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpState : PlayerAirState
{
    public PlayerJumpState(PlayerStateMachine playerStateMachine) : base(playerStateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        //TODO: 점프 애니메이션 실행
        Jump();
        Debug.Log("JumpState 진입");
    }

    public override void Exit()
    {
        base.Exit();
        //TODO: 점프 애니메이션 종료
        Debug.Log("JumpState 해제");
    }

    public override void Update()
    {
        base.Update();
        if(playerStateMachine.Player.playerRigidbody.velocity.y <= 0)
        {
            playerStateMachine.ChangeState(playerStateMachine.FallState);
        }
    }
}
