using Fusion;
using System.Collections;
using UnityEngine;


[CreateAssetMenu(fileName = "NewRepeatRangeSKill", menuName = "Skill/RangeAttack/RepeatRange")]

public class RepeatRangeSkill : RangeAttackSkill
{
    [field: Header("콜라이더 크기")]
    [field: SerializeField] public Vector2 ColliderSize { get; private set; } = new Vector2(1, 1);
    [field: Header("콜라이더 위치 값 조정")]
    [field: SerializeField] public Vector2 ColliderOffset { get; private set; } = new Vector2(1, 1);
    [field: Header("오브젝트 위치 값 조정(플레이어 기준)")]
    [field: SerializeField] public Vector2 SpawnOffset { get; private set; } = Vector2.zero;
    [field: Header("생성 딜레이 시간")]
    [field: SerializeField] public float TickRate { get; private set; } = 0.5f;
    [field: Header("콜라이더 유지 시간")]
    [field: SerializeField] public float Duration { get; private set; } = 1f;
    [field: Header("Effect 이름")]
    [field: SerializeField] public string EffectName { get; private set; } = "이펙트 이름";

    public override void UseSkill()
    {
        base.UseSkill();
        player.SkillTrigger.HoldSkillCoroutine = CoroutineManager.Instance.StartCoroutineEnter(Repeat());
    }

    public IEnumerator Repeat()
    {
        WaitForSeconds wait = new WaitForSeconds(TickRate);

        // 풀에서 ZoneAOE 꺼내기
        for (int i = 0; i < 6; i++)
        {
            Vector2 offset = new Vector2(SpawnOffset.x * PlayerFrontXNormalized(), SpawnOffset.y);
            Vector3 spawnPos = player.transform.position + (Vector3)offset + new Vector3(i * PlayerFrontXNormalized(), -2f, 0);

            var zone = PoolManager.Instance.Get<ZoneAOE>();
            zone.Init(player, this, spawnPos, ColliderSize, ColliderOffset, TickRate, Duration, Damage, TargetLayer, EffectName);

            yield return wait;
        }
    }
}
