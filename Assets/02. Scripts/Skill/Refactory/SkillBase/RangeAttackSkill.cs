using UnityEngine;

public class RangeAttackSkill : Skill
{
    [field: SerializeField] public float Damage { get; private set; } // 데미지
    [field: SerializeField] public float Range { get; private set; } // 사거리
    [field: SerializeField] public float Speed { get; private set; } // 투사체 속도
    [field: SerializeField] public int SpriteNum { get; private set; } // Sprite 인덱스 번호


    /// <summary>
    /// 투사체 발사
    /// </summary>
    /// <typeparam name="T">투사체 타입</typeparam>
    /// <param name="startPos">투사체 시작 위치</param>
    /// <param name="dir">투사체 방향</param>
    public void ThrowProjectile<T>(Vector3 startPos, Vector3 dir) where T : BasePoolable
    {
        PoolManager.Instance.Get<T>().Init(startPos, dir, Range, Speed, SpriteNum, Damage);
    }

    /// <summary>
    /// 버프 상태 투사체 발사
    /// </summary>
    /// <typeparam name="T">투사체 타입</typeparam>
    /// <param name="startPos">투사체 시작 위치</param>
    /// <param name="dir">투사체 방향</param>
    /// <param name="damageMultiple">투사체 데미지 배율</param>
    public void ThrowProjectile<T>(Vector3 startPos, Vector3 dir, float damageMultiple) where T : BasePoolable
    {
        PoolManager.Instance.Get<T>().Init(startPos, dir, Range, Speed, SpriteNum, Damage * damageMultiple);
    }
}
