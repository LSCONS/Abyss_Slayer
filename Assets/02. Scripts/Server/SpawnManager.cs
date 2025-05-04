using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : SingletonNetwork<SpawnManager>
{
    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RequestSpawn_RPC<T>(BasePoolable basePoolable) where T : BasePoolable
    {
        if (!Object.HasStateAuthority) return;

        //var netobj = Runner.Spawn(basePoolable);
    }
}
