using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skill/AreaSkill/DebuffAreaSkill")]
public class DebuffAreaSkill : AreaSkill
{
    [Header("디버프 타입 설정")]
    [SerializeField] private EBuffType debuffType;
    [Header("디버프 지속시간 설정")]
    [SerializeField] private float debuffDuration = 3f;

    public override void UseSkill()
    {
        base.UseSkill();
        Collider2D target = GetTargetsInArea();

        // 디버프 타입에 맞는 효과 생성
        IDebuff effect = DebuffEffectFactory.Create(debuffType);
        if (effect == null) return;
        if (target.TryGetComponent<Boss>(out Boss boss))
        {
            boss.ApplyDebuff(
                debuffType,
                debuffDuration,
                onApply: () =>
                {
                    effect.Apply(boss, effectAmount);   // 디버프 효과 적용시킴 ui등록 해줘야됨
                    DebuffData data = boss.GetDebuffData(debuffType);
                    UIBossBuffSlotManager.Instance.CreateSlot((int)debuffType, data.Name.StringToBytes(), data.Description.StringToBytes(), data.Duration, data.StartTime, SkillIcon);
                },
                onExpire: () => effect.Expire(boss)
            );
        }
    }
}
