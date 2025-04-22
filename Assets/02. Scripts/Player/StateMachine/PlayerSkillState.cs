public class PlayerSkillState : PlayerBaseState
{
    public PlayerSkillState(PlayerStateMachine playerStateMachine) : base(playerStateMachine)
    {
        
    }

    public override void Enter()
    {
        base.Enter();
        StartAnimation(playerStateMachine.Player.PlayerAnimationData.SkillParameterHash);
    }

    public override void Exit() 
    {
        base.Exit();
        StopAnimation(playerStateMachine.Player.PlayerAnimationData.SkillParameterHash);
    }
    public override void Update()
    {
        base.Update();
    }
}
