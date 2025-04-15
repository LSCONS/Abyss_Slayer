using UnityEngine;

public class PlayerJumpState : PlayerAirState
{
    public StoppableAction MoveAction = new();
    public PlayerJumpState(PlayerStateMachine playerStateMachine) : base(playerStateMachine)
    {
    }


    public void Init()
    {
        //Fall State 진입 가능 여부 확인
        MoveAction.AddListener(playerStateMachine.ConnectFallState);
        //Dash State 진입 가능 여부 확인
        MoveAction.AddListener(playerStateMachine.ConnectDashState);
        //Idle State 진입 가능 여부 확인
        MoveAction.AddListener(playerStateMachine.ConnectIdleState);
        //Walk State 진입 가능 여부 확인
        MoveAction.AddListener(playerStateMachine.ConnectWalkState);
    }

    public override void Enter()
    {
        base.Enter();
        StartAnimation(playerStateMachine.Player.playerAnimationData.jumpParameterHash);
        Jump();

#if StateMachineDebug
        Debug.Log("JumpState 진입");
#endif
    }

    public override void Exit()
    {
        base.Exit();
        StopAnimation(playerStateMachine.Player.playerAnimationData.jumpParameterHash);

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
