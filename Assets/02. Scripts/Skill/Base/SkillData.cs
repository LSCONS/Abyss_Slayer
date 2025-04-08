using UnityEngine;
using UnityEngine.TextCore.Text;

// 스킬의 공통으로 필요한 데이터를 저장하는 ScriptableObject.
[CreateAssetMenu(menuName = "Skill/Base/SkillData")]
public class SkillData : ScriptableObject
{
    [Header("기본 정보")]
    public string skillName;                // 스킬명
    public string description;              // 설명
    public int manaCost;                    // 마나 소모량
    public float coolTime;                  // 쿨타임
    public bool canMove;

    [Header("시각적 요소")]
    public Sprite icon;                     // 스킬 아이콘
    public string animationTrigger;         // Animator에서 호출할 트리거 이름
    public GameObject effectPrefab;         // 이펙트 프리팹

    [Header("스킬")]
    public SkillExecuter executer;			// 스킬 실행 클래스
    public SkillCategory category;			// 스킬 종류
    public DamageType damageType;			// 데미지 타입 (물리, 마법)
    public EvasionType evasionType;			// 회피 종류 (대쉬, 텔포)

    [Header("타겟팅")]
    public TargetingData targetingData;     // 타겟팅 데이터

    public void Execute(Character user, Character target)
    {
        executer?.Execute(user, target, this);
    }
}
