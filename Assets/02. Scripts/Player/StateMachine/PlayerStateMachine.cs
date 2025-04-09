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
    public PlayerDieState DieState { get; private set; }

    public StoppableAction SkipAttackAction = new();

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
        DieState = new PlayerDieState(this);
        CommonAttackState = new PlayerCommonAttackState(this);

        SkipAttackAction.AddListener(ConnectJumpState);
        SkipAttackAction.AddListener(ConnectDashState);

        MovementSpeed = Player.playerData.PlayerGroundData.BaseSpeed;
    }


    /// <summary>
    /// SkillState에서 연결 가능한 MoveState를 찾고 연결하는 메서드
    /// </summary>
    /// <param name="state">연결하고 싶은 SkillState</param>
    public void ConnectSkillState(IPlayerState state, SkillData skillData, bool isAction)
    {
        ApplyState applyState = skillData.applyState;

        if ((ApplyState.IdleState | applyState) == applyState)
        {
            IdleState.AttackAction.AddListener(() => ConnectAction(isAction, state, skillData));
        }

        if ((ApplyState.WalkState | applyState) == applyState)
        {
            WalkState.AttackAction.AddListener(() => ConnectAction(isAction, state, skillData));
        }

        if ((ApplyState.JumpState | applyState) == applyState)
        {
            JumpState.AttackAction.AddListener(() => ConnectAction(isAction, state, skillData));
        }

        if ((ApplyState.DashState | applyState) == applyState)
        {
            DashState.AttackAction.AddListener(() => ConnectAction(isAction, state, skillData));
        }

        if ((ApplyState.FallState | applyState) == applyState)
        {
            FallState.AttackAction.AddListener(() => ConnectAction(isAction, state, skillData));
        }
    }

    private bool InputKey()
    {
        return Player.input.IsAttack;
    }


    /// <summary>
    /// State에 있는 AttackAction과 연결되는 메서드
    /// </summary>
    /// <param name="isAction">입력 키 토글 여부</param>
    /// <param name="state">변환할 State</param>
    /// <param name="skillData">참고할 SkillData</param>
    /// <returns>true면 Action 종료, false면 Action 계속</returns>
    private bool ConnectAction(bool isAction, IPlayerState state, SkillData skillData)
    {
        if (InputKey() && skillData.canUse)
        {
            ChangeState(state);
            return true;
        }
        return false;
    }


    /// <summary>
    /// Walk State에 진입 가능 여부를 확인하고 전환하는 메서드
    /// </summary>
    /// <returns>true면 Action 종료, false면 Action 계속</returns>
    public bool ConnectWalkState()
    {
        if (Player.input.MoveDir.x != 0f &&
            Player.playerCheckGround.CanJump &&
            !(Player.playerGroundCollider.isTrigger) &&
            Mathf.Approximately(Player.playerRigidbody.velocity.y, 0))
        {
            ChangeState(WalkState);
            return true;
        }
        return false;
    }


    /// <summary>
    /// Fall State에 진입 가능 여부를 확인하고 전환하는 메서드
    /// </summary>
    /// <returns>true면 Action 종료, false면 Action 계속</returns>
    public bool ConnectFallState()
    {
        if (!(Player.playerCheckGround.CanJump))
        {
            ChangeState(FallState);
            return true;
        }
        return false;
    }


    /// <summary>
    /// Dash State에 진입 가능 여부를 확인하고 전환하는 메서드
    /// </summary>
    /// <returns>true면 Action 종료, false면 Action 계속</returns>
    public bool ConnectDashState()
    {
        if (Player.playerData.PlayerAirData.CanDash &&
            Player.input.IsDash &&
            (Player.input.MoveDir.x != 0 ||
            Player.input.MoveDir.y > 0) &&
            Player.playerData.PlayerAirData.CurDashCount > 0)
        {
            ChangeState(DashState);
            return true;
        }
        return false;
    }


    /// <summary>
    /// Jump State에 진입 가능 여부를 확인하고 전환하는 메서드, DownJump도 확인함.
    /// </summary>
    /// <returns>true면 Action 종료, false면 Action 계속</returns>
    public bool ConnectJumpState()
    {
        if(Player.input.IsJump &&
            Player.playerCheckGround.CanJump &&
            Mathf.Approximately(Player.playerRigidbody.velocity.y, 0))
        {
            if(Player.input.MoveDir.y >= 0)
            {
                ChangeState(JumpState);
                return true;
            }
            
            if(Player.playerCheckGround.GroundPlaneCount == 0)
            {
                Player.playerGroundCollider.isTrigger = true;
                ChangeState(FallState);
                return true;
            }
        }
        return false;
    }


    /// <summary>
    /// Idle State에 진입 가능 여부를 확인하고 전환하는 메서드
    /// </summary>
    /// <returns>true면 Action 종료, false면 Action 계속</returns>
    public bool ConnectIdleState()
    {
        if (Player.input.MoveDir == Vector2.zero &&
            Player.playerCheckGround.CanJump &&
            !(Player.playerGroundCollider.isTrigger) &&
            Mathf.Approximately(Player.playerRigidbody.velocity.y, 0))
        {
            ChangeState(IdleState);
            return true;
        }
        return false;
    }
}
