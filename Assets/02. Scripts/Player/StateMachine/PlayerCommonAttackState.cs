using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCommonAttackState : PlayerAttackState
{
    //TODO: PlayerX스킬 데이터 가져와야함.
    public PlayerCommonAttackState(PlayerStateMachine playerStateMachine) : base(playerStateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        //TODO: 스킬 사용 중 움직일 수 있는지 확인하고 실행
        //TODO: 스킬 X 애니메이터 활성화
    }

    public override void Exit()
    {
        base.Exit();
        //TODO: 스킬 X 애니메이터 비활성화
        playerStateMachine.MovementSpeed = playerStateMachine.Player.playerData.PlayerGroundData.BaseSpeed;
    }

    public override void Update()
    {
        base.Update();
    }
}
