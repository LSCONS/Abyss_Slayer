public class PlayerAirState : PlayerBaseState
{
    public PlayerAirState(PlayerStateMachine playerStateMachine) : base(playerStateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    public override void Update()
    {
        base.Update();
    }


    /// <summary>
    /// 아래 키 + 점프 입력 시 밑점
    /// </summary>
    protected void CheckDownJump(bool changeToFall = true)
    {
        var input = playerStateMachine.Player.input;

        if (input.IsJump &&
            input.MoveDir.y < 0f &&
            playerStateMachine.Player.playerCheckGround.GroundPlatformCount > 0)
        {
            playerStateMachine.Player.PlayerGroundCollider.isTrigger = true;
            playerStateMachine.DidDownJump = true;
            playerStateMachine.StartDownJumpCooldown(); // 점프 키를 뗄 때까지 이단 점프 제한

            if (changeToFall)
                playerStateMachine.ChangeState(playerStateMachine.FallState);
        }
    }

    /// <summary>
    /// 점프 키 새로 눌리고, 점프 횟수 남아있을 때 2단 점프 실행
    /// </summary>
    protected void CheckDoubleJump(ref bool hasJumpedInThisFrame)
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
        else if (!hasJumpedInThisFrame &&
                 input.IsJump &&
                 playerStateMachine.Player.PlayerData.PlayerAirData.CanJump())
        {
            hasJumpedInThisFrame = true;
            playerStateMachine.ChangeState(playerStateMachine.JumpState);
        }

        if (!input.IsJump)
        {
            hasJumpedInThisFrame = false;
        }
    }

    /// <summary>
    /// 점프 키 새로 눌리고, 점프 횟수 남아있을 때 2단 점프 실행
    /// </summary>
    protected void CheckDoubleJump()
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
    protected void UpdateJumpInputMemory()
    {
        playerStateMachine.WasJumpPressedLastFrame = playerStateMachine.Player.input.IsJump;
    }
}
