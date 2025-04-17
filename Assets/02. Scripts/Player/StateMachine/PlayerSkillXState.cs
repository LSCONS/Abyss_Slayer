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
        playerStateMachine.ConnectSkillState(this, SkillData, () => playerStateMachine.Player.input.IsSkillX);
    }

    public override void Enter()
    {
        base.Enter();
        StartAnimation(playerStateMachine.Player.playerAnimationData.X_SkillParameterHash);
        SkillEnter(SkillData.canMove, Slotkey);

#if StateMachineDebug
        Debug.Log("SkillXState 진입");
#endif
    }

    public override void Exit()
    {
        base.Exit();
        StopAnimation(playerStateMachine.Player.playerAnimationData.X_SkillParameterHash);
        SkillExit();

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
