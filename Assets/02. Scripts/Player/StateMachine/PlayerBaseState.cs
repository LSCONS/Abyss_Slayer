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
        Move();
    }

    public virtual void HandleInput()
    {

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
    /// 플레이어가 점프를 실행할 때 실행할 메서드
    /// </summary>
    protected void Jump()
    {
        if(Mathf.Approximately(playerStateMachine.Player.playerRigidbody.velocity.y, 0))
        {
            Vector2 jumpForce = playerStateMachine.Player.playerData.PlayerAirData.JumpForce * Vector2.up;
            playerStateMachine.Player.playerRigidbody.AddForce(jumpForce, ForceMode2D.Impulse);
        }
    }


    protected void Dash()
    {
        Vector2 MoveDir = playerStateMachine.Player.input.MoveDir.normalized;
        
    }
}
