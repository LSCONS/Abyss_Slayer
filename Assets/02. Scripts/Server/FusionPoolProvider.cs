using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

/// <summary>
/// 오브젝트 풀링을 할 때 사용할 클래스
/// </summary>
public class FusionPoolProvider : NetworkObjectProviderDefault
{
    public override NetworkObjectAcquireResult AcquirePrefabInstance(
        NetworkRunner runner,
        in NetworkPrefabAcquireContext context,
        out NetworkObject instance)
    {
        var result = base.AcquirePrefabInstance(runner, context, out instance);
        if (result != NetworkObjectAcquireResult.Success)
            return result;

        var template = instance.GetComponent<BasePoolable>();
        if (template != null)
        {
            GameObject.Destroy(instance.gameObject);

            var pooled = PoolManager.Instance.Get(template.GetType()) as BasePoolable;
            pooled.gameObject.SetActive(true);

            instance = pooled.GetComponent<NetworkObject>();

            pooled.Init();
        }
        return NetworkObjectAcquireResult.Success;
    }


    public override void ReleaseInstance(
        NetworkRunner runner,
        in NetworkObjectReleaseContext context)
    {
        var obj = context.Object;
        var poolable = obj.GetComponent<BasePoolable>();
        if (poolable != null)
        {
            runner.Prefabs.RemoveInstance(context.TypeId.AsPrefabId);

            poolable.ReturnToPool();
            return;
        }

        base.ReleaseInstance(runner, context);
    }
}
