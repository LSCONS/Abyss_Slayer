using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashPlayerSilhouette : BasePoolable
{
    [field: SerializeField]private SpriteChange SpriteChange;
    private Color color = new(1, 1, 1, 0.4f);
    private readonly WaitForSeconds waitSeconds = new(0.24f);

    public override void Rpc_Init() { }


    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void Rpc_Init
        (
            PlayerRef playerRef,
            Vector3 position,
            bool flipX
        )
    {
        //잔상의 Sprite에 포지션값, Icon, FlipX, Color값을 지정하고 활성화 시킴
        transform.position = position;
        SpriteChange spriteChange = ManagerHub.Instance.ServerManager.DictRefToPlayer[playerRef].PlayerSpriteChange;
        SpriteChange.SetSpriteCopy(spriteChange);
        SpriteChange.SetFlipXCopy(flipX);
        SpriteChange.SetSpriteColorSilhouette(color);
        gameObject.SetActive(true);
        //특정 시간 이후 사라지게 만듦.
        StartCoroutine(ReturnPoolObject());
    }


    private IEnumerator ReturnPoolObject()
    {
        yield return waitSeconds;
        if(Runner.IsServer)
        Rpc_ReturnToPool();
    }
}
