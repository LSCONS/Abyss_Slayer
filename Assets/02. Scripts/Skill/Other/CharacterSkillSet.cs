using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCharacterSkillSet", menuName = "Skill/New Skill Set")]
public class CharacterSkillSet : ScriptableObject
{
    [field: SerializeField]public CharacterClass Class { get;private set; } // 어떤 직업의 스킬셋인지
    public List<CharacterSkillSlot> skillSlots = new(); // 연결된 스킬 리스트

    public void InstantiateSkillData(Player player)
    {
        for (int i = 0; i < skillSlots.Count; i++)
        {
            //서로 스킬의 데이터가 겹치지 않게 복사해서 가져옴.
            skillSlots[i].skill = Instantiate(skillSlots[i].skill);

            //어떤 플레이어가 사용하고 있는 스킬인지 초기화
            skillSlots[i].skill.player = player;

            // Z,X,A,S,D 중 어디랑 연결되어있는 스킬인지 명시하고 초기화
            skillSlots[i].skill.slotKey = skillSlots[i].key;

            //스킬에 해당하는 KeyAction을 배정함.
            skillSlots[i].skill.KeyAction = InitKeyAction(skillSlots[i].key);
        }
    }

    public keyAction InitKeyAction(SkillSlotKey slotKey)
    {
        keyAction temp = slotKey switch
        {
            SkillSlotKey.X => keyAction.DefaultAttack,
            SkillSlotKey.Z => keyAction.Dash,
            SkillSlotKey.A => keyAction.Skill1,
            SkillSlotKey.S => keyAction.Skill2,
            SkillSlotKey.D => keyAction.Skill3,
            _ => keyAction.None,
        };
        return temp;
    }
}
