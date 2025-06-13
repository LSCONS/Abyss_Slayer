using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New Player", menuName = "Data/Player")]
public class CharacterData : ScriptableObject
{
    [field: SerializeField]public PlayerGroundData PlayerGroundData { get; private set; }
    [field: SerializeField]public PlayerAirData PlayerAirData { get; private set; }
    [field: SerializeField]public PlayerStatusData PlayerStatusData { get; private set; }
    public void PlayerDataInit(Player player)
    {
        player.PlayerRigidbody.gravityScale = PlayerStatusData.GravityForce;
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

    [field: Header("플레이어 가하는 피해량 배수")]
    [field: SerializeField] public float Damage_Base { get; private set; } = 1;


    //플레이어의 상태이상 여부
    public bool CanMove { get; set; } = true;
    [field: Header("해당 직업의 클래스 설정")]
    [field: SerializeField] public CharacterClass Class { get; private set; }

    [field: Header("플레이어 체력 재생 수치(1이면 최대 체력의 100%씩 회복)")]
    [field: SerializeField] public float HealingHealth { get; private set; } = 0.1f;

    [field: Header("플레이어 체력 재생 시간 기준(1이면 1초마다)")]
    [field: SerializeField] public float HealingDelay { get; private set; } = 1;

    [field: Header("직업 이미지")]
    [field: SerializeField] public Sprite ClassImageSprite { get; private set; } = null;
    public float HealingCurTime { get; set; } = 0;
    //플레이어의 사망 여부
    public bool IsDead { get; set; } = false;
    //플레이어 난이도에 따른 피격 데미지 적용 배수
    public float PlayerOnDamageLevelMultiple => ManagerHub.Instance.GameValueManager.GetPlayerMultiypleOnDamageValue();
}
