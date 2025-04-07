using UnityEngine;
using UnityEngine.TextCore.Text;

// 개별 스킬에 대한 데이터를 저장하는 ScriptableObject.
[CreateAssetMenu(menuName = "Skill/Base/SkillData")]
public class SkillData : ScriptableObject
{
    [Header("기본 정보")]
    public string skillName;                // 스킬명
    public string description;              // 설명
    public int manaCost;                    // 마나 소모량
    public float cooldown;                  // 쿨타임
    public bool canMove;

    [Header("시각적 요소")]
    public Sprite icon;                     // 스킬 아이콘
    public string animationTrigger;         // Animator에서 호출할 트리거 이름
    public GameObject effectPrefab;         // 이펙트 프리팹

    [Header("스킬")]
    public SkillCategory category;
    public SkillExecuter executer;
    public DamageType damageType;
    public EvasionType evasionType;

    [Header("타겟팅")]
    public TargetingData targetingData;     // 타겟팅 방식

    public void Execute(Character user, Character target)
    {
        executer?.Execute(user, target, this);
    }
}
