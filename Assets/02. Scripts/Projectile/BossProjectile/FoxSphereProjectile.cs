using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoxSphereProjectile : BasePoolable
{
    [SerializeField] Animator animator;
    [SerializeField] NormalDamageCollider damageCollider;
    float _fireTime;
    float _returnTime;
    float _endTime;
    Transform _target;
    float _v;           //출발속도
    float _a;           //가속도

    bool _fired;
    bool _isReturn;
    bool _end;
    Vector3 targetDirection;

    private void Update()
    {
        if (_fired)
        {
            Move();
            if(!_isReturn && Time.time >= _returnTime)
            {
                damageCollider.ClearHitList();
                _isReturn = true;
            }
            if (!_end && Time.time >= _endTime)
            {
                animator.SetTrigger("End");
                _end = true;
            }

        }
        else if(Time.time >= _fireTime)
        {
            targetDirection = (_target.position - transform.position).normalized;
            _fired = true;
        }
    }
    public override void Init()
    {  
    }
    public void Init(int damage,Vector3 startPosition, float preDelayTime, Transform target, float speed, float distance, int color)
    {
        damageCollider.Init(damage, null, int.MaxValue);
        transform.position = startPosition;
        _fireTime = Time.time + preDelayTime;
        animator.SetFloat("CreationSpeed", 1 / (Mathf.Min(0.9f, preDelayTime)));
        _target = target;
        _v = speed;
        _a = _v * _v / (2 * distance);
        _returnTime = _fireTime + (2 * distance / _v);
        _endTime = _fireTime + (3.9f * distance / _v);
        animator.SetInteger("Color", color);

        _fired = false;
        _isReturn = false;
        _end = false;
    }

    void Move()
    {
        transform.position += targetDirection * ((_v - (_a * (Time.time - _fireTime))) * Time.deltaTime);
    }

    public override void ReturnToPool()
    {
        animator.SetInteger("Color", -1);
        base.ReturnToPool();
    }
}
