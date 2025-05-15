using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public static class BossAnimationHash
{
    public static int DeadParameterHash           {get; private set;} = Animator.StringToHash("Dead");
    public static int TeleportInParameterHash     {get; private set;} = Animator.StringToHash("TeleportIn");
    public static int TeleportOutParameterHash    {get; private set;} = Animator.StringToHash("TeleportOut");
    public static int CloneExplosionParameterHash {get; private set;} = Animator.StringToHash("CloneExplosion");
    public static int StunParameterHash           {get; private set;} = Animator.StringToHash("Stun");
    public static int IdleParameterHash           {get; private set;} = Animator.StringToHash("Idle");
    public static int Dash1ParameterHash          {get; private set;} = Animator.StringToHash("Dash1");
    public static int Dash2ParameterHash          {get; private set;} = Animator.StringToHash("Dash2");
    public static int Dash3ParameterHash          {get; private set;} = Animator.StringToHash("Dash3");
    public static int Attack3ParameterHash        {get; private set;} = Animator.StringToHash("Attack3");
    public static int DamagedParameterHash        {get; private set;} = Animator.StringToHash("Damaged");
    public static int JumpParameterHash           {get; private set;} = Animator.StringToHash("Jump");
    public static int FallParameterHash           {get; private set;} = Animator.StringToHash("Fall");
    public static int LandParameterHash           {get; private set;} = Animator.StringToHash("Land");
    public static int Attack2ParameterHash        {get; private set;} = Animator.StringToHash("Attack2");
    public static int AppearParameterHash         {get; private set;} = Animator.StringToHash("Appear");
}
