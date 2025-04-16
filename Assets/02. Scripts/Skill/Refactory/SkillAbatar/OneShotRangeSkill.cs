using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RangeOneShotSkill", menuName = "SkillRefactory/Range/OneShot")]
public class OneShotRangeSkill : RangeAttackSkill
{
    private Vector3 distanceY = new Vector3(0, 0.25f, 0);
    public override void UseSkill()
    {
        base.UseSkill();
        Vector3 DirX = new Vector3(PlayerFrontXNomalized() * 1.5f, 0 ,0);  
        Vector3 spawnPos = PlayerPosition() + DirX;

        // 버프 상태일 경우 추가 화살 생성
        if (player.BuffDuration.ContainsKey(BuffType.ArchorDoubleShot))
        {
            ThrowProjectile<ArcherProjectile>(spawnPos + distanceY, DirX, 0.8f);
            ThrowProjectile<ArcherProjectile>(spawnPos - distanceY, DirX, 0.8f);
        }
        else
        {
            ThrowProjectile<ArcherProjectile>(spawnPos, DirX);
        }
    }
}
