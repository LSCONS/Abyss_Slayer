using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkObjectFollowServer : NetworkBehaviour
{
    public Vector2 TargetPosition {  get; set; }
    public override void Spawned()
    {
#if AllMethodDebug
        Debug.Log("Spawned");
#endif
        base.Spawned();
        PoolManager.Instance.CrossHairObject = this;
        transform.parent = null;
        transform.parent = PoolManager.Instance.transform;
        gameObject.SetActive(false);
    }

    public void Update()
    {
        if(TargetPosition != (Vector2)transform.position)
        {
            transform.position = TargetPosition;
        }
    }


    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void Rpc_ChangePosition(Vector2 position)
    {
        TargetPosition = position;
    }
}
