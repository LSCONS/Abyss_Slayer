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

    protected void StartAnimation(int animatorHash)
    {
        playerStateMachine.Player.PlayerAnimator.SetBool(animatorHash, true);
    }

    protected void StopAnimation(int animatorHash)
    {
        playerStateMachine.Player.PlayerAnimator.SetBool(animatorHash, false);
    }

    private void Move()
    {
        float newMoveX = playerStateMachine.Player.input.MoveDir.x * GetMovementSpeed();
        float nowMoveY = playerStateMachine.Player.playerRigidbody.velocity.y;
        playerStateMachine.Player.playerRigidbody.velocity = new Vector2(newMoveX, nowMoveY);
    }

    private float GetMovementSpeed()
    {
        float moveSpeedX = playerStateMachine.MovementSpeed * playerStateMachine.MovementSpeedModifier;
        return moveSpeedX;
    }
}
