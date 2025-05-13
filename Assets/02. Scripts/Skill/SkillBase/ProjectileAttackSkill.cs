using UnityEngine;

public class ProjectileAttackSkill : Skill
{
    [field: Header("투사체 초기 데미지")]
    [field: SerializeField] public float BaseDamage { get; private set; } // 초기 데미지

    [field: Header("투사체 최종 데미지")]
    [field: SerializeField] public float Damage { get; private set; } // 데미지

    [field: Header("투사체 사거리")]
    [field: SerializeField] public float Range { get; private set; } // 사거리

    [field: Header("투사체 속도")]
    [field: SerializeField] public float Speed { get; private set; } // 투사체 속도

    [field: Header(("추가 투사체 초기 데미지 배율"))]
    [field: SerializeField] public float BaseDamageMultiple { get; private set; } = 0.6f; // 추가 투사체 초기 데미지 배율

    [field: Header(("추가 투사체 데미지 배율"))]
    [field: SerializeField] public float DamageMultiple { get; set; } = 0.6f; // 추가 투사체 데미지 배율


    /// <summary>
    /// 투사체 발사
    /// </summary>
    /// <typeparam name="T">투사체 타입</typeparam>
    /// <param name="startPos">투사체 시작 위치</param>
    /// <param name="dir">투사체 방향</param>
    public void ThrowProjectile(Vector3 startPos, Vector3 dir)
    {
        PoolManager.Instance.Get<RogueProjectile>().Init(startPos, dir, Range, Speed, Damage);
    }

    /// <summary>
    /// 버프 상태 투사체 발사
    /// </summary>
    /// <typeparam name="T">투사체 타입</typeparam>
    /// <param name="startPos">투사체 시작 위치</param>
    /// <param name="dir">투사체 방향</param>
    /// <param name="Damage">투사체 데미지</param>
    public void ThrowProjectile(Vector3 startPos, Vector3 dir, float Damage)
    {
        PoolManager.Instance.Get<RogueProjectile>().Init(startPos, dir, Range, Speed, Damage * DamageMultiple);
    }

    public override void SkillUpgrade()
    {
        base.SkillUpgrade();
        Level.Value++;
        Damage = BaseDamage * (1.0f + (Level.Value - 1) * Magnification);
    }
}
