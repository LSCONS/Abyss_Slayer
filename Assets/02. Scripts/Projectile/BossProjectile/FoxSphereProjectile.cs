using Fusion;
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
    Vector3 _direction;
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
                animator.SetTrigger(AnimationHash.EndParameterHash);
                _end = true;
            }

        }
        else if(Time.time >= _fireTime)
        {
            targetDirection =(_direction == Vector3.zero)? (_target.position - transform.position).normalized : _direction;
            _fired = true;
        }
    }
    public override void Rpc_Init()
    {  
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void Rpc_Init(int damage,Vector3 startPosition, float preDelayTime, PlayerRef target, float speed, float distance, int color, float angle = 10)
    {
        gameObject.SetActive(true);
        damageCollider.Init(damage, null, int.MaxValue);
        transform.position = startPosition;
        _fireTime = Time.time + preDelayTime;
        
        animator.SetFloat(AnimationHash.CreationSpeedParameterHash, 1 / (Mathf.Min(0.9f, preDelayTime)));
        _target = ServerManager.Instance.DictRefToPlayer[target].transform;
        _v = speed;
        _a = _v * _v / (2 * distance);
        _returnTime = _fireTime + (2 * distance / _v);
        _endTime = _fireTime + (3.9f * distance / _v);
        animator.SetInteger(AnimationHash.ColorParameterHash, color);

        _fired = false;
        _isReturn = false;
        _end = false;
        _direction = angle == 10? Vector3.zero : new Vector3(Mathf.Cos(angle),Mathf.Sin(angle));
    }

    void Move()
    {
        transform.position += targetDirection * ((_v - (_a * (Time.time - _fireTime))) * Time.deltaTime);
    }

    public override void Rpc_ReturnToPool()
    {
        animator.SetInteger(AnimationHash.ColorParameterHash, -1);
        base.Rpc_ReturnToPool();
    }
}
