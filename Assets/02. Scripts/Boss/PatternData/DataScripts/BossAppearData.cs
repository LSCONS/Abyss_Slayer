using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Pattern", menuName = "BossPattern/BossAppear")]
public class BossAppearData : BasePatternData
{
    [SerializeField] Vector3 appearPosition;
    [SerializeField] float preDelayTime;
    [SerializeField] float spawnAnimationTime;
    [SerializeField] float postDelayTime;
    [SerializeField] float zoomScale;
    public override IEnumerator ExecutePattern()
    {
        bossController.HitCollider.enabled = false;
        bossTransform.position = appearPosition;
        boss.IsLeft = true;
        

            SoundManager.Instance.PlaySFX(EAudioClip[0]);
        yield return new WaitForSeconds(preDelayTime);
        ServerManager.Instance.ThisPlayerData.Rpc_VirtualCamera(10 / zoomScale, 20);
        if (EAudioClip != null && EAudioClip.Count > 1)
            SoundManager.Instance.PlaySFX(EAudioClip[1]);
        yield return new WaitForSeconds(1f);
        boss.Rpc_SetSpriteEnable(true);
        boss.Rpc_SetTriggerAnimationHash(AnimationHash.AppearParameterHash);
        yield return new WaitForSeconds(spawnAnimationTime + 1f);
        bossController.HitCollider.enabled = true;
        ServerManager.Instance.ThisPlayerData.Rpc_VirtualCamera(10 / zoomScale, 5);
        yield return new WaitForSeconds(postDelayTime);
        ServerManager.Instance.ThisPlayerData.Rpc_VirtualCamera(10f, 5);
        ServerManager.Instance.ThisPlayerData.Rpc_ConnectInput();
    }

}
