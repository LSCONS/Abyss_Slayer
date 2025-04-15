using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MageProjectile : BasePoolable
{
    [SerializeField] float dampDuration;
    [SerializeField] AnimationCurve speedCurve;
    [SerializeField] AnimationCurve homingCurve;
    [SerializeField] TrailRenderer trailRenderer;

    int damage;
    Transform target;
    float inputSpeed, speed, homingPower;

    [SerializeField] List<Sprite> sprites;
    [SerializeField] SpriteRenderer spriteRenderer;

    public override void Init()
    {
        // 호출용
    }
    
    /// <summary>
    /// 법사 유도탄 스킬킬 초기화
    /// </summary>
    /// <param name="damage">데미지</param>
    /// <param name="position">생성위치</param>
    /// <param name="target">따라갈 목표</param>
    /// <param name="speed">투사체 속도(비례하여 유동적으로 변화)</param>
    /// <param name="homingPower">투사체 유도력(비례하여 유동적으로 변경)</param>
    public void Init(Vector3 position, Quaternion rotation, int damage, Transform target, float speed, float homingPower)
    {
        transform.position = position;
        this.damage = damage;
        this.target = target;
        inputSpeed = speed;
        this.homingPower = homingPower;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<Dummy>(out Dummy dummy))
        {
            dummy.TakeDamage(damage); // 데미지 전달
        }

        ReturnToPool(); // 투사체 반환
    }
}
