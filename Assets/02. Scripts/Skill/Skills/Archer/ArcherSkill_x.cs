using UnityEngine;

[CreateAssetMenu(menuName = "Skill/Archer/Archer_x")]
public class ArcherSkill_x : SkillExecuter
{
    public GameObject arrow;
    public float arrowSpeed;

    public override void Execute(Player user, Player target, SkillData skillData)
    {
        var targeting = skillData.targetingData;

        // 논타겟팅 스킬
        if (!targeting.requiresTarget)
        {
            foreach (var hit in Physics2D.OverlapCircleAll(user.transform.position, targeting.areaRadius))
            {
                var p = hit.GetComponent<Player>();
                if (p && targeting.IsValidTarget(user, p))
                {
                    // p.TakeDamage(50);
                }
            }
        }

        var spawnPos = user.transform.position + Vector3.right * 1.5f;
        var arrows = Instantiate(arrow, spawnPos, Quaternion.identity);
        Vector2 dir = user.SpriteRenderer.flipX ? Vector2.left : Vector2.right;

        var rb = arrows.GetComponent<Rigidbody2D>();
        rb.velocity = dir * arrowSpeed;
    }
}
