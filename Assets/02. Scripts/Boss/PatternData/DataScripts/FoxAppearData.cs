using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Pattern", menuName = "BossPattern/FoxAppear")]
public class FoxAppearData : BasePatternData
{
    [SerializeField] Vector3 appearPosition;
    [SerializeField] float preDelayTime;
    [SerializeField] float postDelayTime;
    [SerializeField] float zoomScale;
    public override IEnumerator ExecutePattern()
    {
        bossTransform.position = appearPosition;
        bossController.IsLeft = true;
        boss.Rpc_SetAnimationHash(BossAnimationHash.AppearParameterHash);
        yield return new WaitForSeconds(1f); 
        
        bossController.VirtualCamera.m_Lens.OrthographicSize = 10/zoomScale;
        bossController.VirtualCamera.Priority = 20;
        yield return new WaitForSeconds(preDelayTime);

        
        yield return new WaitForSeconds(4f);
        bossController.VirtualCamera.Priority = 5;

        yield return new WaitForSeconds(postDelayTime);
        bossController.VirtualCamera.m_Lens.OrthographicSize = 10f;
        ServerManager.Instance.ThisPlayerData.Rpc_ConnectInput();
    }

}
