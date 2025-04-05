using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

// 개별 스킬에 대한 데이터를 저장하는 ScriptableObject.
[CreateAssetMenu(menuName = "Skill/SkillData")]
public class BaseSkillData : ScriptableObject
{
    [Header("기본 정보")]
    public string skillName;                // 스킬명
    public string description;              // 설명
    public int curlevel = 1;                // 현재 레벨
    public int maxLevel = 3;                // 최대 레벨
    public float cooldown;                  // 쿨타임

    [Header("시각적 요소")]
    public Sprite icon;                     // 스킬 아이콘
    public string animationTrigger;         // Animator에서 호출할 트리거 이름
    public GameObject effectPrefab;         // 이펙트 프리팹

    [Header("스킬 타입")]
    public SkillType skillType;             // 스킬의 실제 로직 담당

    [Header("타겟팅")]
    public TargetingData targetingData;     // 타겟팅 방식

    [Header("레벨별 계수")]
    public List<float> coefficients;        // 레벨별 계수 (ex: 데미지, 회복량 등)

    [Header("레벨별 지속시간")]
    public List<float> duration;            // 레벨별 효과 지속 시간 (ex: 버프, 디버프, 장판)

    // 스킬 실행. SkillType의 Execute() 호출.
    public void Execute(Character user, Character target, int level)
    {
        skillType.Execute(user, target, this, level);
    }
}
