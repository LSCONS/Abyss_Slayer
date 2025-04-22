using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

/// <summary>
/// 오브젝트 풀링을 할 때 사용할 클래스
/// </summary>
public class PooledNetworkObjectProvider : NetworkObjectProviderDefault
{
    
    protected override NetworkObject InstantiatePrefab(NetworkRunner runner, NetworkObject prefab)
    {
        return base.InstantiatePrefab(runner, prefab);

    }

    public override void ReleaseInstance(NetworkRunner runner, in NetworkObjectReleaseContext context)
    {
        base.ReleaseInstance(runner, context);
    }
}
