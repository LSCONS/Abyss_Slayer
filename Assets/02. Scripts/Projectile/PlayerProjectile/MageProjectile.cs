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
        transform.position = position; // 투사체 위치
        transform.rotation = rotation; // 투사체 회전
        this.damage = damage; // 데미지
        this.target = target; // 타겟
        inputSpeed = speed; // 입력 속도
        this.homingPower = homingPower; // 유도력
        if(homingCurve != null) this.homingCurve = homingCurve; // 유도 곡선
        this.homingTime = homingTime; // 유도 시간
        trailRenderer.Clear();
        trailRenderer.enabled = false; // 궤적 비활성화
        fired = true; // 발사 여부
        fireTime = Time.time; // 발사 시간
    }

    // 속도에 따라 투사체의 right(+x)방향으로 고정적으로 진행
    void Move()
    {
        trailRenderer.enabled = true;

        // animationCurve와 시간에 따라 유동적으로 속도 변경
        speed = inputSpeed * speedCurve.Evaluate((Time.time - fireTime)/homingTime);

        transform.Translate(Vector3.right * 10 * speed * Time.deltaTime);
    }

    // 유도력에 따라 회전
    void Rotate() 
    {        
        Vector3 targetDirection = target.position - transform.position; // 타겟 방향 계산          
        float targetAngle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg; // 목표물과의 각도 계산

        // animationCurve와 시간에 따라 유동적으로 유도력 변경
        float homingSpeed = homingPower * homingCurve.Evaluate((Time.time - fireTime) / homingTime);

        float newAngle = Mathf.MoveTowardsAngle(transform.eulerAngles.z, targetAngle, 10 * homingSpeed * Time.deltaTime); // 각도 계산    
        transform.rotation = Quaternion.Euler(0, 0, newAngle);
    }

    // 타겟과 충돌 시 행동
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerData.EnemyLayerIndex && collision.TryGetComponent<IHasHealth>(out IHasHealth enemy))
        {
            trailRenderer.enabled = false; // 투사체 궤적 비활성화
            fired = false; // 발사 여부 초기화
            enemy.Damage((int)damage, transform.position.x); // 데미지 전달
            ReturnToPool(); // 투사체 반환
        }
    }
}
