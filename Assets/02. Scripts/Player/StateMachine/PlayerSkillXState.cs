using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkillXState : PlayerSkillState
{
    private SkillData SkillData { get; set; }
    private SkillSlotKey slotkey { get; set; } = SkillSlotKey.X;
    public PlayerSkillXState(PlayerStateMachine playerStateMachine) : base(playerStateMachine)
    {
        SkillData = playerStateMachine.Player.equippedSkills[slotkey];
        playerStateMachine.ConnectSkillState(this, SkillData,() => playerStateMachine.Player.input.IsSkillX);
    }
    public override void Enter()
    {
        base.Enter();
        StartAnimation(playerStateMachine.Player.playerAnimationData.X_SkillParameterHash);
        if (!(SkillData.canMove)) playerStateMachine.MovementSpeed = 0f;
        playerStateMachine.IsCompareState = false;
        playerStateMachine.Player.SkillCoolTimeUpdate(slotkey);

#if StateMachineDebug
        Debug.Log("SkillXState 진입");
#endif
    }

    public override void Exit()
    {
        base.Exit();
        StopAnimation(playerStateMachine.Player.playerAnimationData.X_SkillParameterHash);
        playerStateMachine.MovementSpeed = playerStateMachine.Player.playerData.PlayerGroundData.BaseSpeed;
        playerStateMachine.Player.SkillTrigger.StopSkillCoroutine();

#if StateMachineDebug
        Debug.Log("SkillXState 해제");
#endif
    }

    public override void Update()
    {
        base.Update();
        playerStateMachine.AnimatorInfo = playerStateMachine.Player.PlayerAnimator.GetCurrentAnimatorStateInfo(0);

        //해당 State의 애니메이터와 연결 완료
        if (!(playerStateMachine.IsCompareState))
        {
            if(playerStateMachine.AnimatorInfo.shortNameHash == playerStateMachine.Player.playerAnimationData.X_SkillAnimationHash)
            {
                playerStateMachine.IsCompareState = true;
            }
            else return;
        }
        if (playerStateMachine.AnimatorInfo.normalizedTime < 1f)
        {
            playerStateMachine.SkipAttackAction?.Invoke();
            return;
        }
        playerStateMachine.EndAttackAction?.Invoke();
    }
}
