using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skill/SkillSet")]
public class SkillSet : ScriptableObject
{
    public string className;                        // 어떤 직업의 스킬셋인지
    public List<SkillSlot> skillSlots = new();      // 연결된 스킬 리스트
    public EvasionType evasionType;			        // 회피 종류 (대쉬, 텔포)
}
