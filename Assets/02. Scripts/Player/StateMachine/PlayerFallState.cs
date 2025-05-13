using UnityEngine;

public class PlayerFallState : PlayerAirState
{
    public StoppableAction MoveAction = new();
    private int animationNum = 0;
    private float animationTime = 0;
    private int animationDelay = 10;
    public PlayerFallState(PlayerStateMachine playerStateMachine) : base(playerStateMachine)
    {
    }

    public void Init()
    {
        //Idle State 진입 가능 여부 확인
        MoveAction.AddListener(playerStateMachine.ConnectIdleState);
        //Walk State 진입 가능 여부 확인
        MoveAction.AddListener(playerStateMachine.ConnectWalkState);
        //Dash State 진입 가능 여부 확인
        MoveAction.AddListener(playerStateMachine.ConnectDashState);

    }

    public override void Enter()
    {
        base.Enter();
        playerStateMachine.Player.PlayerSpriteChange.SetOnceAnimation(AnimationState.Fall, 0);
        animationNum = 0;
        animationTime = animationDelay;

#if StateMachineDebug
        Debug.Log("FallState 진입");
#endif
    }

    public override void Exit()
    {
        base.Exit();

#if StateMachineDebug
        Debug.Log("FallState 해제");
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

        CheckDownJump();                        // 밑점프 체크
        CheckDoubleJump();                      // 2단 점프 체크
        UpdateJumpInputMemory();                // 점프 키 입력 상태 저장
        UpdateFallAnimation();                  // 밑점 애니메이션 업데이트

        MoveAction?.Invoke();
    }

    /// <summary>
    /// 아래 키 + 점프 입력 시 밑점
    /// </summary>
    private void CheckDownJump()
    {
        var input = playerStateMachine.Player.input;

        if (input.IsJump &&
            input.MoveDir.y < 0f &&
            playerStateMachine.Player.playerCheckGround.GroundPlatformCount > 0)
        {
            playerStateMachine.Player.PlayerGroundCollider.isTrigger = true;
            playerStateMachine.DidDownJump = true;
            playerStateMachine.StartDownJumpCooldown(); // 점프 키를 뗄 때까지 이단 점프 제한
            playerStateMachine.ChangeState(playerStateMachine.FallState);
        }
    }

    /// <summary>
    /// 점프 키 새로 눌리고, 점프 횟수 남아있을 때 2단 점프 실행
    /// </summary>
    private void CheckDoubleJump()
    {
        var input = playerStateMachine.Player.input;

        if (playerStateMachine.DidDownJump)
        {
            // 아래 점프 중에는 점프 안되고 키를 떼면 false
            if (!input.IsJump)
            {
                playerStateMachine.DidDownJump = false;
            }
        }
        else if (input.IsJump &&
                 !playerStateMachine.WasJumpPressedLastFrame &&
                 playerStateMachine.Player.PlayerData.PlayerAirData.CanJump())
        {
            playerStateMachine.ChangeState(playerStateMachine.JumpState);
        }
    }

    /// <summary>
    /// 점프 키가 이번 프레임에 눌렸는지 판단하기 위해 입력 상태 저장
    /// </summary>
    private void UpdateJumpInputMemory()
    {
        playerStateMachine.WasJumpPressedLastFrame = playerStateMachine.Player.input.IsJump;
    }

    /// <summary>
    /// fall 애니메이션 업데이트
    /// </summary>
    private void UpdateFallAnimation()
    {
        if (animationTime > 0) return;

        animationTime = animationDelay;
        playerStateMachine.Player.PlayerSpriteChange.SetOnceAnimation(AnimationState.Fall, ++animationNum);
    }

}
