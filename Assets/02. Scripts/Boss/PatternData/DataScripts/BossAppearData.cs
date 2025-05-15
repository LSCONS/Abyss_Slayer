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
        bossController.hitCollider.enabled = false;
        bossTransform.position = appearPosition;
        bossController.isLeft = true;
        yield return new WaitForSeconds(preDelayTime); 
        
        bossController.virtualCamera.m_Lens.OrthographicSize = 10/zoomScale;
        bossController.virtualCamera.Priority = 20;
        yield return new WaitForSeconds(1f);

        bossAnimator.SetTrigger("Appear");
        yield return new WaitForSeconds(spawnAnimationTime + 1f);
        bossController.hitCollider.enabled = true;
        bossController.virtualCamera.Priority = 5;
        yield return new WaitForSeconds(postDelayTime);
        bossController.virtualCamera.m_Lens.OrthographicSize = 10f;
        PlayerManager.Instance.PlayerOnConnected();
    }

}
