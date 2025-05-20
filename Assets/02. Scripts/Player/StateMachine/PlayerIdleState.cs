using UnityEngine;
using static Unity.Collections.Unicode;

public class PlayerIdleState : PlayerGroundState
{
    public StoppableAction MoveAction = new();
    private int animationNum = 0;
    public PlayerIdleState(PlayerStateMachine playerStateMachine) : base(playerStateMachine)
    {
    }

    public void Init()
    {
        //Walk 스테이트 진입 가능 여부 확인
        MoveAction.AddListener(playerStateMachine.ConnectWalkState);
        //Jump 혹은 DownJump스테이트 진입 가능 여부 확인
        MoveAction.AddListener(playerStateMachine.ConnectJumpState);
        //Fall 스테이트 진입 가능 여부 확인
        MoveAction.AddListener(playerStateMachine.ConnectFallState);
        //Dash 스테이트 진입 가능 여부 확인
        MoveAction.AddListener(playerStateMachine.ConnectDashState);
    }

    public override void Enter()
    {
        base.Enter();
        playerStateMachine.Player.PlayerSpriteChange.SetLoopAnimation(AnimationState.Idle1, 0);
        playerStateMachine.MovementSpeed = 0f;
        ResetZeroVelocity();
        animationNum = 0;

#if StateMachineDebug
        Debug.Log("IdleState 진입");
#endif
    }

    public override void Exit()
    {
        base.Exit();
        playerStateMachine.MovementSpeed = playerStateMachine.Player.PlayerData.PlayerGroundData.BaseSpeed;

#if StateMachineDebug
        Debug.Log("IdleState 해제");
#endif
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    public override void Update()
    {
        if (ChangeSpriteTime + CurTime < Time.time)
        {
            CurTime = Time.time;
            playerStateMachine.Player.PlayerSpriteChange.SetLoopAnimation(AnimationState.Idle1, ++animationNum);
        }

        if (playerStateMachine.Player.Runner.IsServer)
            MoveAction?.Invoke();
    }
}
