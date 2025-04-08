using UnityEngine;

[CreateAssetMenu(menuName = "Skill/Archer/Archer_x")]
public class ArcherSkill_x : SkillExecuter
{
    public GameObject arrow;         // 발사할 화살 프리팹
    public float arrowSpeed;         // 화살 속도

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
    }
}
