using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
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
        if (!(SkillData.CanMove))
        {
            playerStateMachine.MovementSpeed = 0f;
            ResetZeroVelocity();
        }
        animationNum = 0;
        animationTime = animationDelay;

        if (SkillData.SkillCategory == SkillCategory.Dash || SkillData.SkillCategory == SkillCategory.DashAttack)
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

        if (SkillData.SkillSound != null)
        {
            if (SkillData.SkillCategory.Equals(SkillCategory.Hold))
            {
                SoundManager.Instance.PlaySFX(SkillData.SkillSound, true);
            }
            else
            {
                SoundManager.Instance.PlaySFX(SkillData.SkillSound, false);
            }
        }


#if StateMachineDebug
        Debug.Log("SkillState 진입");
#endif
    }

    public override void Exit()
    {
        base.Exit();
        SkillExit();
        if (SkillData.SkillCategory == SkillCategory.Dash || SkillData.SkillCategory == SkillCategory.DashAttack)
        {
            ResetZeroVelocity();
            ResetDefaultGravityForce();
        }

        if (SkillData.SkillSound != null)
        {
            SoundManager.Instance.StopSFX(SkillData.SkillSound);
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
        if (SkillData.SkillCategory == SkillCategory.Hold)
        {
            if (!(SkillInputKey()) || playerStateMachine.Player.HoldSkillCoroutine == null)
            {
                playerStateMachine.ChangeState(playerStateMachine.IdleState);
                return;
            }

            if (animationTime > 0) return;

            animationTime = animationDelay;
            playerStateMachine.Player.PlayerSpriteChange.SetLoopAnimation(SkillData.SkillUseState, ++animationNum);
        }
        else if (SkillData.SkillCategory == SkillCategory.Charge)
        {

        }
        else if (SkillData.SkillCategory == SkillCategory.Dash || SkillData.SkillCategory == SkillCategory.DashAttack)
        {
            if (animationTime > 0) return;

            animationTime = animationDelay;
            if (playerStateMachine.Player.PlayerSpriteChange.SetOnceAnimation(SkillData.SkillUseState, ++animationNum))
            {
                if ((SkillData.SkillCategory == SkillCategory.Dash && animationNum % 2 == 0) ||
                    SkillData.SkillCategory == SkillCategory.DashAttack)
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
            playerStateMachine.EndAttackAction?.Invoke();
        }
        else
        {
            if (animationTime > 0) return;

            animationTime = animationDelay;
            if (playerStateMachine.Player.PlayerSpriteChange.SetOnceAnimation(SkillData.SkillUseState, ++animationNum)) return;

            playerStateMachine.EndAttackAction?.Invoke();
        }
    }
}
