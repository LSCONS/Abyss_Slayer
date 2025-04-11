using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class PlayerSkillZState : PlayerSkillState
{
    public StoppableAction MoveAction = new();
    private SkillData SkillData {  get; set; }
    private SkillSlotKey Slotkey { get; set; } = SkillSlotKey.Z;
    public PlayerSkillZState(PlayerStateMachine playerStateMachine) : base(playerStateMachine)
    {
        SkillData = playerStateMachine.Player.equippedSkills[Slotkey];
        playerStateMachine.ConnectSkillState(this, SkillData, () => playerStateMachine.Player.input.IsDash);

        MoveAction.AddListener(playerStateMachine.ConnectIdleState);
        MoveAction.AddListener(playerStateMachine.ConnectFallState);
        MoveAction.AddListener(playerStateMachine.ConnectWalkState);
    }

    public override void Enter()
    {
        base.Enter();
        StartAnimation(playerStateMachine.Player.playerAnimationData.Z_SkillParameterHash);
        if (!(SkillData.canMove)) 
        {
            playerStateMachine.MovementSpeed = 0f;
            ResetZeroVelocity();
        } 
        ResetZeroGravityForce();
        playerStateMachine.IsCompareState = false;
        playerStateMachine.Player.SkillCoolTimeUpdate(Slotkey);
        SkillData.canUse = false;

#if StateMachineDebug
        Debug.Log("Dash 스테이트 진입");
#endif
    }


    public override void Exit()
    {
        base.Exit();
        StopAnimation(playerStateMachine.Player.playerAnimationData.Z_SkillParameterHash);
        ResetZeroVelocity();
        ResetDefaultGravityForce();
        playerStateMachine.MovementSpeed = playerStateMachine.Player.playerData.PlayerGroundData.BaseSpeed;
        playerStateMachine.Player.SkillTrigger.StopSkillCoroutine();

#if StateMachineDebug
        Debug.Log("Dash 스테이트 해제");
#endif
    }


    public override void Update()
    {
        base.Update();
        playerStateMachine.AnimatorInfo = playerStateMachine.Player.PlayerAnimator.GetCurrentAnimatorStateInfo(0);

        //해당 State의 애니메이터와 연결 완료
        if (!(playerStateMachine.IsCompareState))
        {
            if (playerStateMachine.AnimatorInfo.shortNameHash == playerStateMachine.Player.playerAnimationData.Z_SkillAnimationHash)
            {
                playerStateMachine.IsCompareState = true;
            }
            else return;
        }

        if (playerStateMachine.AnimatorInfo.normalizedTime < 1f)
        {
            return;
        }
        MoveAction?.Invoke();
    }

    protected void Dash()
    {
        Vector2 DashVector = playerStateMachine.Player.input.MoveDir.normalized;
        DashVector *= playerStateMachine.Player.playerData.PlayerAirData.DashForce;
        ResetZeroVelocity();
        ResetZeroGravityForce();
        FlipRenderer(DashVector.x);   
        playerStateMachine.Player.playerRigidbody.AddForce(DashVector, ForceMode2D.Impulse);
        playerStateMachine.Player.playerData.PlayerAirData.CanDash = false;
        playerStateMachine.Player.SkillCoolTimeUpdate(Slotkey);
        playerStateMachine.Player.playerData.PlayerAirData.CurDashCount--;
    }
}
