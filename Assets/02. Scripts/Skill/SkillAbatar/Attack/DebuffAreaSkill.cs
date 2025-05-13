using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skill/AreaSkill/DebuffAreaSkill")]
public class DebuffAreaSkill : AreaSkill
{
    [SerializeField] private DebuffType debuffType;
    [SerializeField] private float debuffDuration = 3f;

    public override void UseSkill()
    {
        base.UseSkill();

        var targets = GetTargetsInArea();

        // 디버프 타입에 맞는 효과 생성
        var effect = DebuffEffectFactory.Create(debuffType);
        if (effect == null) return;

        foreach (var target in targets)
        {
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
}
