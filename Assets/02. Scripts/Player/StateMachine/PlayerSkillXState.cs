using UnityEngine;

public class PlayerSkillXState : PlayerSkillState
{
    private Skill SkillData { get; set; }
    private SkillSlotKey Slotkey { get; set; } = SkillSlotKey.X;
    public PlayerSkillXState(PlayerStateMachine playerStateMachine) : base(playerStateMachine)
    {
    }

    public void Init()
    {
        SkillData = playerStateMachine.Player.equippedSkills[Slotkey];
        playerStateMachine.ConnectSkillState(this, SkillData, () => playerStateMachine.Player.Input.IsSkillX);
    }

    public override void Enter()
    {
        base.Enter();
        StartAnimation(playerStateMachine.Player.PlayerAnimationData.X_SkillParameterHash);
        SkillEnter(SkillData.CanMove, Slotkey);

#if StateMachineDebug
        Debug.Log("SkillXState 진입");
#endif
    }

    public override void Exit()
    {
        base.Exit();
        StopAnimation(playerStateMachine.Player.PlayerAnimationData.X_SkillParameterHash);
        SkillExit();

#if StateMachineDebug
        Debug.Log("SkillXState 해제");
#endif
    }

    public override void Update()
    {
        base.Update();
        if (SkillUpdate(playerStateMachine.Player.PlayerAnimationData.X_SkillAnimationHash,
            () => playerStateMachine.Player.Input.IsSkillX)) return;
        playerStateMachine.EndAttackAction?.Invoke();
    }
}
