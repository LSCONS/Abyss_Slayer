using UnityEngine;

public class PlayerSkillAState : PlayerSkillState
{
    private Skill SkillData { get; set; }
    private SkillSlotKey Slotkey { get; set; } = SkillSlotKey.A;
    public PlayerSkillAState(PlayerStateMachine playerStateMachine) : base(playerStateMachine)
    {
    }

    public void Init()
    {
        SkillData = playerStateMachine.Player.equippedSkills[Slotkey];
        playerStateMachine.ConnectSkillState(this, SkillData, () => playerStateMachine.Player.Input.IsSkillA);
    }

    public override void Enter()
    {
        base.Enter();
        StartAnimation(playerStateMachine.Player.PlayerAnimationData.A_SkillParameterHash);
        SkillEnter(SkillData.CanMove, Slotkey);

#if StateMachineDebug
        Debug.Log("SkillAState 진입");
#endif
    }

    public override void Exit()
    {
        base.Exit();
        StopAnimation(playerStateMachine.Player.PlayerAnimationData.A_SkillParameterHash);
        SkillExit();

#if StateMachineDebug
        Debug.Log("SkillAState 해제");
#endif
    }

    public override void Update()
    {
        base.Update();
        if (SkillUpdate(playerStateMachine.Player.PlayerAnimationData.A_SkillAnimationHash,
            () => playerStateMachine.Player.Input.IsSkillA)) return;
        playerStateMachine.EndAttackAction?.Invoke();
    }
}
