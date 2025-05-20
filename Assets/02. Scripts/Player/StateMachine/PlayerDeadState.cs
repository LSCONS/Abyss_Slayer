using UnityEngine;

public class PlayerDeadState : PlayerAbnomalState
{
    private int animationNum = 0;
    public PlayerDeadState(PlayerStateMachine playerStateMachine) : base(playerStateMachine)
    {
    }

    public void Init()
    {

    }

    public override void Enter()
    {
        base.Enter();
        playerStateMachine.Player.PlayerSpriteChange.SetLoopAnimation(AnimationState.Fall, 0);
        playerStateMachine.MovementSpeed = 0f;
        ResetZeroVelocity();

#if StateMachineDebug
        Debug.Log("IdleState 진입");
#endif
    }

    public override void Exit()
    {
        base.Exit();
        playerStateMachine.MovementSpeed = playerStateMachine.Player.PlayerData.PlayerGroundData.BaseSpeed;

#if StateMachineDebug
        Debug.Log("IdleState 해제");
#endif
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    public override void Update()
    {
        if (ChangeSpriteTime + CurTime < Time.time)
        {
            CurTime = Time.time;
            playerStateMachine.Player.PlayerSpriteChange.SetOnceAnimation(AnimationState.Fall, ++animationNum);
        }
    }
}
