using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateMachine : StateMachine
{
    public Player Player { get; }
    public PlayerIdleState IdleState { get; private set; }
    public PlayerWalkState WalkState { get; private set; }
    public PlayerJumpState JumpState { get; private set; }
    public PlayerFallState FallState { get; private set; }
    public PlayerDashState DashState { get; private set; }
    public PlayerCommonAttackState CommonAttackState { get; private set; }
    public PlayerSkillAState SkillAState { get; private set; }
    public PlayerSkillSState SkillSState { get; private set; }
    public PlayerSkillDState SkillDState { get; private set; }

    public float MovementSpeed {  get; set; }
    public float MovementSpeedModifier { get; set; } = 1f;

    public PlayerStateMachine(Player player)
    {
        this.Player = player;

        IdleState = new PlayerIdleState(this);
        WalkState = new PlayerWalkState(this);
        JumpState = new PlayerJumpState(this);
        FallState = new PlayerFallState(this);
        DashState = new PlayerDashState(this);
        SkillAState = new PlayerSkillAState(this);
        SkillSState = new PlayerSkillSState(this);
        SkillDState = new PlayerSkillDState(this);
        CommonAttackState = new PlayerCommonAttackState(this);

        MovementSpeed = Player.playerData.PlayerGroundData.BaseSpeed;
    }
}
