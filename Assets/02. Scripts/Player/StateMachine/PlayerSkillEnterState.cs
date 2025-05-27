using System;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerSkillEnterState : PlayerBaseState
{
    private Skill SkillData { get; set; }
    private SkillSlotKey Slotkey { get; set; }
    private Func<bool> SkillInputKey { get; set; }
    private int animationNum = 0;
    public PlayerSkillEnterState(PlayerStateMachine playerStateMachine, SkillSlotKey key) : base(playerStateMachine)
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
        playerStateMachine.Player.PlayerSpriteChange.SetOnceAnimation(SkillData.SkillEnterState, 0);
        if (!(SkillData.CanMove))
        {
            playerStateMachine.MovementSpeed = 0f;
            ResetZeroVelocity();
            ResetZeroGravityForce();
        }
        animationNum = 0;
        playerStateMachine.Player.SkillCoolTimeUpdate(Slotkey);

#if StateMachineDebug
        Debug.Log("SkillState 진입");
#endif
    }

    public override void Exit()
    {
        base.Exit();
        SkillExit(SkillData);

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
        if (SkillData.SkillCategory == SkillCategory.Hold && !(SkillInputKey()))
        {
            if (playerStateMachine.Player.Runner.IsServer)
                playerStateMachine.ChangeState(playerStateMachine.IdleState);
            return;
        }

        if (ChangeSpriteTime + CurTime > Time.time) return;
            CurTime = Time.time;

        if (playerStateMachine.Player.PlayerSpriteChange.SetOnceAnimation(SkillData.SkillEnterState, ++animationNum)) return;


        if (playerStateMachine.Player.Runner.IsServer)
            playerStateMachine.ChangeState(playerStateMachine.PlayerSkillUseStateDict[Slotkey]);
    }
}
