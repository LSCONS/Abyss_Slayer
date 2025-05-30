using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewDoubleShotSkill", menuName = "Skill/Buff/DoubleShot")]
public class DoubleShotSkill : BuffSkill
{
    public override void UseSkill()
    {
        base.UseSkill();
        player.Rpc_SetBuff((int)slotKey);
    }
}
