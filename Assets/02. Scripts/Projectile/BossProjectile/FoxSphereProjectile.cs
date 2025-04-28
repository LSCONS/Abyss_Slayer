using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoxSphereProjectile : BasePoolable
{
    [SerializeField] Animator animator;
    [SerializeField] NormalDamageCollider damageCollider;
    int _damage;
    float _fireTime;
    float _endTime;
    Transform _target;
    float _v;           //출발속도
    float _a;           //가속도

    bool _fired;
    Vector3 targetPosition;
    private void Update()
    {
        if (_fired)
        {
            Move();
            if(Time.time >= _endTime)
            {
                animator.SetTrigger("End");
            }
        }
        else if(Time.time >= _fireTime)
        {
            targetPosition = _target.position;
            _fired = true;
        }
    }
    public override void Init()
    {  
    }
    public void Init(int damage,Vector3 startPosition, float preDelayTime, Transform target, float speed, float distance)
    {
        _damage = damage;
        transform.position = startPosition;
        _fireTime = Time.time + preDelayTime;
        animator.SetFloat("CreationSpeed", 1 / (Mathf.Min(0.9f, preDelayTime)));
        _target = target;
        _v = speed;
        _a = _v * _v / (2 * distance);
        _endTime = _fireTime + (3.9f * distance / _v);

        _fired = false;
    }

    void Move()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, (_v - (_a * Time.deltaTime)) * Time.deltaTime);
    }

    public void ReturnDamageInit()
    {

    }
}
