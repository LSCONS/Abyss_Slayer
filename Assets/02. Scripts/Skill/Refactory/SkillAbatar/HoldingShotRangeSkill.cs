using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RangeHoldingShotSkill", menuName = "SkillRefactory/Range/HoldingShot")]
public class HoldingShotRangeSkill : RangeAttackSkill
{
    [field: SerializeField] public int ProjectileCount { get; private set; } = 50;
    [field: SerializeField] public float ShotDelay { get; private set; } = 0.1f;

    public override void UseSkill()
    {
        base.UseSkill();
        CoroutineManager.Instance.StartCoroutineManager(player.SkillTrigger.HoldSkillCoroutine, FireArrows());
    }

    // 화살 발사 코루틴
    public IEnumerator FireArrows()
    {
        // GC 최적화를 위한 WaitForSeconds 캐싱
        WaitForSeconds wait = new WaitForSeconds(ShotDelay);

        for (int i = 0; i < ProjectileCount; i++)
        {
            // float y = Random.Range(-0.1f, 0.1f);
            // 플레이어가 바라보는 방향 계산
            Vector2 dir = new Vector2(PlayerFrontXNomalized(), 0);
            Vector3 distance = new Vector3(0.2f, 0.25f, 0);

            // 일정 범위 내에서 화살 랜덤 생성
            float x = Random.Range(0.45f, 0.47f);
            float randomXSpawn = player.SpriteRenderer.flipX ? -x : x;
            float randomYSpawn = Random.Range(-0.4f, 0.4f);

            // 화살 생성 위치 설정
            Vector3 spawnPos = player.transform.position + (Vector3)(dir * 1.0f) + new Vector3(randomXSpawn, randomYSpawn, 0);;

            // 버프 상태일 경우 추가 화살 생성
            if (player.BuffDuration.ContainsKey(BuffType.ArcherDoubleShot) && player.BuffDuration[BuffType.ArcherDoubleShot].IsApply)
            {
                PoolManager.Instance.Get<ArcherProjectile>().Init(spawnPos + distance, dir, Range, Speed, SpriteNum, Damage * 0.8f);
                PoolManager.Instance.Get<ArcherProjectile>().Init(spawnPos - distance, dir, Range, Speed, SpriteNum, Damage * 0.8f);
            }
            else
            {
                PoolManager.Instance.Get<ArcherProjectile>().Init(spawnPos, dir, Range, Speed, SpriteNum, Damage);
            }

            // 다음 화살 발사까지 대기
            yield return wait;
        }
    }
}
