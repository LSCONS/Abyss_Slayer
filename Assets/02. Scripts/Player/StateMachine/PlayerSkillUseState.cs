using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSkillUseState : PlayerBaseState
{

    private Skill SkillData { get; set; }
    private SkillSlotKey Slotkey { get; set; }
    private Func<bool> SkillInputKey { get; set; }
    private int animationNum = 0;
    private float animationTime = 0;
    private int animationDelay = 10;
    public PlayerSkillUseState(PlayerStateMachine playerStateMachine, SkillSlotKey key) : base(playerStateMachine)
    {
        Slotkey = key;
        SkillData = playerStateMachine.Player.EquippedSkills[key];
        SkillInputKey = playerStateMachine.Player.input.SkillInputKey[key];
        animationDelay = SkillData.AnimationChangeDelayTime;
    }
    public void Init()
    {
        playerStateMachine.ConnectSkillState(this, SkillData, SkillInputKey);
    }

    public override void Enter()
    {
        base.Enter();
        playerStateMachine.Player.PlayerSpriteChange.SetOnceAnimation(SkillData.SkillUseState, 0);
        SkillEnter(SkillData.CanMove, Slotkey);
        animationNum = 0;
        animationTime = animationDelay;

        if(SkillData.SkillCategory == SkillCategory.Dash)
        {
            PoolManager.Instance.Get<DashPlayerSilhouette>().Init
                (
                    playerStateMachine.Player.PlayerSpriteChange,
                    SkillData.SkillUseState,
                    0,
                    playerStateMachine.Player.PlayerSpriteChange.transform.position,
                    playerStateMachine.Player.IsFlipX
                );
            ResetZeroVelocity();
            ResetZeroGravityForce();
        }
        SkillData.UseSkill();


#if StateMachineDebug
        Debug.Log("SkillState 진입");
#endif
    }

    public override void Exit()
    {
        base.Exit();
        SkillExit();
        if (SkillData.SkillCategory == SkillCategory.Dash)
        {
            ResetZeroVelocity();
            ResetDefaultGravityForce();
        }

#if StateMachineDebug
        Debug.Log("SkillAState 해제");
#endif
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        animationTime--;
    }

    public override void Update()
    {
        base.Update();
        if (animationTime <= 0)
        {
            animationTime = animationDelay;

            if(SkillData.SkillCategory == SkillCategory.Hold)
            {
                //TODO: Hold스킬 로직 필요
            }

            if (SkillData.SkillCategory == SkillCategory.Charge)
            {
                //TODO: Charge스킬 로직 필요
            }


            if (playerStateMachine.Player.PlayerSpriteChange.SetOnceAnimation(SkillData.SkillUseState, ++animationNum))
            {
                if(SkillData.SkillCategory == SkillCategory.Dash && animationNum % 2 == 0)
                {
                    PoolManager.Instance.Get<DashPlayerSilhouette>().Init
                    (
                        playerStateMachine.Player.PlayerSpriteChange,
                        SkillData.SkillUseState,
                        animationNum,
                        playerStateMachine.Player.PlayerSpriteChange.transform.position,
                        playerStateMachine.Player.IsFlipX
                    );
                }
                return; 
            }
        }
        else { return; }

        playerStateMachine.SkipAttackAction?.Invoke();
        playerStateMachine.CheckHoldSkillStop(this, SkillInputKey);
        playerStateMachine.EndAttackAction?.Invoke();
    }
}
