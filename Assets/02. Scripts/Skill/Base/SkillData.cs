using UniRx;
using UnityEngine;


// 스킬의 공통으로 필요한 데이터를 저장하는 ScriptableObject.
[CreateAssetMenu(menuName = "Skill/Base/SkillData")]
public class SkillData : ScriptableObject
{
    [Header("기본 정보")]
    public string skillName;                // 스킬명
    public string description;              // 설명
    public bool canMove;                    // 움직임 가능 여부
    public bool canUse;                     // 사용 가능 여부
    [field: SerializeField] public ReactiveProperty<float> MaxCoolTime { get; set; }    // 최대 쿨타임
    [field: SerializeField] public ReactiveProperty<float> CurCoolTime { get; set; }    // 현재 쿨타임

    [Header("시각적 요소")]
    public Sprite icon;                     // 스킬 아이콘
    public string animationTrigger;         // Animator에서 호출할 트리거 이름
    public GameObject effectPrefab;         // 이펙트 프리팹

    [Header("스킬")]
    public SkillExecuter executer;			// 스킬 실행 클래스
    public SkillCategory category;			// 스킬 종류
    public DamageType damageType;			// 데미지 타입 (물리, 마법)
    public ApplyState applyState;           // 연결 가능한 State 체크

    [Header("타겟팅")]
    public TargetingData targetingData;     // 타겟팅 데이터

    // 스킬 실행 추상 클래스 호출
    public void Execute(Player user, Boss target)
    {
        executer?.Execute(user, target, this);
    }
}
