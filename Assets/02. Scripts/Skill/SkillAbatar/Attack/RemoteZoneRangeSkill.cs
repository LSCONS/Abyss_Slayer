using UnityEngine;


[CreateAssetMenu(fileName = "NewRemoteZoneRangeSkill", menuName = "Skill/RangeAttack/RemoteZone")]

public class RemoteZoneRangeSkill : RangeAttackSkill
{
    [field: Header("콜라이더 크기")]
    [field: SerializeField] public Vector2 ColliderSize { get; private set; } = new Vector2(1, 1);
    [field: Header("콜라이더 위치(플레이어 기준)")]
    [field: SerializeField] public Vector2 SpawnOffset{ get; private set; } = Vector2.zero;
    [field: Header("데미지 딜레이 시간")]
    [field: SerializeField] public float TickRate { get; private set; } = 0.2f;
    [field: Header("콜라이더 유지 시간")]
    [field: SerializeField] public float Duration { get; private set; } = 1f;
    [field: Header("Effect 이름")]
    [field: SerializeField] public string EffectName { get; private set; } = "이펙트 이름";

    public override void UseSkill()
    {
        Vector2 offset = new Vector2(SpawnOffset.x * PlayerFrontXNormalized(), SpawnOffset.y);
        Vector3 spawnPos = player.transform.position + (Vector3)offset;


        // 풀에서 ZoneAOE 꺼내기
        var zone = PoolManager.Instance.Get<ZoneAOE>();
        zone.Init(player, this, spawnPos, ColliderSize, TickRate, Duration, Damage, TargetLayer, EffectName);
    }
}
