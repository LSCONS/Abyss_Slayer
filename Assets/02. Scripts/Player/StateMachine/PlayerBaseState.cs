using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerBaseState : IPlayerState
{
    protected PlayerStateMachine playerStateMachine;
    protected readonly PlayerGroundData playerGroundData;
    public float ChangeSpriteTime { get; set; } = 0.2f;
    public float CurTime { get; set; }

    public PlayerBaseState(PlayerStateMachine playerStateMachine)
    {
        this.playerStateMachine = playerStateMachine;
        playerGroundData = playerStateMachine.Player.PlayerData.PlayerGroundData;
    }

    public virtual void Enter()
    {
        CurTime = Time.time;
    }

    public virtual void Exit()
    {

    }

    public virtual void FixedUpdate()
    {
        if (!(playerStateMachine.Player.Runner.IsServer)) return;

        if (playerStateMachine.MovementSpeed != 0f)
        {
            Move();
        }

        if(playerStateMachine.Player.PlayerPosition != (Vector2)playerStateMachine.Player.transform.position)
        {
            playerStateMachine.Player.Rpc_PlayerPositionSynchro(playerStateMachine.Player.transform.position);
        }
    }

    public virtual void Update()
    {

    }


    /// <summary>
    /// SkillState에서 해제될 경우 필수적으로 실행해야하는 메서드
    /// </summary>
    protected void SkillExit(Skill skill)
    {
        playerStateMachine.MovementSpeed = playerStateMachine.Player.PlayerData.PlayerGroundData.BaseSpeed;
        ResetDefaultGravityForce();
        if(skill.SkillCategory == SkillCategory.Hold)
        playerStateMachine.Player.StopHoldSkillActionCoroutine();
    }


    /// <summary>
    /// SkillState의 Update에서 필수적으로 실행해야하는 메서드
    /// </summary>
    /// <param name="SkillAnimationHash">해당 애니메이션의 Hash값</param>
    /// <returns>true면 Update종료, false면 계속 실행</returns>
    protected bool SkillUpdate(int SkillAnimationHash, System.Func<bool> isAction)
    {
        //TODO: 매 프레임 초기화 시켜주는 방식. 큰 리로스 차지는 없지만 나중에 리팩토링 때 고려 필요.
        //playerStateMachine.AnimatorInfo = playerStateMachine.Player.PlayerAnimator.GetCurrentAnimatorStateInfo(0);

        //해당 State의 애니메이터와 연결 완료
        if (!(playerStateMachine.IsCompareState))
        {
            if (playerStateMachine.AnimatorInfo.shortNameHash == SkillAnimationHash)
            {
                playerStateMachine.IsCompareState = true;
            }
            return true;
        }
        else if (playerStateMachine.AnimatorInfo.normalizedTime < 1f)
        {
            playerStateMachine.SkipAttackAction?.Invoke();
            playerStateMachine.CheckHoldSkillStop(this, isAction);
            return true;
        }
        return false;
    }


    /// <summary>
    /// 플레이어를 입력한 방향으로 움직이게 만들 메서드
    /// </summary>
    private void Move()
    {
        float newMoveX = playerStateMachine.Player.NetworkInput.MoveDir.x * GetMovementSpeed();
        float nowMoveY = playerStateMachine.Player.playerRigidbody.velocity.y;
        playerStateMachine.Player.playerRigidbody.velocity = new Vector2(newMoveX, nowMoveY);
        playerStateMachine.Player.FlipRenderer(newMoveX); //플레이어의 바라보는 방향을 바꿔주는 메서드
    }


    /// <summary>
    /// 플레이어의 이동 가능한 스피드를 계산하는 메서드
    /// </summary>
    /// <returns>float로 속도 값을 반환</returns>
    private float GetMovementSpeed()
    {
        float moveSpeedX = playerStateMachine.MovementSpeed * playerStateMachine.MovementSpeedModifier;
        return moveSpeedX;
    }


    /// <summary>
    /// 플레이어의 Rigidbody의 Velocity값을 초기화하는 메서드
    /// </summary>
    protected void ResetZeroVelocity()
    {
        playerStateMachine.Player.playerRigidbody.velocity = Vector2.zero;
    }


    /// <summary>
    /// 플레이어의 Rigidbody의 Gravity값을 0으로 초기화하는 메서드
    /// </summary>
    protected void ResetZeroGravityForce()
    {
        playerStateMachine.Player.playerRigidbody.gravityScale = 0f;
    }


    /// <summary>
    /// 플레이어의 Rigidbody의 Gravity값을 기본값으로 초기화하는 메서드
    /// </summary>
    protected void ResetDefaultGravityForce()
    {
        playerStateMachine.Player.playerRigidbody.gravityScale = 
            playerStateMachine.Player.PlayerData.PlayerStatusData.GravityForce;
    }

    protected Func<bool> SlotKeyConvertFunc(SkillSlotKey key)
    {
        Func<bool> temp = key switch
        {
            SkillSlotKey.X => () => playerStateMachine.Player.NetworkInput.IsSkillX,
            SkillSlotKey.Z => () => playerStateMachine.Player.NetworkInput.IsSkillZ,
            SkillSlotKey.A => () => playerStateMachine.Player.NetworkInput.IsSkillA,
            SkillSlotKey.S => () => playerStateMachine.Player.NetworkInput.IsSkillS,
            SkillSlotKey.D => () => playerStateMachine.Player.NetworkInput.IsSkillD,
            _ => () => false
        };
        return temp;
    }
}
