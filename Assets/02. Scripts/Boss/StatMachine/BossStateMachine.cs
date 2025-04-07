using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossStateMachine : StateMachine
{
    public Boss boss { get; }
    public BossStartState StartState { get; private set; }
    public PlayerWalkState WalkState { get; private set; }
    public PlayerJumpState JumpState { get; private set; }
    public PlayerFallState FallState { get; private set; }
    public float MovementSpeed { get; set; }
    public float MovementSpeedModifier { get; set; } = 1f;

    public BossStateMachine(Boss boss)
    {
        this.boss = boss;

        StartState = new BossStartState(this);
        //WalkState = new PlayerWalkState(this);
        //JumpState = new PlayerJumpState(this);
        //FallState = new PlayerFallState(this);

    }
}
