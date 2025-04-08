using UnityEngine;

[CreateAssetMenu(menuName = "Skill/Archer/Archer_x")]
public class ArcherSkill_x : SkillExecuter
{
    public GameObject arrow;
    public float arrowSpeed;

    public override void Execute(Player user, Player target, SkillData skillData)
    {
        var spawnPos = user.transform.position + Vector3.right * 1.5f;
        Vector2 dir = user.SpriteRenderer.flipX ? Vector2.left : Vector2.right;

        var arrows = Instantiate(arrow, spawnPos, Quaternion.identity);

        var rb = arrows.GetComponent<Rigidbody2D>();
        rb.velocity = dir * arrowSpeed;
    }
}
