[System.Serializable]
public class SkillSlot
{
    public SkillSlotKey key;           // key들이 포함된 enum
    public SkillData skillData;        // 연결된 실제 스킬 데이터 (ScriptableObject)
}
