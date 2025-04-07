using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBaseState : IPlayerState
{
    protected PlayerStateMachine playerStateMachine;
    protected readonly PlayerGroundData playerGroundData;

    public BossBaseState(PlayerStateMachine playerStateMachine)
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

    }

    public virtual void HandleInput()
    {

    }

    public virtual void Update()
    {

    }
}
