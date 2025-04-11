using UnityEngine;

[CreateAssetMenu(menuName = "Skill/Archer/Archer_z")]
public class ArcherSkill_z : SkillExecuter
{
    public ArrowProjectile arrowPrefab;        // 발사할 화살 프리팹
    public float arrowSpeed;                   // 화살 속도

    public override void Execute(Player user, Player target, SkillData skillData)
    {
    }
}
