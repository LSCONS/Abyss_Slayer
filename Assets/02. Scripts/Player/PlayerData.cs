using System;
using UniRx;
using UnityEngine;

[CreateAssetMenu(fileName = "New Player", menuName = "Data/Player")]
public class PlayerData : ScriptableObject
{
    [field: SerializeField]public PlayerGroundData PlayerGroundData { get; private set; }
    [field: SerializeField]public PlayerAirData PlayerAirData { get; private set; }
    [field: SerializeField]public PlayerStatusData PlayerStatusData { get; private set; }
    public void PlayerDataInit(Player player)
    {
        player.playerRigidbody.gravityScale = PlayerStatusData.GravityForce;
    }
}


[Serializable]
public class PlayerAirData
{
    [field: Header("JumpData")]
    [field: SerializeField] public float JumpForce { get; private set; } = 10f;
    [field: SerializeField] public int MaxJumpCount { get; private set; } = 1;
    [field: SerializeField] public int CurJumpCount { get; set; } = 1;
    public void ResetJumpCount() => CurJumpCount = MaxJumpCount;
    public void UseJump() => CurJumpCount--;
    public bool CanJump() => CurJumpCount > 0;

    [field: Header("DashData")]
    [field: SerializeField] public int MaxDashCount { get; private set; } = 1;
    [field: SerializeField] public int CurDashCount { get; set; } = 1;
    public void ResetDashCount() => CurDashCount = MaxDashCount;
}


[Serializable]
public class PlayerGroundData
{
    [field: Header("SpeedData")]
    [field: SerializeField] public float BaseSpeed { get; private set; } = 10f;
}


[Serializable]
public class PlayerStatusData
{
    [field: Header("Data")]
    [field: SerializeField] public float GravityForce { get; private set; } = 5f;

    [field: Header("Status")]
    [field: SerializeField] public int HP_Cur { get; private set; } = 100;
    [field: SerializeField] public int HP_Max { get; private set; } = 100;

    [field: Header("Damage Magnification")]
    [field: SerializeField] public float Damage_Base { get; private set; } = 1;


    [field: Header("Abnormal")]
    [field: SerializeField] public bool CanMove { get; set; } = true;
    [field: Header("Class")]
    [field: SerializeField] public CharacterClass Class { get; private set; }

    [field: Header("플레이어 체력 재생 수치(1이면 1씩 회복)")]
    [field: SerializeField] public int HealingHealth { get; private set; } = 1;

    [field: Header("플레이어 체력 재생 시간 기준(1이면 1초마다)")]
    [field: SerializeField] public float HealingDelay { get; private set; } = 1;
    public float HealingCurTime { get; set; } = 0;
}
