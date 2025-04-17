using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeAttackSkill : Skill
{
    [field: SerializeField] public float Damage { get; private set; }     // 데미지
    [field: SerializeField] public float Range { get; private set; }      // 사거리
    [field: SerializeField] public float Speed { get; private set; }      // 투사체 속도
    [field: SerializeField] public int SpriteNum { get; private set; }    // Sprite 인덱스 번호


    public void ThrowProjectile<T>(Vector3 startPos, Vector3 dir) where T : BasePoolable
    {
        PoolManager.Instance.Get<T>().Init(startPos, dir, Range, Speed, SpriteNum, Damage);
    }

    public void ThrowProjectile<T>(Vector3 startPos, Vector3 dir, float damageMultiple) where T : BasePoolable
    {
        PoolManager.Instance.Get<T>().Init(startPos, dir, Range, Speed, SpriteNum, Damage * damageMultiple);
    }
}
