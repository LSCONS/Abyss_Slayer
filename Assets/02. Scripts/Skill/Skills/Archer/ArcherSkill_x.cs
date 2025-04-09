using UnityEngine;

[CreateAssetMenu(menuName = "Skill/Archer/Archer_x")]
public class ArcherSkill_x : SkillExecuter
{
    public GameObject arrow;         // 발사할 화살 프리팹
    public float arrowSpeed;         // 화살 속도

    /// <summary>
    /// 아처의 평타 로직을 담당하는 메소드
    /// </summary>
    /// <param name="user">스킬 시전자</param>
    /// <param name="target">타겟팅 정보</param>
    /// <param name="skillData">스킬의 공통 데이터</param>
    public override void Execute(Player user, Player target, SkillData skillData)
    {
        // 플레이어가 바라보는 방향 계산 (flipX 기준)
        Vector2 dir = user.SpriteRenderer.flipX ? Vector2.left : Vector2.right;

        // 바라보는 방향으로 1.5만큼 떨어진 위치에 화살 생성
        Vector3 spawnPos = user.transform.position + (Vector3)(dir * 1.5f);

        // 화살 생성
        var arrows = Instantiate(arrow, spawnPos, Quaternion.identity);

        // 화살에 속도 적용 (지정 방향으로 발사)
        arrows.GetComponent<Rigidbody2D>().velocity = dir * arrowSpeed;

        // Arrow의 SetRange()에 범위 매개변수 전달
        arrows.GetComponent<Arrow>().SetRange(skillData.targetingData.range);
    }
}
