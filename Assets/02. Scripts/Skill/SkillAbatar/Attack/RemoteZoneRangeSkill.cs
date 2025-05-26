using UnityEngine;


/// <summary>
/// 플레이어를 기준으로 특정 위치에 특정 BoxCollider2D를 설치해서 데미지를 주는 기능
/// </summary>
[CreateAssetMenu(fileName = "NewRemoteZoneRangeSkill", menuName = "Skill/RangeAttack/RemoteZone")]
public class RemoteZoneRangeSkill : RangeAttackSkill
{
    [field: Header("콜라이더 크기 조정")]
    [field: SerializeField]public virtual Vector2 ColliderSize { get;protected set; } = new Vector2(1, 1);

    [field: Header("오브젝트 크기")]
    [field: SerializeField] public Vector2 SpawnSize { get; private set; } = new Vector2(2, 2);

    [field: Header("콜라이더 위치 값 조정")]
    [field: SerializeField] public virtual Vector2 ColliderOffset { get; protected set; } = new Vector2(1, 1);

    [field: Header("오브젝트 위치 값 조정(플레이어 기준)")]
    [field: SerializeField] public Vector2 SpawnOffset{ get; private set; } = Vector2.zero;

    [field: Header("콜라이더 유지 시간")]
    [field: SerializeField] public float ColliderDuration { get; private set; } = 1f;

    [field: Header("데미지 딜레이 시간")]
    [field: SerializeField] public float TickRate { get; private set; } = 0.2f;

    [field: Header("Effect 이름")]
    [field: SerializeField] public EAnimatorController EEffectAnimatorController { get; private set; } = EAnimatorController.None;

    [field: Header("콜라이더 생성 딜레이 시간")]//TODO: 로직 구현 필요
    [field: SerializeField] public float ColliderSetDelayTime { get; private set; } = 0f;

    [field: Header("콜라이더 유지 동안 다단 히트 적용 여부")]
    [field: SerializeField] public bool CanRepeatHit { get; private set; } = false;

    public override void UseSkill()
    {
        base.UseSkill();
        // 풀에서 ZoneAOE 꺼내기
        MeleeDamageCheckData data = new MeleeDamageCheckData(player.PlayerRef, (int)slotKey, ColliderSize, ColliderOffset, TargetLayer, 0, Damage, ColliderDuration, TickRate, (int)EEffectAnimatorController);
        PoolManager.Instance.Get<ZoneAOE>().Init(data, SpawnSize, SpawnOffset, Vector2.zero);
    }
}
