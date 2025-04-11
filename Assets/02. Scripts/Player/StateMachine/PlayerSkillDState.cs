using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkillDState : PlayerSkillState
{
    private SkillData SkillData { get; set; }
    private SkillSlotKey Slotkey { get; set; } = SkillSlotKey.D;
    //TODO: PlayerD스킬 데이터 가져와야함.
    public PlayerSkillDState(PlayerStateMachine playerStateMachine) : base(playerStateMachine)
    {
        //skillData = playerStateMachine.Player.equippedSkills[slotkey];
        //playerStateMachine.ConnectAttackState(this);
    }
    public override void Enter()
    {
        base.Enter();
        StartAnimation(playerStateMachine.Player.playerAnimationData.D_SkillParameterHash);
        if (!(SkillData.canMove)) playerStateMachine.MovementSpeed = 0f;
        playerStateMachine.IsCompareState = false;
        playerStateMachine.Player.SkillCoolTimeUpdate(Slotkey);

#if StateMachineDebug
        Debug.Log("SkillDState 진입");
#endif
    }

    public override void Exit()
    {
        base.Exit();
        StopAnimation(playerStateMachine.Player.playerAnimationData.D_SkillParameterHash);
        playerStateMachine.MovementSpeed = playerStateMachine.Player.playerData.PlayerGroundData.BaseSpeed;
        playerStateMachine.Player.SkillTrigger.StopSkillCoroutine();

#if StateMachineDebug
        Debug.Log("SkillDState 해제");
#endif
    }

    public override void Update()
    {
        base.Update();
        playerStateMachine.AnimatorInfo = playerStateMachine.Player.PlayerAnimator.GetCurrentAnimatorStateInfo(0);

        //해당 State의 애니메이터와 연결 완료
        if (!(playerStateMachine.IsCompareState))
        {
            if (playerStateMachine.AnimatorInfo.shortNameHash == playerStateMachine.Player.playerAnimationData.D_SkillAnimationHash)
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
