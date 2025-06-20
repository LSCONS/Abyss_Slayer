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
        SkillData = playerStateMachine.Player.DictSlotKeyToSkill[key];
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
            playerStateMachine.Player.Invincibility = true;
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
            if (playerStateMachine.Player.Runner.IsServer)
                PoolManager.Instance.Get<DashPlayerSilhouette>().Rpc_Init
                (
                    playerStateMachine.Player.PlayerRef,
                    playerStateMachine.Player.PlayerSpriteChange.transform.position,
                    playerStateMachine.Player.IsFlipX
                );
            ResetZeroVelocity();
            ResetZeroGravityForce();
        }

        playerStateMachine.UseSkillData = SkillData;

        if (playerStateMachine.Player.Runner.IsServer)
            playerStateMachine.UseSkill();

        if (SkillData.EAudioClip != EAudioClip.None)
        {
            ManagerHub.Instance.SoundManager.PlaySFX(SkillData.EAudioClip);
        }


#if StateMachineDebug
        Debug.Log("SkillState 진입");
#endif
    }

    public override void Exit()
    {
        base.Exit();
        if (SkillData.SkillCategory == SkillCategory.Dash)
        {
            playerStateMachine.IsDash = false;
            playerStateMachine.Player.Invincibility = false;
            playerStateMachine.Player.PlayerGroundCollider.isTrigger = false;
        }
            
        SkillExit(SkillData);
        if (SkillData.SkillCategory == SkillCategory.Dash || SkillData.SkillCategory == SkillCategory.DashAttack)
        {
            ResetZeroVelocity();
            ResetDefaultGravityForce();
        }

        if (SkillData.EAudioClip != EAudioClip.None)
        {
            if (!(ManagerHub.Instance.DataManager.DictEnumToAudioData.TryGetValue(SkillData.EAudioClip, out var audioData))) return;
            if (audioData.Audio == null || !(audioData.IsLoop)) return;
            ManagerHub.Instance.SoundManager.StopSFX(SkillData.EAudioClip);
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
            CurTime = ChangeSpriteTime + CurTime;

            playerStateMachine.Player.PlayerSpriteChange.SetLoopAnimation(SkillData.SkillUseState, ++animationNum);
        }
        else if (SkillData.SkillCategory == SkillCategory.Charge)
        {

        }
        else if (SkillData.SkillCategory == SkillCategory.Dash || SkillData.SkillCategory == SkillCategory.DashAttack)
        {
            //남는 시간 = Time.time - (ChangeSpriteTime + CurTime)
            //
            if (ChangeSpriteTime + CurTime > Time.time) return;
            CurTime = ChangeSpriteTime + CurTime;

            if (playerStateMachine.Player.PlayerSpriteChange.SetOnceAnimation(SkillData.SkillUseState, ++animationNum))
            {
                if ((SkillData.SkillCategory == SkillCategory.Dash && animationNum % 2 == 0) ||
                    SkillData.SkillCategory == SkillCategory.DashAttack)
                {
                    if (playerStateMachine.Player.Runner.IsServer)
                    {
                        PoolManager.Instance.Get<DashPlayerSilhouette>().Rpc_Init
                    (
                        playerStateMachine.Player.PlayerRef,
                        playerStateMachine.Player.PlayerSpriteChange.transform.position,
                        playerStateMachine.Player.IsFlipX
                    );
                    }
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
            CurTime = ChangeSpriteTime + CurTime;

            if (playerStateMachine.Player.PlayerSpriteChange.SetOnceAnimation(SkillData.SkillUseState, ++animationNum)) return;

            if (playerStateMachine.Player.Runner.IsServer)
                playerStateMachine.EndAttackAction?.Invoke();
        }
    }
}
