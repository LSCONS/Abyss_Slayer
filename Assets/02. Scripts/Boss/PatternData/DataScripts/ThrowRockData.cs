using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Pattern", menuName = "BossPattern/ThrowRock")]
public class ThrowRockData : BasePatternData
{
    [SerializeField] int damage;
    [SerializeField] float preDelayTime = 2f;
    [SerializeField] float rockSpeed = 10f;
    [SerializeField] float rockSize = 1f;
    [SerializeField] float postDelayTime = 1f;
    public override IEnumerator ExecutePattern()
    {
        bossAnimator.SetTrigger("ThrowRock1");
        bossController.chasingTarget = true;
        bossController.showTargetCrosshair = true;
        yield return new WaitForSeconds(preDelayTime);
        bossAnimator.SetTrigger("ThrowRock2");
        PoolManager.Instance.Get<GravityProjectile>().Init(damage, bossTransform.position + Vector3.up * 2, rockSpeed, target.position, int.MaxValue,rockSize, 1);
        bossController.chasingTarget = false;
        bossController.showTargetCrosshair = false;
        yield return new WaitForSeconds(postDelayTime);
        bossAnimator.SetTrigger("ThrowRock3");
    }
}
