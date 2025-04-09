using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkillDState : PlayerSkillState
{
    private SkillData skillData;
    private SkillSlotKey slotkey = SkillSlotKey.D;
    //TODO: PlayerD스킬 데이터 가져와야함.
    public PlayerSkillDState(PlayerStateMachine playerStateMachine) : base(playerStateMachine)
    {
        //skillData = playerStateMachine.Player.equippedSkills[slotkey];
        //playerStateMachine.ConnectAttackState(this);
    }
    public override void Enter()
    {
        base.Enter();
        //TODO: 스킬 사용 중 움직일 수 있는지 확인하고 실행
        //TODO: 스킬 D 애니메이터 활성화
#if StateMachineDebug
        Debug.Log("SkillDState 진입");
#endif
    }

    public override void Exit()
    {
        base.Exit();
        //TODO: 스킬 D 애니메이터 비활성화
        playerStateMachine.MovementSpeed = playerStateMachine.Player.playerData.PlayerGroundData.BaseSpeed;
#if StateMachineDebug
        Debug.Log("SkillDState 해제");
#endif
    }

    public override void Update()
    {
        base.Update();
    }
}
