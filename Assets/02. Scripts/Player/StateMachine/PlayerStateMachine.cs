using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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


    public void ConnectAttackState(IPlayerState state)
    {
        IPlayerAttackInput input = null;
        if (state is IPlayerAttackInput) input = state as IPlayerAttackInput;
        else return;

        ApplyState applyState = input.GetSkillData().applyState;


        if ((ApplyState.IdleState | applyState) == applyState)
        {
            IdleState.AttackAction.AddListener(() => ConnectActionState(input.GetIsInputKey(), state, input.GetSkillData()));
        }

        if ((ApplyState.WalkState | applyState) == applyState)
        {
            WalkState.AttackAction.AddListener(() => ConnectActionState(input.GetIsInputKey(), state, input.GetSkillData()));
        }

        if ((ApplyState.JumpState | applyState) == applyState)
        {
            JumpState.AttackAction.AddListener(() => ConnectActionState(input.GetIsInputKey(), state, input.GetSkillData()));
        }

        if ((ApplyState.DashState | applyState) == applyState)
        {
            DashState.AttackAction.AddListener(() => ConnectActionState(input.GetIsInputKey(), state, input.GetSkillData()));
        }

        if ((ApplyState.FallState | applyState) == applyState)
        {
            FallState.AttackAction.AddListener(() => ConnectActionState(input.GetIsInputKey(), state, input.GetSkillData()));
        }
    }


    public bool ConnectActionState(bool isAction, IPlayerState state, SkillData skillData)
    {
        if (isAction && skillData.canUse)
        {
            ChangeState(state);
            return false;
        }
        return true;
    }
}
