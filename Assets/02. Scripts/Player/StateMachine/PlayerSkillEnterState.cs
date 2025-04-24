using System;
using UnityEngine;

public class PlayerSkillEnterState : PlayerBaseState
{
    private Skill SkillData { get; set; }
    private SkillSlotKey Slotkey { get; set; }
    private Func<bool> SkillInputKey { get; set; }
    private int animationNum = 0;
    private float animationTime = 0;
    private int animationDelay = 10;
    public PlayerSkillEnterState(PlayerStateMachine playerStateMachine, SkillSlotKey key) : base(playerStateMachine)
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
        playerStateMachine.Player.PlayerSpriteChange.SetOnceAnimation(SkillData.SkillEnterState, 0);
        SkillEnter(SkillData.CanMove, Slotkey);
        animationNum = 0;
        animationTime = animationDelay;

#if StateMachineDebug
        Debug.Log("SkillState 진입");
#endif
    }

    public override void Exit()
    {
        base.Exit();
        SkillExit();

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
            if (playerStateMachine.Player.PlayerSpriteChange.SetOnceAnimation(SkillData.SkillEnterState, ++animationNum)) return;
        }
        else return;
        playerStateMachine.ChangeState(playerStateMachine.PlayerSkillUseStateDict[Slotkey]);
    }
}
