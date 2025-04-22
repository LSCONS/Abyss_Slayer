public class PlayerGroundState : PlayerBaseState
{
    public PlayerGroundState(PlayerStateMachine playerStateMachine) : base(playerStateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        StartAnimation(playerStateMachine.Player.PlayerAnimationData.GroundParameterHash);
        playerStateMachine.Player.PlayerData.PlayerAirData.ResetDashCount();
        playerStateMachine.Player.PlayerData.PlayerAirData.ResetJumpCount();
    }

    public override void Exit()
    {
        base.Exit();
        StopAnimation(playerStateMachine.Player.PlayerAnimationData.GroundParameterHash);
    }

    public override void Update()
    {
        base.Update();
    }
}
