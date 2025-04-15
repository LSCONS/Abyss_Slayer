using System.Collections;
using UniRx;
using UnityEngine;

[CreateAssetMenu(menuName = "Skill/Archer/Archer_d")]
public class ArcherSkill_d : SkillExecuter
{
    /// <summary>
    /// 아처의 D키 스킬 로직을 담당하는 메소드
    /// </summary>
    /// <param name="user">스킬 시전자</param>
    /// <param name="target">타겟팅 정보</param>
    /// <param name="skillData">스킬의 공통 데이터</param>
    public override void Execute(Player user, Player target, SkillData skillData)
    {
        // 버프 활성화
        user.SetBuff(true);
    }
}
