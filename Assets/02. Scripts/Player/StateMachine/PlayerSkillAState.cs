using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkillAState : PlayerSkillState
{
    private SkillData skillData;
    private SkillSlotKey slotkey = SkillSlotKey.A;
    //TODO: PlayerA스킬 데이터 가져와야함.
    public PlayerSkillAState(PlayerStateMachine playerStateMachine) : base(playerStateMachine)
    {
        //skillData = playerStateMachine.Player.equippedSkills[slotkey];
        //playerStateMachine.ConnectAttackState(this);
    }

    public override void Enter()
    {
        base.Enter();
        StartAnimation(playerStateMachine.Player.playerAnimationData.A_SkillParameterHash);

        //TODO: 스킬 사용 중 움직일 수 있는지 확인하고 실행
#if StateMachineDebug
        Debug.Log("SkillAState 진입");
#endif
    }

    public override void Exit()
    {
        base.Exit();
        StopAnimation(playerStateMachine.Player.playerAnimationData.A_SkillParameterHash);
        playerStateMachine.MovementSpeed = playerStateMachine.Player.playerData.PlayerGroundData.BaseSpeed;

#if StateMachineDebug
        Debug.Log("SkillAState 해제");
#endif
    }
}
