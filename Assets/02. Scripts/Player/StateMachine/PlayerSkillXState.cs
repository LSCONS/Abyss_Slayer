using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkillXState : PlayerSkillState
{
    private SkillData SkillData { get; set; }
    private SkillSlotKey Slotkey { get; set; } = SkillSlotKey.X;
    public PlayerSkillXState(PlayerStateMachine playerStateMachine) : base(playerStateMachine)
    {
        SkillData = playerStateMachine.Player.equippedSkills[Slotkey];
        playerStateMachine.ConnectSkillState(this, SkillData,() => playerStateMachine.Player.input.IsSkillX);
    }
    public override void Enter()
    {
        base.Enter();
        StartAnimation(playerStateMachine.Player.playerAnimationData.X_SkillParameterHash);
        AttackEnter(SkillData.canMove, Slotkey);

#if StateMachineDebug
        Debug.Log("SkillXState 진입");
#endif
    }

    public override void Exit()
    {
        base.Exit();
        StopAnimation(playerStateMachine.Player.playerAnimationData.X_SkillParameterHash);
        AttackExit();

#if StateMachineDebug
        Debug.Log("SkillXState 해제");
#endif
    }

    public override void Update()
    {
        base.Update();
        if (SkillUpdate(playerStateMachine.Player.playerAnimationData.X_SkillAnimationHash)) return;
        playerStateMachine.EndAttackAction?.Invoke();
    }
}
