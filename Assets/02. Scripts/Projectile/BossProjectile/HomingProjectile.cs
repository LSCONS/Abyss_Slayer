using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingProjectile : BasePoolable
{
    [SerializeField] float dampDuration;
    [SerializeField] AnimationCurve speedCurve;
    [SerializeField] AnimationCurve homingCurve;
    [SerializeField] BaseBossProjectileCollider _collider;
    [SerializeField] TrailRenderer trailRenderer;
    int _damage;
    Transform _target;
    float _inputSpeed;
    float _speed;
    float _homingPower;

    bool _inited;
    float _fireTime;
    bool _fired;

    private void Update()
    {
        if (_inited)
        {
            if(_fireTime <= Time.time)
            {
                _inited = false;
                _fired = true;
                //발사사운드 삽입
            }
        }
        else if (_fired)
        {
            Move();
            Rotate();
        }

    }
    private void Awake()
    {
         trailRenderer.enabled = false;
    }
    public override void Init()
    {
    }
    public void Init(int damage, Vector3 position, Quaternion rotate, Transform target,float speed, float delayFireTime = 0f, float homingPower = 10f)
    {
        _damage = damage;
        transform.position = position;
        transform.rotation = rotate;
        _target = target;
        _inputSpeed = speed;
        _homingPower = homingPower;
        _inited = true;
        _fireTime = Time.time + delayFireTime;

        _collider.colliderSet(Hit);
        trailRenderer.enabled = true;
    }

    void Move()
    {
        _speed = _inputSpeed * speedCurve.Evaluate((Time.time - _fireTime)/dampDuration);
        transform.Translate(Vector3.right * 10 * _speed * Time.deltaTime);
    }
    void Rotate()
    {
        Vector3 targetDirection = _target.position - transform.position;
        float targetAngle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;

        float _homingSpeed = _homingPower * homingCurve.Evaluate((Time.time - _fireTime) / dampDuration);

        float newAngle = Mathf.MoveTowardsAngle(transform.eulerAngles.z, targetAngle, 10 * _homingSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Euler(0, 0, newAngle);
    }

    public void Hit()
    {
        trailRenderer.enabled = false;
        _fired = false;
        PoolManager.Instance.Get<Explosion>().Init(transform.position, _damage, 0.1f);
        ReturnToPool();
    }
}
