using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

// 개별 스킬에 대한 데이터를 저장하는 ScriptableObject.
[CreateAssetMenu(menuName = "Skill/SkillData")]
public class SkillData : ScriptableObject
{
    [Header("기본 정보")]
    public string skillName;                // 스킬명
    public string description;              // 설명

    [Header("시각적 요소")]
    public Sprite icon;                     // 스킬 아이콘
    public string animationTrigger;         // Animator에서 호출할 트리거 이름
    public GameObject effectPrefab;         // 이펙트 프리팹

    [Header("스킬")]
    public SkillCategory category;
    public SkillExecuter executer;

    [Header("타겟팅")]
    public TargetingData targetingData;     // 타겟팅 방식

    public void Execute(Character user, Character target)
    {
        executer?.Execute(user, target, this);
    }
}
