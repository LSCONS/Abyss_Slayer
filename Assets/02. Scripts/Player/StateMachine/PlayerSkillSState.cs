using UnityEngine;

public class PlayerSkillSState : PlayerSkillState
{
    private SkillData SkillData { get; set; }
    private SkillSlotKey Slotkey { get; set; } = SkillSlotKey.S;
    public PlayerSkillSState(PlayerStateMachine playerStateMachine) : base(playerStateMachine)
    {
        SkillData = playerStateMachine.Player.equippedSkills[Slotkey];
        playerStateMachine.ConnectSkillState(this, SkillData, () => playerStateMachine.Player.input.IsSkillS);
    }

    public override void Enter()
    {
        base.Enter();
        StartAnimation(playerStateMachine.Player.playerAnimationData.S_SkillParameterHash);
        SkillEnter(SkillData.canMove, Slotkey);

#if StateMachineDebug
        Debug.Log("SkillSState 진입");
#endif
    }

    public override void Exit()
    {
        base.Exit();
        StopAnimation(playerStateMachine.Player.playerAnimationData.S_SkillParameterHash);
        SkillExit();

#if StateMachineDebug
        Debug.Log("SkillSState 해제");
#endif
    }

    public override void Update()
    {
        base.Update();
        if (SkillUpdate(playerStateMachine.Player.playerAnimationData.S_SkillAnimationHash)) return;
        playerStateMachine.EndAttackAction?.Invoke();
    }
}
