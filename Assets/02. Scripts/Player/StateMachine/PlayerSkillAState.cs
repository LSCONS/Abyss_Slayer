using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkillAState : PlayerSkillState
{
    private SkillData SkillData { get; set; }
    private SkillSlotKey Slotkey { get; set; } = SkillSlotKey.A;
    public PlayerSkillAState(PlayerStateMachine playerStateMachine) : base(playerStateMachine)
    {
        SkillData = playerStateMachine.Player.equippedSkills[Slotkey];
        playerStateMachine.ConnectSkillState(this, SkillData, () => playerStateMachine.Player.input.IsSkillA);
    }

    public override void Enter()
    {
        base.Enter();
        StartAnimation(playerStateMachine.Player.playerAnimationData.A_SkillParameterHash);
        AttackEnter(SkillData.canMove, Slotkey);

#if StateMachineDebug
        Debug.Log("SkillAState 진입");
#endif
    }

    public override void Exit()
    {
        base.Exit();
        StopAnimation(playerStateMachine.Player.playerAnimationData.A_SkillParameterHash);
        AttackExit();

#if StateMachineDebug
        Debug.Log("SkillAState 해제");
#endif
    }

    public override void Update()
    {
        base.Update();
        if (SkillUpdate(playerStateMachine.Player.playerAnimationData.A_SkillAnimationHash)) return;
        playerStateMachine.EndAttackAction?.Invoke();
    }
}
