using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDashState : PlayerAttackState, IPlayerAttackInput
{
    public StoppableAction AttackAction = new();
    private SkillData skillData;
    private SkillSlotKey slotkey = SkillSlotKey.Z;
    public PlayerDashState(PlayerStateMachine playerStateMachine) : base(playerStateMachine)
    {
        //skillData = playerStateMachine.Player.equippedSkills[slotkey];
        //playerStateMachine.ConnectAttackState(this);
    }
    private float changeStateDelayTime = 0;

    public override void Enter()
    {
#if StateMachineDebug
        Debug.Log("Dash 스테이트 진입");
#endif
        base.Enter();
        changeStateDelayTime = 0;
        //TODO: Dash애니메이션 파라미터 활성화
        playerStateMachine.Player.playerData.PlayerStatusData.CanMove = false;
        Dash();
    }


    public override void Exit()
    {
#if StateMachineDebug
        Debug.Log("Dash 스테이트 해제");
#endif
        base.Exit();
        //TODO: Dash애니메이션 파라미터 비활성화
        ResetZeroVelocity();
        ResetDefaultGravityForce();
        playerStateMachine.Player.playerData.PlayerStatusData.CanMove = true;
    }


    public override void Update()
    {
        changeStateDelayTime += Time.deltaTime;
        Debug.Log("changeStateDelyTime = " + changeStateDelayTime);
        if (changeStateDelayTime < playerStateMachine.Player.playerData.PlayerAirData.DashChangeStateDelayTime)
        {
            return;
        }
        base.Update();

        //Idle 스테이트 진입 가능 여부 확인
        if (playerStateMachine.Player.playerCheckGround.CanJump &&
            Mathf.Approximately(playerStateMachine.Player.playerRigidbody.velocity.y, 0))
        {
            playerStateMachine.ChangeState(playerStateMachine.IdleState);
            return;
        }

        //Fall 스테이트 진입 가능 여부 확인
        if (!(playerStateMachine.Player.playerCheckGround.CanJump))
        {
            playerStateMachine.ChangeState(playerStateMachine.FallState);
            return;
        }
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
        playerStateMachine.Player.SkillCoolTimeUpdate(slotkey);
        playerStateMachine.Player.playerData.PlayerAirData.CurDashCount--;
    }

    public bool GetIsInputKey()
    {
        return playerStateMachine.Player.input.IsDash;
    }

    public SkillData GetSkillData()
    {
        return skillData;
    }
}
