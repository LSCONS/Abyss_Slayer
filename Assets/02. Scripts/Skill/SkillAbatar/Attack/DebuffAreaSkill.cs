using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skill/AreaSkill/DebuffAreaSkill")]
public class DebuffAreaSkill : AreaSkill
{
    [Header("디버프 타입 설정")]
    [SerializeField] private DebuffType debuffType;
    [Header("디버프 지속시간 설정")]
    [SerializeField] private float debuffDuration = 3f;

    public override void UseSkill()
    {
        base.UseSkill();

        var target = GetTargetsInArea();

        // 디버프 타입에 맞는 효과 생성
        var effect = DebuffEffectFactory.Create(debuffType);
        if (effect == null) return;
        if (target.TryGetComponent<Boss>(out var boss))
        {
            boss.ApplyDebuff(
                debuffType,
                debuffDuration,
                onApply: () =>
                {
                    effect.Apply(boss, effectAmount);   // 디버프 효과 적용시킴
                                                        // ui등록 해줘야됨
                    var data = boss.GetDebuffData(debuffType);
                    UIBossBuffSlotManager.Instance.CreateSlot(debuffType, data, SkillIcon);
                },
                onExpire: () => effect.Expire(boss)
            );
        }
    }
}
