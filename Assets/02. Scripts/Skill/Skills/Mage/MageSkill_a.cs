using UnityEngine;

[CreateAssetMenu(menuName = "Skill/Mage/Mage_a")]
public class MageSkill_a : SkillExecuter
{
    public int count;
    public int damage;
    public float speed;
    public float homingPower;
    public float fireDegree;
    public float spreadDegree;

    public override void Execute(Player user, Player target, SkillData skillData)
    {
        var homingProjectile = PoolManager.Instance.Get<HomingProjectile>();
        homingProjectile.Init(damage, user.transform.position, user.transform.rotation, target.transform, speed, homingPower);
    }
}
