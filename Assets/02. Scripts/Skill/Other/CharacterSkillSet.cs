using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCharacterSkillSet", menuName = "Skill/New Skill Set")]
public class CharacterSkillSet : ScriptableObject
{
    public string className; // 어떤 직업의 스킬셋인지
    public List<CharacterSkillSlot> skillSlots = new(); // 연결된 스킬 리스트


    public void InstantiateSkillData(Player player)
    {
        for (int i = 0; i < skillSlots.Count; i++)
        {
            skillSlots[i].skill = Instantiate(skillSlots[i].skill);
            skillSlots[i].skill.player = player;
        }
    }
}
