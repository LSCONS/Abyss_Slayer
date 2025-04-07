using UnityEngine;
using UnityEngine.TextCore.Text;

/// <summary>
/// 스킬 실행 로직을 정의하는 추상 클래스.
/// 각 스킬(힐, 데미지, 버프 등)은 이 클래스를 상속하여 Execute()를 구현해야함.
/// </summary>
public abstract class SkillExecuter : ScriptableObject
{
    /// <summary>
    /// 스킬 실행 메서드
    /// </summary>
    /// <param name="user">스킬 시전자</param>
    /// <param name="target">스킬 대상</param>
    /// <param name="skillData">실행되는 스킬 데이터</param>
    public abstract void Execute(Character user, Character target, SkillData skillData);
}
