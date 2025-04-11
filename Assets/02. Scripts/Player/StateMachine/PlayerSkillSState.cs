using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkillSState : PlayerSkillState
{
    private SkillData SkillData { get; set; }
    private SkillSlotKey Slotkey { get; set; } = SkillSlotKey.S;
    public PlayerSkillSState(PlayerStateMachine playerStateMachine) : base(playerStateMachine)
    {
        SkillData = playerStateMachine.Player.equippedSkills[Slotkey];
        playerStateMachine.ConnectSkillState(this, SkillData, () => playerStateMachine.Player.input.IsSkillS);
    }

    public override void Enter()
    {
        base.Enter();
        StartAnimation(playerStateMachine.Player.playerAnimationData.S_SkillParameterHash);
        if (!(SkillData.canMove))
        {
            playerStateMachine.MovementSpeed = 0f;
            ResetZeroVelocity();
        }
        playerStateMachine.IsCompareState = false;
        playerStateMachine.Player.SkillCoolTimeUpdate(Slotkey);
        SkillData.canUse = false;

#if StateMachineDebug
        Debug.Log("SkillSState 진입");
#endif
    }

    public override void Exit()
    {
        base.Exit();
        StopAnimation(playerStateMachine.Player.playerAnimationData.S_SkillParameterHash);
        playerStateMachine.MovementSpeed = playerStateMachine.Player.playerData.PlayerGroundData.BaseSpeed;
        playerStateMachine.Player.SkillTrigger.StopSkillCoroutine();

#if StateMachineDebug
        Debug.Log("SkillSState 해제");
#endif
    }

    public override void Update()
    {
        base.Update();
        playerStateMachine.AnimatorInfo = playerStateMachine.Player.PlayerAnimator.GetCurrentAnimatorStateInfo(0);

        //해당 State의 애니메이터와 연결 완료
        if (!(playerStateMachine.IsCompareState))
        {
            if (playerStateMachine.AnimatorInfo.shortNameHash == playerStateMachine.Player.playerAnimationData.S_SkillAnimationHash)
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
        if (playerStateMachine.AnimatorInfo.normalizedTime < 1f) return;
        playerStateMachine.EndAttackAction?.Invoke();
    }
}
