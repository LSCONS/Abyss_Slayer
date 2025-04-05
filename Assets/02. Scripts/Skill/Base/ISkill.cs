using UnityEngine;
using UnityEngine.TextCore.Text;

/// <summary>
/// UI와 스킬 시스템 사이의 결합도를 낮추기 위한 인터페이스
/// 따로 스킬 시스템을 파악하지 않고 ISkill의 값만 이용하여 UI에 적용 가능
/// (필요없을 시 제거)
/// </summary>
public interface ISkill
{
    string SkillName { get; }
    Sprite Icon { get; }

    void Execute(Character user, Character target, int level);
}
