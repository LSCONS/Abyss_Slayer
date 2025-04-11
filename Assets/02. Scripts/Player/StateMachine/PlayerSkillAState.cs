using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkillAState : PlayerSkillState
{
    private SkillData SkillData { get; set; }
    private SkillSlotKey Slotkey { get; set; } = SkillSlotKey.A;
    public PlayerSkillAState(PlayerStateMachine playerStateMachine) : base(playerStateMachine)
    {
        SkillData = playerStateMachine.Player.equippedSkills[Slotkey];
        playerStateMachine.ConnectSkillState(this, SkillData, () => playerStateMachine.Player.input.IsSkillA);
    }

    public override void Enter()
    {
        base.Enter();
        StartAnimation(playerStateMachine.Player.playerAnimationData.A_SkillParameterHash);
        if (!(SkillData.canMove)) playerStateMachine.MovementSpeed = 0f;
        playerStateMachine.IsCompareState = false;
        playerStateMachine.Player.SkillCoolTimeUpdate(Slotkey);
        SkillData.canUse = false;

#if StateMachineDebug
        Debug.Log("SkillAState 진입");
#endif
    }

    public override void Exit()
    {
        base.Exit();
        StopAnimation(playerStateMachine.Player.playerAnimationData.A_SkillParameterHash);
        playerStateMachine.MovementSpeed = playerStateMachine.Player.playerData.PlayerGroundData.BaseSpeed;
        playerStateMachine.Player.SkillTrigger.StopSkillCoroutine();

#if StateMachineDebug
        Debug.Log("SkillAState 해제");
#endif
    }

    public override void Update()
    {
        base.Update();
        playerStateMachine.AnimatorInfo = playerStateMachine.Player.PlayerAnimator.GetCurrentAnimatorStateInfo(0);

        //해당 State의 애니메이터와 연결 완료
        if (!(playerStateMachine.IsCompareState))
        {
            if (playerStateMachine.AnimatorInfo.shortNameHash == playerStateMachine.Player.playerAnimationData.A_SkillAnimationHash)
            {
                playerStateMachine.IsCompareState = true;
            }
            else
            {
                playerStateMachine.SkipAttackAction?.Invoke();
                return;
            }
        }
        if (playerStateMachine.AnimatorInfo.normalizedTime < 1f) return;
        playerStateMachine.EndAttackAction?.Invoke();
    }
}
