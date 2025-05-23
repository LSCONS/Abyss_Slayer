using Fusion;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

public enum HomingProjectileType
{
    None = 0,
    FoxFire,
    Diamond
}
public class HomingProjectile : BasePoolable
{
    Animator _animator;
    NormalDamageCollider hitCollider;
    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;
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
    Vector3 _position;

    bool _inited = false;
    float _fireTime;
    bool _fired;

    //Vector3 fireTarget = new Vector3(0,40,0);

    public override void FixedUpdateNetwork()
    {
        if (_inited)
        {
            transform.position = _position;
            if (_fireTime <= Time.time)  //대기시간 대기 후 실제 발사
            {
                trailRenderer.enabled = true;
                _inited = false;
                _fired = true;
                //발사사운드 삽입
            }
            Rotate();
        }
        else if (_fired)                //발사, 이동,회전
        {
            Move();
            Rotate();
        }
    }


    public override void Spawned()
    {
        base.Spawned();
        trailRenderer.enabled = false; //탄 궤적 비활성
        _animator = GetComponentInChildren<Animator>();
        hitCollider = GetComponent<NormalDamageCollider>();
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponentInChildren <SpriteRenderer>();
    }

    public override void Rpc_Init()
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
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public async void Rpc_Init(int damage, Vector3 position, Quaternion rotate, PlayerRef target, float speed, int HomingProjectileType, float delayFireTime = 0f, float homingPower = 10f, float homingTime = 3f, float explosionSize = 0.5f, int homingCurve = 0, int speedCurve = 0)
    {
        gameObject.SetActive(true);
        spriteRenderer.enabled = false;
        _position = position;
        transform.position = _position;
        transform.localScale = Vector3.one;
        transform.rotation = rotate;
        _target = ServerManager.Instance.DictRefToPlayer[target].transform;
        _inputSpeed = speed;
        _homingPower = homingPower;
        _explosionSize = explosionSize;
        _fireTime = Time.time + delayFireTime;
        this.homingTime = homingTime;
        if (homingCurve != 0)
            this.homingCurve = DataManager.Instance.DictEnumToCurve[(EAniamtionCurve)homingCurve];
        if (speedCurve != 0)
            this.speedCurve = DataManager.Instance.DictEnumToCurve[(EAniamtionCurve)speedCurve];
        _animator.SetTrigger(((HomingProjectileType)HomingProjectileType).ToString());

        hitCollider.Init(damage, Destroy);     //하위 충돌여부 판단하는 콜라이더 소지 오브젝트 초기화
        transform.position = _position;
        Debug.Log("position = " + position);
        _inited = true;

        await Task.Delay(100);
        spriteRenderer.enabled = true; //탄 궤적 비활성
    }


    void Move()                     //정해진 속도에 따라, 자신(투사체)의 right(+x)방향으로 고정적으로 진행
    {
        _speed = _inputSpeed * speedCurve.Evaluate((Time.time - _fireTime));   //animationCurve와 시간 에따라 속도 유동적으로 변경
        rigid.velocity = 1000 * _speed * Runner.DeltaTime * (Vector2)transform.right;
    }


    void Rotate()                   //정해진 유도력에 따라, 자신의 rotation.z를 회전
    {
        //Vector3 _targetPos = (Time.time - _fireTime >= 1f) ? _target.position : fireTarget;
        //Vector3 targetDirection = _targetPos - transform.position;
        Vector3 targetDirection = _target.position - transform.position;
        float targetAngle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;              //목표물과의 각도 계산

        float _homingSpeed = _homingPower * homingCurve.Evaluate((Time.time - _fireTime) / homingTime);   //animationCurve와 시간 에따라 유도력 유동적으로 변경

        float newAngle = Mathf.MoveTowardsAngle(transform.eulerAngles.z, targetAngle, 10 * _homingSpeed * Runner.DeltaTime);
        transform.rotation = Quaternion.Euler(0, 0, newAngle);                                                         //유동적인 유도력에 따라 자신을 회전
    }


    public void Destroy()
    {
        trailRenderer.enabled = false;  //탄궤적 비활성화(오브젝트풀 사용하기에 안끄면 생성시 마지막 위치에서 생성위치까지 궤적생김)
        _fired = false;
        transform.rotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));
        _animator.SetTrigger("Explosion");
        transform.localScale = Vector3.one * (_explosionSize / 0.7f);
        rigid.velocity /= 5;
    }


    public void GiveDamage()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, _explosionSize, LayerData.PlayerLayerMask);
        if (hits.Length <= 0) return;
        for (int i = 0; i < hits.Length; i++)
        {
            hits[i].GetComponent<Player>().Damage(_damage);
        }
    }
}
