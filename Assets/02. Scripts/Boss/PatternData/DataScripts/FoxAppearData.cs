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
        bossController.isLeft = true;
        bossAnimator.SetTrigger("Appear");
        yield return new WaitForSeconds(1f); 
        
        bossController.virtualCamera.m_Lens.OrthographicSize = 10/zoomScale;
        bossController.virtualCamera.Priority = 20;
        yield return new WaitForSeconds(preDelayTime);

        
        yield return new WaitForSeconds(4f);
        bossController.virtualCamera.Priority = 5;

        yield return new WaitForSeconds(postDelayTime);
        bossController.virtualCamera.m_Lens.OrthographicSize = 10f;
        PlayerManager.Instance.PlayerOnConnected();
    }

}
