using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeAttackSkill : Skill
{
    public float damage = 10f;
    public float range = 5f;
    public float speed = 1f;
    public int spriteNum = 1;


    public void ThrowProjectile<T>(Vector3 startPos, Vector3 dir) where T : BasePoolable
    {
        PoolManager.Instance.Get<T>().Init(startPos, dir, range, speed, spriteNum, damage);
    }

    public void ThrowProjectile<T>(Vector3 startPos, Vector3 dir, float damageMultiple) where T : BasePoolable
    {
        PoolManager.Instance.Get<T>().Init(startPos, dir, range, speed, spriteNum, damage * damageMultiple);
    }
}
