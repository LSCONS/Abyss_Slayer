using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "ArcherSkillS", menuName = "SkillRefactory/Range/RapidArrow")]
public class RapidArrowSkill : ProjectileAttackSkill
{
    [field: SerializeField] public int ProjectileCount { get; private set; } = 50;
    [field: SerializeField] public float ShotDelay { get; private set; } = 0.1f;
    private Vector3 distanceY = new Vector3(0, 0.25f, 0);

    public override void UseSkill()
    {
        base.UseSkill();
        player.SkillTrigger.HoldSkillCoroutine = CoroutineManager.Instance.StartCoroutineEnter(FireArrows());
    }

    // 화살 발사 코루틴
    public IEnumerator FireArrows()
    {
        // GC 최적화를 위한 WaitForSeconds 캐싱
        WaitForSeconds wait = new WaitForSeconds(ShotDelay);

        for (int i = 0; i < ProjectileCount; i++)
        {
            // 플레이어가 바라보는 방향 계산
            Vector2 dir = new Vector2(PlayerFrontXNomalized(), 0);

            // 일정 범위 내에서 화살 랜덤 생성
            float randomYSpawn = Random.Range(-0.3f, 0.3f);

            // 화살 생성 위치 설정
            Vector3 spawnPos = player.transform.position + (Vector3)(dir * 1.5f) + new Vector3(0, randomYSpawn, 0);

            // 버프 상태일 경우 추가 화살 생성
            if (player.BuffDuration.ContainsKey(BuffType.ArcherDoubleShot) && player.BuffDuration[BuffType.ArcherDoubleShot].IsApply)
            {
                float x = Random.Range(-1f, 1f);
                Vector3 distanceX = new Vector3(x, 0, 0);
                PoolManager.Instance.Get<ArcherProjectile>().Init(spawnPos + distanceY + distanceX, dir, Range, Speed, SpriteNum, Damage * 0.8f);
                
                x = Random.Range(-1f, 1f);
                distanceX = new Vector3(x, 0, 0);
                PoolManager.Instance.Get<ArcherProjectile>().Init(spawnPos - distanceY + distanceX, dir, Range, Speed, SpriteNum, Damage * 0.8f);
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
