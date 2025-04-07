using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skill/SkillSet")]
public class SkillSet : ScriptableObject
{
    public string characterClassName;
    public List<SkillSlot> skillSlots;
}
