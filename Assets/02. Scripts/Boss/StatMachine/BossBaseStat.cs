using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBaseState : IPlayerState
{
    protected BossStateMachine bossStateMachine;
    Boss boss;

    public BossBaseState(BossStateMachine bossStateMachine)
    {
        this.bossStateMachine = bossStateMachine;
        boss = bossStateMachine.boss;
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
