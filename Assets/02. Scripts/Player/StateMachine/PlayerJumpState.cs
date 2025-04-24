using UnityEngine;

public class PlayerJumpState : PlayerAirState
{
    public StoppableAction MoveAction = new();
    private int animationNum = 0;
    private float animationTime = 0;
    private int animationDelay = 10;
    public PlayerJumpState(PlayerStateMachine playerStateMachine) : base(playerStateMachine)
    {
    }


    public void Init()
    {
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
        playerStateMachine.Player.PlayerSpriteChange.SetOnceAnimation(AnimationState.Jump, 0);
        animationNum = 0;
        animationTime = animationDelay;
        Jump();

#if StateMachineDebug
        Debug.Log("JumpState 진입");
#endif
    }

    public override void Exit()
    {
        base.Exit();

#if StateMachineDebug
        Debug.Log("JumpState 해제");
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
            if (playerStateMachine.Player.PlayerSpriteChange.SetOnceAnimation(AnimationState.Jump, ++animationNum)) return;
        }
        if (playerStateMachine.ConnectRigidbodyFallState()) return;
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
