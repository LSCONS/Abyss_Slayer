using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public static class AnimationHash
{
    public static int DeadParameterHash           { get; private set; } = Animator.StringToHash("Dead");
    public static int TeleportInParameterHash     { get; private set; } = Animator.StringToHash("TeleportIn");
    public static int TeleportOutParameterHash    { get; private set; } = Animator.StringToHash("TeleportOut");
    public static int CloneExplosionParameterHash { get; private set; } = Animator.StringToHash("CloneExplosion");
    public static int StunParameterHash           { get; private set; } = Animator.StringToHash("Stun");
    public static int IdleParameterHash           { get; private set; } = Animator.StringToHash("Idle");
    public static int Dash1ParameterHash          { get; private set; } = Animator.StringToHash("Dash1");
    public static int Dash2ParameterHash          { get; private set; } = Animator.StringToHash("Dash2");
    public static int Dash3ParameterHash          { get; private set; } = Animator.StringToHash("Dash3");
    public static int Attack3ParameterHash        { get; private set; } = Animator.StringToHash("Attack3");
    public static int DamagedParameterHash        { get; private set; } = Animator.StringToHash("Damaged");
    public static int JumpParameterHash           { get; private set; } = Animator.StringToHash("Jump");
    public static int FallParameterHash           { get; private set; } = Animator.StringToHash("Fall");
    public static int LandParameterHash           { get; private set; } = Animator.StringToHash("Land");
    public static int Attack2ParameterHash        { get; private set; } = Animator.StringToHash("Attack2");
    public static int AppearParameterHash         { get; private set; } = Animator.StringToHash("Appear");
    public static int ParameterHash               { get; private set; } = Animator.StringToHash("Run");
    public static int AttackJumpParameterHash     { get; private set; } = Animator.StringToHash("AttackJump");
    public static int RunSlashParameterHash       { get; private set; } = Animator.StringToHash("RunSlash");
    public static int SlashEndParameterHash       { get; private set; } = Animator.StringToHash("SlashEnd");
    public static int SpawnParameterHash          { get; private set; } = Animator.StringToHash("Spawn");
    public static int ThrowParameterHash          { get; private set; } = Animator.StringToHash("Throw");
    public static int SlashReadyParameterHash     { get; private set; } = Animator.StringToHash("SlashReady");
    public static int ReadyRunParameterHash       { get; private set; } = Animator.StringToHash("ReadyRun");
    public static int CreationSpeedParameterHash  { get; private set; } = Animator.StringToHash("CreationSpeed");
    public static int ColorParameterHash          { get; private set; } = Animator.StringToHash("Color");
    public static int EndParameterHash            { get; private set; } = Animator.StringToHash("End");
    public static int SpeedParameterHash          { get; private set; } = Animator.StringToHash("Speed");
    public static int CrossSlash1ParameterHash    { get; private set; } = Animator.StringToHash("CrossSlash1");
    public static int CrossSlash2ParameterHash    { get; private set; } = Animator.StringToHash("CrossSlash2");
    public static int ExplosionParameterHash { get; private set; } = Animator.StringToHash("Explosion");
    public static int WarningTimeParameterHash { get; private set; } = Animator.StringToHash("WarningTime"); 
    public static int EndChasingParameterHash { get; private set; } = Animator.StringToHash("EndChasing");
}
