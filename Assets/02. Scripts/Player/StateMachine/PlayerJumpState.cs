using UnityEngine;

public class PlayerJumpState : PlayerAirState
{
    public StoppableAction MoveAction = new();
    private int animationNum = 0;
    private float animationTime = 0;
    private int animationDelay = 10;

    private bool hasJumpedInThisFrame = false;
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
        hasJumpedInThisFrame = true;

        // 사운드 재생
        SoundManager.Instance.PlaySFX(ESFXType.Jump);


#if StateMachineDebug
        Debug.Log("JumpState 진입");
#endif
    }

    public override void Exit()
    {
        base.Exit();
        // 사운드 멈춤
        SoundManager.Instance.StopSFX(ESFXType.Jump);

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

        CheckDownJump();                                    // 밑점프 체크
        CheckDoubleJump(ref hasJumpedInThisFrame);          // 2단 점프 체크
        UpdateJumpAnimation();                              // 점프 애니메이션 업데이트
        CheckFallTransition();                              // fallState 상태로 전환 확인

        MoveAction?.Invoke();
    }

    /// <summary>
    /// 점프 애니메이션 변경
    /// </summary>
    private void UpdateJumpAnimation()
    {
        if (animationTime > 0) return;

        animationTime = animationDelay;
        playerStateMachine.Player.PlayerSpriteChange.SetOnceAnimation(AnimationState.Jump, ++animationNum);
    }

    /// <summary>
    /// 떨어지는 상태면 fallState로 전환
    /// </summary>
    private void CheckFallTransition()
    {
        playerStateMachine.ConnectRigidbodyFallState();
    }

    /// <summary>
    /// 플레이어가 점프를 실행할 때 실행할 메서드
    /// </summary>
    protected void Jump()
    {
        Vector2 jumpVector = playerStateMachine.Player.PlayerData.PlayerAirData.JumpForce * Vector2.up;

        // 기존 y속도 제거하고 새 점프력 적용함
        var velocity = playerStateMachine.Player.playerRigidbody.velocity;
        playerStateMachine.Player.playerRigidbody.velocity = new Vector2(velocity.x, 0);
        playerStateMachine.Player.playerRigidbody.AddForce(jumpVector, ForceMode2D.Impulse);

        playerStateMachine.Player.PlayerData.PlayerAirData.UseJump();   // 점프 카운트 --
    }
}
