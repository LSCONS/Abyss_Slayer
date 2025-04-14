using UnityEngine;

[CreateAssetMenu(menuName = "Skill/Archer/Archer_x")]
public class ArcherSkill_x : SkillExecuter
{
    public int damage;              // 화살 데미지
    public float arrowSpeed;        // 화살 속도
    public int spriteNum;           // 화살 스프라이트 인덱스

    /// <summary>
    /// 아처의 평타 로직을 담당하는 메소드
    /// </summary>
    /// <param name="user">스킬 시전자</param>
    /// <param name="target">타겟팅 정보</param>
    /// <param name="skillData">스킬의 공통 데이터</param>
    public override void Execute(Player user, Player target, SkillData skillData)
    {
        // 플레이어가 보는 방향 계산 (flipX 기준)
        Vector2 dir = user.SpriteRenderer.flipX ? Vector2.left : Vector2.right;

        // 보는 방향으로 1.5만큼 떨어진 위치에 화살 생성
        Vector3 spawnPos = user.transform.position + (Vector3)(dir * 1.5f);

        // 오브젝트 풀에서 화살 가져오기
        var arrow = PoolManager.Instance.Get<ArrowProjectile>();

        // 화살 초기화 데이터 투사체에 전달
        arrow.Init(spawnPos, dir, skillData.targetingData.range, arrowSpeed, spriteNum, damage);

        // 버프 상태일 경우 추가 화살 생성
        if (user.IsDoubleShot)
        {
            var secondArrow = PoolManager.Instance.Get<ArrowProjectile>();
            Vector3 secondSpawnPos = spawnPos + new Vector3(-1.0f, 0.5f, 0);
            secondArrow.Init(secondSpawnPos, dir, skillData.targetingData.range, arrowSpeed, spriteNum, damage * 0.8f);
        }
    }
}
