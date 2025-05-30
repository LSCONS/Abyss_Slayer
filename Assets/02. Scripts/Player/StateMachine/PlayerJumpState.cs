using UnityEngine;

public class PlayerJumpState : PlayerAirState
{
    public StoppableAction MoveAction = new();
    private int animationNum = 0;

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
        //Fall State 진입 가능 여부 확인
        MoveAction.AddListener(playerStateMachine.ConnectRigidbodyFallState);
    }

    public override void Enter()
    {
        base.Enter();
        playerStateMachine.Player.PlayerSpriteChange.SetOnceAnimation(AnimationState.Jump, 0);
        animationNum = 0;
        Jump();
        hasJumpedInThisFrame = true;

        // 사운드 재생
        SoundManager.Instance.PlaySFX(EAudioClip.SFX_PlayerJump);


#if StateMachineDebug
        Debug.Log("JumpState 진입");
#endif
    }

    public override void Exit()
    {
        base.Exit();
        // 사운드 멈춤
        SoundManager.Instance.StopSFX(EAudioClip.SFX_PlayerJump);

#if StateMachineDebug
        Debug.Log("JumpState 해제");
#endif
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }


    public override void Update()
    {
        base.Update();

        CheckDownJump();                                    // 밑점프 체크
        CheckDoubleJump(ref hasJumpedInThisFrame);          // 2단 점프 체크
        UpdateJumpAnimation();                              // 점프 애니메이션 업데이트

        MoveAction?.Invoke();
    }

    /// <summary>
    /// 점프 애니메이션 변경
    /// </summary>
    private void UpdateJumpAnimation()
    {
        if (ChangeSpriteTime + CurTime < Time.time)
        {
            CurTime = ChangeSpriteTime + CurTime;
            playerStateMachine.Player.PlayerSpriteChange.SetOnceAnimation(AnimationState.Jump, ++animationNum);
        }
    }

    /// <summary>
    /// 플레이어가 점프를 실행할 때 실행할 메서드
    /// </summary>
    protected void Jump()
    {
        Vector2 jumpVector = playerStateMachine.Player.PlayerData.PlayerAirData.JumpForce * Vector2.up;

        // 기존 y속도 제거하고 새 점프력 적용함
        var velocity = playerStateMachine.Player.PlayerRigidbody.velocity;
        playerStateMachine.Player.PlayerRigidbody.velocity = new Vector2(velocity.x, 0);
        playerStateMachine.Player.PlayerRigidbody.AddForce(jumpVector, ForceMode2D.Impulse);

        playerStateMachine.Player.PlayerData.PlayerAirData.UseJump();   // 점프 카운트 --
    }
}
