using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingProjectile : BasePoolable
{
    Animator _animator;
    NormalDamageCollider hitCollider;
    [SerializeField] float homingTime;
    [SerializeField] AnimationCurve speedCurve;
    [SerializeField] AnimationCurve homingCurve;     
    [SerializeField] TrailRenderer trailRenderer;
    int _damage;
    Transform _target;
    float _inputSpeed;
    float _speed;
    float _homingPower;
    float _explosionSize;

    bool _inited;
    float _fireTime;
    bool _fired;

    private void Update()
    {
        if (_inited)
        {
            if(_fireTime <= Time.time)  //대기시간 대기 후 실제 발사
            {
                _inited = false;
                _fired = true;
                //발사사운드 삽입
            }
        }
        else if (_fired)                //발사, 이동,회전
        {
            Move();
            Rotate();
        }

    }
    private void Awake()
    {
         trailRenderer.enabled = false; //탄 궤적 비활성
        _animator = GetComponent<Animator>();
        hitCollider = GetComponent<NormalDamageCollider>();
    }
    public override void Init()
    {
    }

    /// <summary>
    /// 유도탄 초기화
    /// </summary>
    /// <param name="damage">데미지</param>
    /// <param name="position">생성위치</param>
    /// <param name="rotate">생성시 회전값</param>
    /// <param name="target">따라갈 목표</param>
    /// <param name="speed">전체적인 탄속도(비례하여 유동적으로 변화)</param>
    /// <param name="delayFireTime">지연발사 시간</param>
    /// <param name="homingPower">전체적인 유도력(비례하여 유동적으로 변화)</param>
    public void Init(int damage, Vector3 position, Quaternion rotate, Transform target, float speed, float delayFireTime = 0f, float homingPower = 10f, float homingTime = 3f, float explosionSize = 0.5f, AnimationCurve homingCurve = null, AnimationCurve speedCurve = null)
    { 
        transform.localScale = Vector3.one;
        _damage = damage;
        transform.position = position;
        transform.rotation = rotate;
        _target = target;
        _inputSpeed = speed;
        _homingPower = homingPower;
        _explosionSize = explosionSize;
        _inited = true;
        _fireTime = Time.time + delayFireTime;
        this.homingTime = homingTime;
        if(homingCurve != null)
            this.homingCurve = homingCurve;
        if(speedCurve != null)
            this.speedCurve = speedCurve;

        hitCollider.Init(0,Destroy);     //하위 충돌여부 판단하는 콜라이더 소지 오브젝트 초기화
        trailRenderer.enabled = true;   //탄 궤적 활성화
    }

    void Move()                     //정해진 속도에 따라, 자신(투사체)의 right(+x)방향으로 고정적으로 진행
    {
        _speed = _inputSpeed * speedCurve.Evaluate((Time.time - _fireTime)/homingTime);   //animationCurve와 시간 에따라 속도 유동적으로 변경
        transform.Translate(Vector3.right * 10 * _speed * Time.deltaTime);
    }

    void Rotate()                   //정해진 유도력에 따라, 자신의 rotation.z를 회전
    {
        Vector3 targetDirection = _target.position - transform.position;                        
        float targetAngle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;              //목표물과의 각도 계산

        float _homingSpeed = _homingPower * homingCurve.Evaluate((Time.time - _fireTime) / homingTime);   //animationCurve와 시간 에따라 유도력 유동적으로 변경

        float newAngle = Mathf.MoveTowardsAngle(transform.eulerAngles.z, targetAngle, 10 * _homingSpeed * Time.deltaTime);  
        transform.rotation = Quaternion.Euler(0, 0, newAngle);                                                         //유동적인 유도력에 따라 자신을 회전
    }

    public void Destroy()
    {
        trailRenderer.enabled = false;  //탄궤적 비활성화(오브젝트풀 사용하기에 안끄면 생성시 마지막 위치에서 생성위치까지 궤적생김)
        _fired = false;
        transform.rotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));
        _animator.SetTrigger("Explosion");
        transform.localScale = Vector3.one * (_explosionSize / 0.7f);
    }
    public void GiveDamage()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, _explosionSize, LayerData.PlayerLayerMask);
        if(hits.Length <=0) return;
        for (int i = 0; i < hits.Length; i++)
        {
            hits[i].GetComponent<Player>().Damage(_damage);
        }
    }
}
