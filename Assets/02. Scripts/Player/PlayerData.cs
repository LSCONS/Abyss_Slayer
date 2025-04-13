using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Player", menuName = "Characters/Player")]
public class PlayerData : ScriptableObject
{
    [field: SerializeField]public PlayerGroundData PlayerGroundData { get; private set; }
    [field: SerializeField]public PlayerAirData PlayerAirData { get; private set; }
    [field: SerializeField]public PlayerStatusData PlayerStatusData { get; private set; }
}


[Serializable]
public class PlayerAirData
{
    [field: Header("JumpData")]
    [field: SerializeField] public float WalkSpeedModifier { get; private set; } = 0.225f;
    [field: SerializeField] public float JumpForce { get; private set; } = 10f;
    [field: SerializeField] public int MaxJumpCount { get; private set; } = 1;
    [field: SerializeField] public int CurJumpCount { get; set; } = 1;
    public void ResetJumpCount() => CurJumpCount = MaxJumpCount;

    [field: Header("DashData")]
    [field: SerializeField] public float DashForce { get; private set; } = 5f;
    [field: SerializeField] public float DashCoolTime { get; private set; } = 1f;
    [field: SerializeField] public float DashChangeStateDelayTime { get; private set; } = 0.5f;
    [field: SerializeField] public bool CanDash {  get; set; } = true;
    [field: SerializeField] public int MaxDashCount { get; private set; } = 1;
    [field: SerializeField] public int CurDashCount { get; set; } = 1;
    public void ResetDashCount() => CurDashCount = MaxDashCount;
}


[Serializable]
public class PlayerGroundData
{
    [field: Header("IdleData")]
    [field: SerializeField] public float BaseSpeed { get; private set; } = 10f;
    [field: SerializeField] public float BaseRotationDamping { get; private set; } = 1f;

    [field: Header("WalkData")]
    [field: SerializeField] public float WalkSpeedModifier { get; private set; } = 0.225f;

    [field: Header("RunData")]
    [field: SerializeField] public float RunSpeedModifier { get; private set; } = 1f;
}


[Serializable]
public class PlayerStatusData
{
    [field: Header("Data")]
    [field: SerializeField] public float GravityForce { get; private set; } = 2f;

    [field: Header("Status")]
    [field: SerializeField] public float HP_Max { get; set; } = 100;
    [field: SerializeField] public float HP_Cur { get; set; } = 100;
    [field: SerializeField] public float MP_Max { get; set; } = 100;
    [field: SerializeField] public float MP_Cur { get; set; } = 100;

    [field: Header("Abnormal")]
    [field: SerializeField] public bool CanMove { get; set; } = true;
}
