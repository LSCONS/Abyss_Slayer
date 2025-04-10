using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCommonAttackState : PlayerSkillState
{
    private SkillData skillData;
    private SkillSlotKey slotkey = SkillSlotKey.X;
    //TODO: 임시 코드 추가. 나중에 삭제 필요
    private float EnterUpdateTime = 0f;
    private float ChangeStateDelayTime = 0.5f;
    public PlayerCommonAttackState(PlayerStateMachine playerStateMachine) : base(playerStateMachine)
    {
        skillData = playerStateMachine.Player.equippedSkills[slotkey];
        playerStateMachine.ConnectSkillState(this, skillData,() => playerStateMachine.Player.input.IsAttack);
    }
    public override void Enter()
    {
        base.Enter();
        if (!(skillData.canMove)) playerStateMachine.MovementSpeed = 0f;
        StartAnimation(playerStateMachine.Player.playerAnimationData.commonAttackParameterHash);
        skillData.Execute(playerStateMachine.Player, null);
        playerStateMachine.Player.SkillCoolTimeUpdate(slotkey);
        EnterUpdateTime = 0f;

#if StateMachineDebug
        Debug.Log("SkillXState 진입");
#endif
    }

    public override void Exit()
    {
        base.Exit();
        StopAnimation(playerStateMachine.Player.playerAnimationData.commonAttackParameterHash);
        playerStateMachine.MovementSpeed = playerStateMachine.Player.playerData.PlayerGroundData.BaseSpeed;

#if StateMachineDebug
        Debug.Log("SkillXState 해제");
#endif
    }

    public override void Update()
    {
        base.Update();
        EnterUpdateTime += Time.deltaTime;
        if(EnterUpdateTime <= ChangeStateDelayTime)
        {
            playerStateMachine.SkipAttackAction?.Invoke();
            return;
        }

        //TODO: 임시 코드
        playerStateMachine.ChangeState(playerStateMachine.IdleState);
    }
}
