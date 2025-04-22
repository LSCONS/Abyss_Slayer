public class PlayerDeadState : PlayerAbnomalState
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
        StartAnimation(playerStateMachine.Player.PlayerAnimationData.DeadParameterHash);
        playerStateMachine.MovementSpeed = 0f;
        ResetZeroVelocity();

#if StateMachineDebug
        Debug.Log("IdleState 진입");
#endif
    }

    public override void Exit()
    {
        base.Exit();
        StopAnimation(playerStateMachine.Player.PlayerAnimationData.IdleParameterHash);
        playerStateMachine.MovementSpeed = playerStateMachine.Player.PlayerData.PlayerGroundData.BaseSpeed;

#if StateMachineDebug
        Debug.Log("IdleState 해제");
#endif
    }

    public override void Update()
    {
        base.Update();
    }
}
