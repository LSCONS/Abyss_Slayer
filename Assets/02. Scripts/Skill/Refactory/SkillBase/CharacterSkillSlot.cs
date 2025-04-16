using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CharacterSkillSlot
{
    public SkillSlotKey key;           // key들이 포함된 enum
    public Skill skill;        // 연결된 실제 스킬 데이터 (ScriptableObject)
}
