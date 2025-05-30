using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpEffect : BasePoolable
{
    public override void Rpc_Init()
    {
    }
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void Rpc_Init(Vector3 position, float size = 1f)
    {
        gameObject.SetActive(true);
        transform.position = position;
        transform.localScale = Vector3.one * size;
    }

    public override void Rpc_ReturnToPool()
    {
        if(Runner.IsServer)
        base.Rpc_ReturnToPool();
    }
}
