public class PlayerDeadState : PlayerBaseState
{
    public PlayerDeadState(PlayerStateMachine playerStateMachine) : base(playerStateMachine)
    {
    }
    public void Init()
    {

    }

    public override void Enter()
    {
        base.Enter();
        StartAnimation(playerStateMachine.Player.playerAnimationData.DeadParameterHash);
        playerStateMachine.MovementSpeed = 0f;
        ResetZeroVelocity();

#if StateMachineDebug
        Debug.Log("IdleState 진입");
#endif
    }

    public override void Exit()
    {
        base.Exit();
        StopAnimation(playerStateMachine.Player.playerAnimationData.idleParameterHash);
        playerStateMachine.MovementSpeed = playerStateMachine.Player.playerData.PlayerGroundData.BaseSpeed;

#if StateMachineDebug
        Debug.Log("IdleState 해제");
#endif
    }

    public override void Update()
    {
        base.Update();
    }
}
