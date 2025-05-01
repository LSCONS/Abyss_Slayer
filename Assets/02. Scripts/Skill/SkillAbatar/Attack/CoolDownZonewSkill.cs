using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 특정 스킬이 적중할 경우 이 스킬의 쿨타임을 줄이는 기능이 있는 스킬
/// </summary>
[CreateAssetMenu(fileName = "NewCoolDownZonewSkill", menuName = "Skill/RangeAttack/CoolDownZone")]
public class CoolDownZonewSkill : RemoteZoneRangeSkill
{
    [field: Header("타격 시 줄이고 싶은 쿨타임 시간")]
    [field: SerializeField] public float CoolDownTime { get; private set; } = 0.1f;

    public override void UseSkill()
    {
        base.UseSkill();
    }

    public override void Init()
    {
        foreach(Skill skill in player.EquippedSkills.Values)
        {
            if (skill.IsConnectSkillCoolDown)
            {
                skill.AttackAction += CoolTimeDown;
            }
        }
    }

    private void CoolTimeDown()
    {
        if (!(CanUse))
        {
            CurCoolTime.Value -= CoolDownTime;
            if(CurCoolTime.Value <= 0)
            {
                CurCoolTime.Value = 0;
                CanUse = true;
            }
        }
    }
}
