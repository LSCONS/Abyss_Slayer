using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCommonAttackState : PlayerSkillState
{
    private SkillData skillData;
    private SkillSlotKey slotkey = SkillSlotKey.X;
    //TODO: 임시 코드 추가. 나중에 삭제 필요
    private float EnterUpdateTime = 0f;
    private float ChangeStateDelayTime = 0.5f;
    public PlayerCommonAttackState(PlayerStateMachine playerStateMachine) : base(playerStateMachine)
    {
        skillData = playerStateMachine.Player.equippedSkills[slotkey];
        playerStateMachine.ConnectSkillState(this, skillData, playerStateMachine.Player.input.IsAttack);
    }
    public override void Enter()
    {
        base.Enter();
        if (!(skillData.canMove)) playerStateMachine.MovementSpeed = 0f;
        //TODO: 스킬 X 애니메이터 활성화
        skillData.Execute(playerStateMachine.Player, null);
        playerStateMachine.Player.SkillCoolTimeUpdate(slotkey);
        EnterUpdateTime = 0f;
    }

    public override void Exit()
    {
        base.Exit();
        //TODO: 스킬 X 애니메이터 비활성화
        playerStateMachine.MovementSpeed = playerStateMachine.Player.playerData.PlayerGroundData.BaseSpeed;
    }

    public override void Update()
    {
        base.Update();
        playerStateMachine.SkipAttackAction?.Invoke();
        EnterUpdateTime += Time.deltaTime;
        if(EnterUpdateTime <= ChangeStateDelayTime)
        {
            return;
        }

        //TODO: 임시 코드
        playerStateMachine.ChangeState(playerStateMachine.IdleState);
    }
}
