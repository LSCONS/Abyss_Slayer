using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "Skill/Archer/Archer_s")]
public class ArcherSkill_s : SkillExecuter
{
    public GameObject arrow;                // 발사할 화살 프리팹
    public float arrowSpeed;                // 화살 속도
    public int arrowCount;                  // 발사할 화살 수
    public float shotDelay;                 // 화살 발사 간격

    /// <summary>
    /// 아처의 S키 스킬 로직을 담당하는 메소드
    /// </summary>
    /// <param name="user">스킬 시전자</param>
    /// <param name="target">타겟팅 정보</param>
    /// <param name="skillData">스킬의 공통 데이터</param>
    public override void Execute(Player user, Player target, SkillData skillData)
    {
        // 스킬 사용 시 상태 설정
        skillData.canUse = false;
        skillData.canMove = false;
        
        // 코루틴 시작
        user.StartCoroutine(FireArrows(user, target, skillData));
    }

    // 화살 발사 코루틴
    private IEnumerator FireArrows(Player user, Player target, SkillData skillData)
    {
        // GC 최적화를 위해 WaitForSeconds 캐싱
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

            // 화살 생성
            var arrows = Instantiate(arrow, spawnPos, Quaternion.identity);

            // 화살 속도 적용
            arrows.GetComponent<Rigidbody2D>().velocity = dir * arrowSpeed;

            // Arrow의 SetRange()에 범위 변수 전달
            arrows.GetComponent<Arrow>().SetRange(skillData.targetingData.range);

            // 다음 화살 발사까지 대기
            yield return wait;
        }

        // 스킬 사용 후 상태 설정
        skillData.canUse = true;
        skillData.canMove = true;
    }
}
