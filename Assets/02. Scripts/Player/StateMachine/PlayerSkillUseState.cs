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
        SkillInputKey = SlotKeyConvertFunc(key);
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
            SoundManager.Instance.PlaySFX(SkillData.SkillSound.clip, SkillData.SkillSound.loop, SkillData.SkillSound.pitch, SkillData.SkillSound.volume);
        }


#if StateMachineDebug
        Debug.Log("SkillState 진입");
#endif
    }

    public override void Exit()
    {
        base.Exit();
        SkillExit(SkillData);
        if (SkillData.SkillCategory == SkillCategory.Dash || SkillData.SkillCategory == SkillCategory.DashAttack)
        {
            ResetZeroVelocity();
            ResetDefaultGravityForce();
        }

        if (SkillData.SkillSound != null && SkillData.SkillSound.loop)
        {
            SoundManager.Instance.StopSFX(SkillData.SkillSound.clip);
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
        if (SkillData.SkillCategory == SkillCategory.Hold)
        {
            if (!(SkillInputKey()) || playerStateMachine.Player.HoldSkillCoroutine == null)
            {

                if (playerStateMachine.Player.IsThisRunner)
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

            if (playerStateMachine.Player.IsThisRunner)
                playerStateMachine.EndAttackAction?.Invoke();
        }
        else
        {
            if (animationTime > 0) return;

            animationTime = animationDelay;
            if (playerStateMachine.Player.PlayerSpriteChange.SetOnceAnimation(SkillData.SkillUseState, ++animationNum)) return;

            if (playerStateMachine.Player.IsThisRunner)
                playerStateMachine.EndAttackAction?.Invoke();
        }
    }
}
