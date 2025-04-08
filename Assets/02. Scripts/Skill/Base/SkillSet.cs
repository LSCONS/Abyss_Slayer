using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skill/SkillSet")]
public class SkillSet : ScriptableObject
{
    public string className;
    public List<SkillSlot> skillSlots = new();
}
