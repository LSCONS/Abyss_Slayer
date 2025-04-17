using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BuffDoubleShotSkill", menuName = "SkillRefactory/Buff/DoubleShotSkill")]
public class DoubleShotBuffSkill : BuffSkill
{
    [field: SerializeField]public int ProjectileCount { get; private set; } = 2;
    public override void UseSkill()
    {
        base.UseSkill();
        player.SetBuff(this);
    }
}
