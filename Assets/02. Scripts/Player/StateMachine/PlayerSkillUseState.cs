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
    public PlayerSkillUseState(PlayerStateMachine playerStateMachine, SkillSlotKey key) : base(playerStateMachine)
    {
        Slotkey = key;
        SkillData = playerStateMachine.Player.EquippedSkills[key];
        SkillInputKey = SlotKeyConvertFunc(key);
        ChangeSpriteTime = SkillData.AnimationChangeDelayTime;
    }
    public void Init()
    {
        playerStateMachine.ConnectSkillState(this, SkillData, SkillInputKey);
    }

    public override void Enter()
    {
        base.Enter();
        if (SkillData.SkillCategory == SkillCategory.Dash)
        {
            playerStateMachine.IsDash = true;
            playerStateMachine.IsTriggerTrue();   
        }

        animationNum = 0;
        playerStateMachine.Player.PlayerSpriteChange.SetOnceAnimation(SkillData.SkillUseState, 0);
        if (!(SkillData.CanMove))
        {
            playerStateMachine.MovementSpeed = 0f;
            ResetZeroVelocity();
        }

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

        playerStateMachine.UseSkillData = SkillData;

        if (playerStateMachine.Player.Runner.IsServer)
            playerStateMachine.Player.Rpc_UseSkill();

        if (SkillData.EAudioClip != EAudioClip.None)
        {
            SoundManager.Instance.PlaySFX(SkillData.EAudioClip);
        }


#if StateMachineDebug
        Debug.Log("SkillState 진입");
#endif
    }

    public override void Exit()
    {
        base.Exit();
        if (SkillData.SkillCategory == SkillCategory.Dash) playerStateMachine.IsDash = false;
        SkillExit(SkillData);
        if (SkillData.SkillCategory == SkillCategory.Dash || SkillData.SkillCategory == SkillCategory.DashAttack)
        {
            ResetZeroVelocity();
            ResetDefaultGravityForce();
        }

        if (SkillData.EAudioClip != EAudioClip.None)
        {
            if (!(DataManager.Instance.DictEnumToAudioData.TryGetValue(SkillData.EAudioClip, out var audioData))) return;
            if (audioData.Audio == null || !(audioData.IsLoop)) return;
            SoundManager.Instance.StopSFX(SkillData.EAudioClip);
        }

#if StateMachineDebug
        Debug.Log("SkillAState 해제");
#endif
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    public override void Update()
    {
        if (SkillData.SkillCategory == SkillCategory.Hold)
        {
            if (!(SkillInputKey()) || playerStateMachine.Player.HoldSkillCoroutine == null)
            {

                if (playerStateMachine.Player.Runner.IsServer)
                    playerStateMachine.ChangeState(playerStateMachine.IdleState);
                return;
            }

            if (ChangeSpriteTime + CurTime > Time.time) return;
            CurTime = Time.time;

            playerStateMachine.Player.PlayerSpriteChange.SetLoopAnimation(SkillData.SkillUseState, ++animationNum);
        }
        else if (SkillData.SkillCategory == SkillCategory.Charge)
        {

        }
        else if (SkillData.SkillCategory == SkillCategory.Dash || SkillData.SkillCategory == SkillCategory.DashAttack)
        {
            if (ChangeSpriteTime + CurTime > Time.time) return;
            CurTime = Time.time;

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

            if (playerStateMachine.Player.Runner.IsServer)
            {
                playerStateMachine.ChangeState(playerStateMachine.FallState);
            }
        }
        else
        {
            if (ChangeSpriteTime + CurTime > Time.time) return;
            CurTime = Time.time;

            if (playerStateMachine.Player.PlayerSpriteChange.SetOnceAnimation(SkillData.SkillUseState, ++animationNum)) return;

            if (playerStateMachine.Player.Runner.IsServer)
                playerStateMachine.EndAttackAction?.Invoke();
        }
    }
}
