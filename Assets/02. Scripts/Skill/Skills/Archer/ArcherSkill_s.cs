using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "Skill/Archer/Archer_s")]
public class ArcherSkill_s : SkillExecuter
{
    [SerializeField] private int damage;              // 화살 데미지
    [SerializeField] private float arrowSpeed;        // 화살 속도
    [SerializeField] private int arrowCount;          // 발사할 화살 수
    [SerializeField] private int spriteNum;           // 화살 스프라이트 인덱스
    [SerializeField] private float shotDelay;         // 화살 발사 간격

    /// <summary>
    /// 아처의 S키 스킬 로직을 담당하는 메소드
    /// </summary>
    /// <param name="user">스킬 시전자</param>
    /// <param name="target">타겟팅 정보</param>
    /// <param name="skillData">스킬의 공통 데이터</param>
    public override void Execute(Player user, Boss target, SkillData skillData)
    {
        // 호출용
    }

    // 화살 발사 코루틴
    public IEnumerator FireArrows(Player user, Player target, SkillData skillData)
    {
        // GC 최적화를 위한 WaitForSeconds 캐싱
        WaitForSeconds wait = new WaitForSeconds(shotDelay);

        for (int i = 0; i < arrowCount; i++)
        {
            // 플레이어가 바라보는 방향 계산
            Vector2 dir = user.SpriteRenderer.flipX ? Vector2.left : Vector2.right;
            
            // 일정 범위 내에서 화살 랜덤 생성
            float randomXSpawn;

            if (!user.SpriteRenderer.flipX)
            {
                randomXSpawn = Random.Range(-0.3f, 1.2f);
            }
            else
            {
                randomXSpawn = Random.Range(0.3f, -1.2f);
            }

            float randomYSpawn = Random.Range(-0.3f, 0.5f);

            // 화살 생성 위치 설정
            Vector3 spawnPos = user.transform.position + (Vector3)(dir * 2f) + new Vector3(randomXSpawn, randomYSpawn, 0);
            

            // 버프 상태일 경우 추가 화살 생성
            if (user.IsBuff)
            {
                Vector3 secondSpawnPos = user.SpriteRenderer.flipX ? spawnPos + new Vector3(-1.0f, 0.5f, 0) : spawnPos + new Vector3(1.0f, 0.5f, 0);
                PoolManager.Instance.Get<ArcherProjectile>().Init(secondSpawnPos, dir, skillData.targetingData.range, arrowSpeed, spriteNum, damage * 0.8f);
            }

            // 화살 초기화 데이터 투사체에 전달
            PoolManager.Instance.Get<ArcherProjectile>().Init(spawnPos, dir, skillData.targetingData.range, arrowSpeed, spriteNum, damage);

            // 다음 화살 발사까지 대기
            yield return wait;
        }
    }
}
