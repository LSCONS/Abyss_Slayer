using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpState : PlayerAirState
{
    public StoppableAction MoveAction = new();
    public PlayerJumpState(PlayerStateMachine playerStateMachine) : base(playerStateMachine)
    {
        //Fall State 진입 가능 여부 확인
        MoveAction.AddListener(playerStateMachine.ConnectFallState);
        //Dash State 진입 가능 여부 확인
        MoveAction.AddListener(playerStateMachine.ConnectDashState);
    }

    public override void Enter()
    {
        base.Enter();
        //TODO: 점프 애니메이션 실행
        Jump();
#if StateMachineDebug
        Debug.Log("JumpState 진입");
#endif
    }

    public override void Exit()
    {
        base.Exit();
        //TODO: 점프 애니메이션 종료
#if StateMachineDebug
        Debug.Log("JumpState 해제");
#endif
    }

    public override void Update()
    {
        base.Update();
        MoveAction?.Invoke();
    }


    /// <summary>
    /// 플레이어가 점프를 실행할 때 실행할 메서드
    /// </summary>
    protected void Jump()
    {
        Vector2 jumpVector = playerStateMachine.Player.playerData.PlayerAirData.JumpForce * Vector2.up;
        playerStateMachine.Player.playerRigidbody.AddForce(jumpVector, ForceMode2D.Impulse);
    }
}
