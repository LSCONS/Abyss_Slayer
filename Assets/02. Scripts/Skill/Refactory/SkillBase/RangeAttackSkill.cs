using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeAttackSkill : Skill
{
    [SerializeField] private float damage;
    [SerializeField] private float range;
    [SerializeField] private float speed;
    [SerializeField] private int spriteNum;


    public void ThrowProjectile<T>(Vector3 startPos, Vector3 dir) where T : BasePoolable
    {
        PoolManager.Instance.Get<T>().Init(startPos, dir, range, speed, spriteNum, damage);
    }

    public void ThrowProjectile<T>(Vector3 startPos, Vector3 dir, float damageMultiple) where T : BasePoolable
    {
        PoolManager.Instance.Get<T>().Init(startPos, dir, range, speed, spriteNum, damage * damageMultiple);
    }
}
