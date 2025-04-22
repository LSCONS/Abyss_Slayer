using UnityEngine;

public class PlayerSkillSState : PlayerSkillState
{
    private Skill SkillData { get; set; }
    private SkillSlotKey Slotkey { get; set; } = SkillSlotKey.S;
    public PlayerSkillSState(PlayerStateMachine playerStateMachine) : base(playerStateMachine)
    {
    }

    public void Init()
    {
        SkillData = playerStateMachine.Player.equippedSkills[Slotkey];
        playerStateMachine.ConnectSkillState(this, SkillData, () => playerStateMachine.Player.Input.IsSkillS);
    }

    public override void Enter()
    {
        base.Enter();
        StartAnimation(playerStateMachine.Player.PlayerAnimationData.S_SkillParameterHash);
        SkillEnter(SkillData.CanMove, Slotkey);

#if StateMachineDebug
        Debug.Log("SkillSState 진입");
#endif
    }

    public override void Exit()
    {
        base.Exit();
        StopAnimation(playerStateMachine.Player.PlayerAnimationData.S_SkillParameterHash);
        SkillExit();

#if StateMachineDebug
        Debug.Log("SkillSState 해제");
#endif
    }

    public override void Update()
    {
        base.Update();
        if (SkillUpdate(playerStateMachine.Player.PlayerAnimationData.S_SkillAnimationHash,
            () => playerStateMachine.Player.Input.IsSkillS)) return;
        playerStateMachine.EndAttackAction?.Invoke();
    }
}
