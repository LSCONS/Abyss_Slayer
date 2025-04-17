using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RangeHoldingShotSkill", menuName = "SkillRefactory/Range/HoldingShot")]
public class HoldingShotRangeSkill : RangeAttackSkill
{
    private Vector3 distanceY = new Vector3(0, 0.25f, 0);
    public override void UseSkill()
    {
        
    }
}
