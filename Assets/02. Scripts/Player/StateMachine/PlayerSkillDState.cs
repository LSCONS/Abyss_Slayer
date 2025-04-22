using UnityEngine;

public class PlayerSkillDState : PlayerSkillState
{
    private Skill SkillData { get; set; }
    private SkillSlotKey Slotkey { get; set; } = SkillSlotKey.D;
    //TODO: PlayerD스킬 데이터 가져와야함.
    public PlayerSkillDState(PlayerStateMachine playerStateMachine) : base(playerStateMachine)
    {
    }

    public void Init()
    {
        SkillData = playerStateMachine.Player.equippedSkills[Slotkey];
        playerStateMachine.ConnectSkillState(this, SkillData, () => playerStateMachine.Player.input.IsSkillD);
    }

    public override void Enter()
    {
        base.Enter();
        StartAnimation(playerStateMachine.Player.playerAnimationData.D_SkillParameterHash);
        SkillEnter(SkillData.CanMove, Slotkey);

#if StateMachineDebug
        Debug.Log("SkillDState 진입");
#endif
    }

    public override void Exit()
    {
        base.Exit();
        StopAnimation(playerStateMachine.Player.playerAnimationData.D_SkillParameterHash);
        SkillExit();
#if StateMachineDebug
        Debug.Log("SkillDState 해제");
#endif
    }

    public override void Update()
    {
        base.Update();
        if (SkillUpdate(playerStateMachine.Player.playerAnimationData.D_SkillAnimationHash,
            () => playerStateMachine.Player.input.IsSkillD)) return;
        playerStateMachine.EndAttackAction?.Invoke();
    }
}
