using UnityEngine;

public class PlayerWalkState : PlayerGroundState
{
    public StoppableAction MoveAction = new();
    private int animationNum = 0;
    private float animationTime = 0;
    private int animationDelay = 3;
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
        animationTime = animationDelay;

        // 걷기 사운드 재생
        SoundManager.Instance.PlaySFX(ESFXType.Walk, true, 1.3f);

#if StateMachineDebug
        Debug.Log("WalkState 진입");
#endif
    }


    public override void Exit()
    {
        // 걷기 사운드 끝
        SoundManager.Instance.StopSFX(ESFXType.Walk);

        base.Exit();

#if StateMachineDebug
        Debug.Log("WalkState 해제");
#endif
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        animationTime--;
    }

    public override void Update()
    {
        base.Update();
        if (animationTime <= 0)
        {
            animationTime = animationDelay;
            playerStateMachine.Player.PlayerSpriteChange.SetLoopAnimation(AnimationState.Run1, ++animationNum);
        }
        MoveAction?.Invoke();
    }
}
