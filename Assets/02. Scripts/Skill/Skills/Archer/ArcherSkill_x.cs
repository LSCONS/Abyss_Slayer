using UnityEngine;

[CreateAssetMenu(menuName = "Skill/Archer/Archer_x")]
public class ArcherSkill_x : SkillExecuter
{
    public GameObject arrow;
    public float arrowSpeed = 30f;

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

        var arrows = Instantiate(arrow, user.transform.position + Vector3.up, Quaternion.identity);
        var dir = (target != null)
            ? (target.transform.position - user.transform.position).normalized
            : user.transform.right;

        arrows.GetComponent<Rigidbody2D>().velocity = dir * arrowSpeed;
    }
}
