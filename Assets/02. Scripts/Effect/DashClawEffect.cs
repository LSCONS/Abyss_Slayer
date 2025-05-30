using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashClawEffect : BasePoolable
{
    [SerializeField] Animator animator;                     //인스펙터 상 연결
    [SerializeField] NormalDamageCollider damageCollider;   //인스펙터 상 연결
    float _distance;
    float _attackStartTime;

    private void Update()
    {
        if (Time.time >= _attackStartTime)
        {
            animator.SetTrigger("Attack");
            transform.localScale = Vector3.one + Vector3.right * (_distance / 5f - 1);
            enabled = false;
        }

    }
    public override void Rpc_Init()
    {
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void Rpc_Init(int damage, Vector3 startPosition, bool isLeft, float distance = 15f, float preDelayTime = 1f, float attackTime = 1f)
    {
        gameObject.SetActive(true);
        transform.localScale = Vector3.one;
        damageCollider.Init(damage, null, int.MaxValue);
        transform.position = startPosition + (Vector3.right * (isLeft ? 1 : -1));
        transform.rotation = Quaternion.Euler(0, isLeft ? 0 : 180, 0);
        _distance = distance;

        animator.SetFloat(AnimationHash.PreDelayTimeParameterHash,(preDelayTime >= 1)? 1 : 1/preDelayTime);
        animator.SetFloat(AnimationHash.AttackDurationParameterHash,1/attackTime);

        _attackStartTime = Time.time + preDelayTime;
        enabled = true;
    }
}
