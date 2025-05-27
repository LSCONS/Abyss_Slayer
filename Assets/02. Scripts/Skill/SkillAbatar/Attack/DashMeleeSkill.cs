using System;
using System.Collections;
using UnityEngine;



[CreateAssetMenu(fileName = "NewDashMeleeSkill", menuName = "Skill/MeleeAttack/Dash")]
public class DashMeleeSkill : RemoteZoneRangeSkill
{
    [field: Header("대시 시간")]
    [field: SerializeField] public float DashTime { get; private set; } = 0.5f;
    [field: Header("대시 거리")]
    [field: SerializeField] public float DashDistance { get; private set; } = 5f;

    public override void UseSkill()
    {
        // 대시 이후의 처리들 (데미지 넣을 콜라이더 이동 + )
        MeleeDamageCheckData data = new MeleeDamageCheckData
        (
            player.PlayerRef,
            (int)slotKey,
            ColliderSize,
            ColliderOffset,
            TargetLayer.value,
            0,
            Damage,
            ColliderDuration,
            TickRate,
            (int)EEffectAnimatorController,
            (int)EHitEffectType.Dash,
            (int)EType.BossHitEffect
        );
        PoolManager.Instance.Get<ZoneAOE>().Rpc_DashInit(data, SpawnSize, SpawnOffset, DashDistance, DashTime);
    }
}
