using UnityEngine;

public class PlayerSkillZState : PlayerSkillState
{
    public StoppableAction MoveAction = new();
    private Skill SkillData {  get; set; }
    private SkillSlotKey Slotkey { get; set; } = SkillSlotKey.Z;
    public PlayerSkillZState(PlayerStateMachine playerStateMachine) : base(playerStateMachine)
    {
    }

    public void Init()
    {
        SkillData = playerStateMachine.Player.equippedSkills[Slotkey];
        playerStateMachine.ConnectSkillState(this, SkillData, () => playerStateMachine.Player.Input.IsSkillZ);

        MoveAction.AddListener(playerStateMachine.ConnectIdleState);
        MoveAction.AddListener(playerStateMachine.ConnectFallState);
        MoveAction.AddListener(playerStateMachine.ConnectWalkState);
    }

    public override void Enter()
    {
        base.Enter();
        StartAnimation(playerStateMachine.Player.PlayerAnimationData.Z_SkillParameterHash);
        SkillEnter(SkillData.CanMove, Slotkey);
        ResetZeroGravityForce();
        playerStateMachine.IsDash = true;

#if StateMachineDebug
        Debug.Log("Dash 스테이트 진입");
#endif
    }


    public override void Exit()
    {
        base.Exit();
        StopAnimation(playerStateMachine.Player.PlayerAnimationData.Z_SkillParameterHash);
        SkillExit();
        ResetZeroVelocity();
        ResetDefaultGravityForce();
        playerStateMachine.IsDash = false;

#if StateMachineDebug
        Debug.Log("Dash 스테이트 해제");
#endif
    }


    public override void Update()
    {
        base.Update();
        if (SkillUpdate(playerStateMachine.Player.PlayerAnimationData.Z_SkillAnimationHash,
            () => playerStateMachine.Player.Input.IsSkillZ)) return;
        MoveAction?.Invoke();
    }
}
