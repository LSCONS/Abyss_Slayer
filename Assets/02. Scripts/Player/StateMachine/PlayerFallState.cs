using UnityEngine;

public class PlayerFallState : PlayerAirState
{
    public StoppableAction MoveAction = new();
    private int animationNum = 0;
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
    /// fall 애니메이션 업데이트
    /// </summary>
    private void UpdateFallAnimation()
    {
        if (ChangeSpriteTime + CurTime < Time.time)
        {
            CurTime = ChangeSpriteTime + CurTime;
            playerStateMachine.Player.PlayerSpriteChange.SetOnceAnimation(AnimationState.Fall, ++animationNum);
        }
    }
}
