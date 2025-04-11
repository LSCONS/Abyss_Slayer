using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBaseState : IPlayerState
{
    protected PlayerStateMachine playerStateMachine;
    protected readonly PlayerGroundData playerGroundData;

    public PlayerBaseState(PlayerStateMachine playerStateMachine)
    {
        this.playerStateMachine = playerStateMachine;
        playerGroundData = playerStateMachine.Player.playerData.PlayerGroundData;
    }

    public virtual void Enter()
    {

    }

    public virtual void Exit()
    {

    }

    public virtual void FixedUpdate()
    {
        if (playerStateMachine.MovementSpeed != 0f)
        {
            Move();
        }
    }

    public virtual void Update()
    {

    }


    /// <summary>
    /// 매개변수로 들어온 애니메이션의 파라미터를 활성화 시켜줄 메서드.
    /// </summary>
    /// <param name="animatorHash">활성화 시킬 애니메이션의 해시태그</param>
    protected void StartAnimation(int animatorHash)
    {
        playerStateMachine.Player.PlayerAnimator.SetBool(animatorHash, true);
    }


    /// <summary>
    /// 매개변수로 들어온 애니메이션의 파라미터를 비활성화 시켜줄 메서드.
    /// </summary>
    /// <param name="animatorHash">비활성화 시킬 애니메이션의 해시태그</param>
    protected void StopAnimation(int animatorHash)
    {
        playerStateMachine.Player.PlayerAnimator.SetBool(animatorHash, false);
    }


    /// <summary>
    /// 플레이어를 입력한 방향으로 움직이게 만들 메서드
    /// </summary>
    private void Move()
    {
        float newMoveX = playerStateMachine.Player.input.MoveDir.x * GetMovementSpeed();
        float nowMoveY = playerStateMachine.Player.playerRigidbody.velocity.y;
        playerStateMachine.Player.playerRigidbody.velocity = new Vector2(newMoveX, nowMoveY);
        FlipRenderer(newMoveX); //플레이어의 바라보는 방향을 바꿔주는 메서드
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
    /// 플레이어가 X좌표로 움직이는 방향을 계산하고 바꿔주는 메서드
    /// </summary>
    /// <param name="nowMoveX">움직이고 있는 X좌표값의 크기</param>
    protected void FlipRenderer(float nowMoveX)
    {
        if(nowMoveX > 0)
        {
            playerStateMachine.Player.SpriteRenderer.flipX = false;
        }
        else if (nowMoveX < 0)
        {
            playerStateMachine.Player.SpriteRenderer.flipX = true;
        }
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
            playerStateMachine.Player.playerData.PlayerStatusData.GravityForce;
    }
}
