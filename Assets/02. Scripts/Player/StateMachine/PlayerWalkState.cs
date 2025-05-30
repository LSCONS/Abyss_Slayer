using UnityEngine;

public class PlayerWalkState : PlayerGroundState
{
    public StoppableAction MoveAction = new();
    private int animationNum = 0;
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
        playerStateMachine.Player.PlayerSpriteChange.SetLoopAnimation(AnimationState.Run1, 0);
        animationNum = 0;

#if StateMachineDebug
        Debug.Log("WalkState 진입");
#endif
    }


    public override void Exit()
    {
        base.Exit();

#if StateMachineDebug
        Debug.Log("WalkState 해제");
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
            playerStateMachine.Player.PlayerSpriteChange.SetLoopAnimation(AnimationState.Run1, ++animationNum);
        }

        if (playerStateMachine.Player.Runner.IsServer)
            MoveAction?.Invoke();
    }
}
