using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

[CreateAssetMenu(menuName = "Skill/SkillData")]
public class BaseSkillData : ScriptableObject
{
    public SkillType skillType;
    public Sprite icon;
    public string skillName;
    public string description;
    public string animationTrigger;
    public GameObject effectPrefab;

    public float cooldown;
    public int curlevel = 1;
    public int maxLevel = 3;

    public List<float> coefficients;
    public List<float> duration;

    public void Execute(Character user, Character target, int level)
    {
        skillType.Execute(user, target, this, level);
    }
}
