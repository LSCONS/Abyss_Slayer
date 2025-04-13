using UnityEngine;

public class PlayerWalkState : PlayerGroundState
{
    public StoppableAction MoveAction = new();
    public PlayerWalkState(PlayerStateMachine playerStateMachine) : base(playerStateMachine)
    {
    }

    public void Init()
    {
        //Idle 스테이트 진입 가능 여부 확인
        MoveAction.AddListener(playerStateMachine.ConnectIdleState);
        //Jump 스테이트 진입 가능 여부 확인
        MoveAction.AddListener(playerStateMachine.ConnectJumpState);
        //Fall 스테이트 진입 가능 여부 확인
        MoveAction.AddListener(playerStateMachine.ConnectFallState);
        //Dash 스테이트 진입 가능 여부 확인
        MoveAction.AddListener(playerStateMachine.ConnectDashState);
    }

    public override void Enter()
    {
        base.Enter();
        StartAnimation(playerStateMachine.Player.playerAnimationData.walkParameterHash);

#if StateMachineDebug
        Debug.Log("WalkState 진입");
#endif
    }


    public override void Exit()
    {
        base.Exit();
        StopAnimation(playerStateMachine.Player.playerAnimationData.walkParameterHash);

#if StateMachineDebug
        Debug.Log("WalkState 해제");
#endif
    }

    public override void Update()
    {
        base.Update();
        MoveAction?.Invoke();
    }
}
