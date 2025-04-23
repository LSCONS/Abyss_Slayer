using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewDoubleShotSkill", menuName = "Skill/Buff/DoubleShot")]
public class DoubleShotSkill : BuffSkill
{
    [field: Header("발사할 탄환 개수")]
    [field: SerializeField]public int ProjectileCount { get; private set; } = 2;
    public override void UseSkill()
    {
        base.UseSkill();
        player.SetBuff(this);
    }
}
