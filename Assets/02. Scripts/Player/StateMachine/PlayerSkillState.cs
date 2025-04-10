using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkillState : PlayerBaseState
{
    public PlayerSkillState(PlayerStateMachine playerStateMachine) : base(playerStateMachine)
    {
        
    }

    public override void Enter()
    {
        base.Enter();
        StartAnimation(playerStateMachine.Player.playerAnimationData.attackParameterHash);
    }

    public override void Exit() 
    {
        base.Exit();
        StopAnimation(playerStateMachine.Player.playerAnimationData.attackParameterHash);
    }
    public override void Update()
    {
        base.Update();
    }
}
