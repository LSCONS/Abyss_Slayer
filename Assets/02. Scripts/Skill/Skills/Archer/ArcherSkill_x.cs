using UnityEngine;

[CreateAssetMenu(menuName = "Skill/Archer/Archer_x")]
public class ArcherSkill_x : SkillExecuter
{
    public GameObject arrow;
    public float arrowSpeed;

    public override void Execute(Player user, Player target, SkillData skillData)
    {
        Vector2 dir = user.SpriteRenderer.flipX ? Vector2.left : Vector2.right;
        Vector3 spawnPos = user.transform.position + (Vector3)(dir * 1.5f);

        var arrows = Instantiate(arrow, spawnPos, Quaternion.identity);
        arrows.GetComponent<Rigidbody2D>().velocity = dir * arrowSpeed;
    }
}
