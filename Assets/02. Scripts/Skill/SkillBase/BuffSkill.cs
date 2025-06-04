using System.Collections.Generic;
using UniRx;
using UnityEngine;

/// <summary>
/// 버프 효과를 가진 스킬의 기본 클래스
/// 지속시간과 적용 여부를 관리하며, 특정 버프 타입에 따라 추가 효과를 적용할 수 있음
/// </summary>
public class BuffSkill : Skill
{
    [field: Header("버프 초기화할 지속 시간")]
    [field: SerializeField]public ReactiveProperty<float> MaxBuffDuration { get; set; } // 최대 지속시간
        = new ReactiveProperty<float>(5f);
    public ReactiveProperty<float> CurBuffDuration { get; set; } // 현재 지속시간
        = new ReactiveProperty<float>(0f);
    public bool IsApply { get; set; } = false; // 현재 버프 적용 여부
    [field: Header("버프 타입")]
    [field: SerializeField] public EBuffType Type { get; private set; } = EBuffType.None; // 버프 타입
    [field: Header("적용된 버프의 정보")]
    [field: SerializeField] public BuffInfo Info { get; set; }

    // 버프 효과가 적용될 투사체 스킬들의 참조 저장할 배열
    protected ProjectileAttackSkill[] projectileAttackSkills;

    // 스킬 참조 메서드
    public override void Init()
    {
        base.Init();

        // 사용한 버프의 타입이 더블샷인 경우 투사체 스킬들을 찾아서 참조
        if (Type == EBuffType.RogueDoubleShot)
        {
            var foundSkills = new List<ProjectileAttackSkill>(); // 투사체 스킬 리스트 저장
            
            // 장착된 스킬들 중에서 투사체 스킬 찾기
            foreach (var skill in player.DictSlotKeyToSkill.Values)
            {
                if (skill is ProjectileAttackSkill projectileSkill)
                {
                    foundSkills.Add(projectileSkill);
                }
            }
            projectileAttackSkills = foundSkills.ToArray(); // 찾은 투사체 스킬들을 배열로 변환하여 저장
        }
    }

    // 스킬 강화 메서드
    public override void SkillUpgrade()
    {
        base.SkillUpgrade();
    }
}
