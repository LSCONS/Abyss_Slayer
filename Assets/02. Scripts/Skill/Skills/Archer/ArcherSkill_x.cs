using UnityEngine;
using UnityEngine.TextCore.Text;

[CreateAssetMenu(menuName = "Skill/Archer/Archer_x")]
public class ArcherSkill_x : SkillExecuter
{
    public override void Execute(Character user, Character target, SkillData skillData)
    {
        Debug.LogAssertion("궁수 평타");
    }
}
