using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public abstract class SkillType : ScriptableObject
{
    public abstract void Execute(Character user, Character target, BaseSkillData skillData, int level);
}
