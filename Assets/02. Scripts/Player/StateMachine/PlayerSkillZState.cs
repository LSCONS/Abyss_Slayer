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
        playerStateMachine.ConnectSkillState(this, SkillData, () => playerStateMachine.Player.input.IsSkillZ);

        MoveAction.AddListener(playerStateMachine.ConnectIdleState);
        MoveAction.AddListener(playerStateMachine.ConnectFallState);
        MoveAction.AddListener(playerStateMachine.ConnectWalkState);
    }

    public override void Enter()
    {
        base.Enter();
        StartAnimation(playerStateMachine.Player.playerAnimationData.Z_SkillParameterHash);
        SkillEnter(SkillData.canMove, Slotkey);
        ResetZeroGravityForce();

#if StateMachineDebug
        Debug.Log("Dash 스테이트 진입");
#endif
    }


    public override void Exit()
    {
        base.Exit();
        StopAnimation(playerStateMachine.Player.playerAnimationData.Z_SkillParameterHash);
        SkillExit();
        ResetZeroVelocity();
        ResetDefaultGravityForce();

#if StateMachineDebug
        Debug.Log("Dash 스테이트 해제");
#endif
    }


    public override void Update()
    {
        base.Update();
        if(SkillUpdate(playerStateMachine.Player.playerAnimationData.Z_SkillAnimationHash)) return;
        MoveAction?.Invoke();
    }
}
