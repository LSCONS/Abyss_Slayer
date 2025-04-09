using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkillSState : PlayerSkillState
{
    private SkillData skillData;
    private SkillSlotKey slotkey = SkillSlotKey.S;
    public PlayerSkillSState(PlayerStateMachine playerStateMachine) : base(playerStateMachine)
    {
        //skillData = playerStateMachine.Player.equippedSkills[slotkey];
        //playerStateMachine.ConnectAttackState(this);
    }

    public override void Enter()
    {
        base.Enter();
        //TODO: 스킬 사용 중 움직일 수 있는지 확인하고 실행
        //TODO: 스킬 S 애니메이터 활성화
    }

    public override void Exit()
    {
        base.Exit();
        //TODO: 스킬 S 애니메이터 비활성화
        playerStateMachine.MovementSpeed = playerStateMachine.Player.playerData.PlayerGroundData.BaseSpeed;
    }
}
