using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MageProjectile : BasePoolable
{
    float homingTime;
    [SerializeField] AnimationCurve speedCurve;
    [SerializeField] AnimationCurve homingCurve;
    [SerializeField] TrailRenderer trailRenderer;

    // Init으로 전달받은 매개변수를 저장하는 변수
    Transform target;
    float damage, inputSpeed, speed, homingPower, fireTime;
    bool fired = false;

    private void Update()
    {
        if(fired)
        {
            Move();
            Rotate();
            UpdateTrailPosition();
        }
    }

    // 투사체 궤적 위치 업데이트
    private void UpdateTrailPosition()
    {
        trailRenderer.transform.position = transform.position;
        trailRenderer.transform.rotation = transform.rotation;
    }

    public override void Init()
    {
        // BasePoolable의 추상 메서드 구현
    }
    
    /// <summary>
    /// 법사 유도탄 스킬 초기화
    /// </summary>
    /// <param name="damage">데미지</param>
    /// <param name="position">생성위치</param>
    /// <param name="rotation">생성 시 회전값</param>
    /// <param name="target">따라갈 목표</param>
    /// <param name="speed">투사체 속도(비례하여 유동적으로 변화)</param>
    /// <param name="homingPower">투사체 유도력(비례하여 유동적으로 변경)</param>
    /// <param name="homingTime">투사체 유도시간</param>
    /// <param name="homingCurve">투사체 유도곡선</param>
    public void Init(float damage, Vector3 position, Quaternion rotation, Transform target, float speed, float homingPower, float homingTime, AnimationCurve homingCurve)
    {
        transform.position = position;
        transform.rotation = rotation;
        this.damage = damage;
        this.target = target;
        inputSpeed = speed;
        this.homingPower = homingPower;
        if(homingCurve != null) this.homingCurve = homingCurve;
        this.homingTime = homingTime;
        trailRenderer.Clear();
        trailRenderer.enabled = true;
        fired = true;
        fireTime = Time.time;
    }

    //정해진 속도에 따라, 자신(투사체)의 right(+x)방향으로 고정적으로 진행
    void Move()
    {
        speed = inputSpeed * speedCurve.Evaluate((Time.time - fireTime)/homingTime); //animationCurve와 시간 에따라 속도 유동적으로 변경
        transform.Translate(Vector3.right * 10 * speed * Time.deltaTime);
    }

    // 정해진 유도력에 따라 회전
    void Rotate() 
    {        
        Vector3 targetDirection = target.position - transform.position;                                                     // 타겟 방향 계산          
        float targetAngle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;                              // 목표물과의 각도 계산

        float homingSpeed = homingPower * homingCurve.Evaluate((Time.time - fireTime) / homingTime);                        // animationCurve와 시간 에따라 유도력 유동적으로 변경

        float newAngle = Mathf.MoveTowardsAngle(transform.eulerAngles.z, targetAngle, 10 * homingSpeed * Time.deltaTime);   // 각도 계산    
        transform.rotation = Quaternion.Euler(0, 0, newAngle);                                                              // 유동적인 유도력에 따라 자신을 회전
    }

    // 타겟과 충돌 시 행동
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<Boss>(out Boss boss))
        {
            trailRenderer.enabled = false;      // 투사체 궤적 비활성화
            fired = false;                      // 발사 여부 초기화
            boss.Damage((int)damage);           // 데미지 전달
            ReturnToPool();                     // 투사체 반환
        }
    }
}
