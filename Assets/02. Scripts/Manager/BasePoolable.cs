using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(NetworkObject))]
public abstract class BasePoolable : NetworkBehaviour
{
    public ObjectPool _pool { get; set; }


    public override void Spawned()
    {
        base.Spawned();
        gameObject.SetActive(false);
    }

    //풀 설정
    public virtual void SetPool(ObjectPool pool)
    {
        _pool = pool;
    }

    //반드시 오버로딩하여 사용
    public abstract void Rpc_Init();


    // aliveTime 후에 풀에 반환
    public virtual void AutoReturn(float aliveTime){}


    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public virtual void Rpc_ReturnToPool()
    {
#if AllMethodDebug
        Debug.Log("Rpc_ReturnToPool");
#endif
        gameObject.SetActive(false);
        if (Runner.IsServer)
        {
            _pool.ReturnToPool(this);
        }
    }
}
