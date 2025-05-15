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
        yield return new WaitForSeconds(preDelayTime); 
        
        bossController.VirtualCamera.m_Lens.OrthographicSize = 10/zoomScale;
        bossController.VirtualCamera.Priority = 20;
        yield return new WaitForSeconds(1f);

        boss.Rpc_SetTriggerAnimationHash(BossAnimationHash.AppearParameterHash);
        yield return new WaitForSeconds(spawnAnimationTime + 1f);
        bossController.HitCollider.enabled = true;
        bossController.VirtualCamera.Priority = 5;
        yield return new WaitForSeconds(postDelayTime);
        bossController.VirtualCamera.m_Lens.OrthographicSize = 10f;
        ServerManager.Instance.ThisPlayerData.Rpc_ConnectInput();
    }

}
