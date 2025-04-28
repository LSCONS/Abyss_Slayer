using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Pattern", menuName = "BossPattern/FoxSphere")]
public class FoxSphereData : BasePatternData
{
    [SerializeField] int damage;
    [SerializeField] float preDelayTime = 1f;
    [SerializeField] int SphereCount = 3;
    [SerializeField] float startSpeed = 3;
    [SerializeField] float distance = 10;
    [SerializeField] float postDelayTime = 0.5f;
    public override IEnumerator ExecutePattern()
    {
        bossController.isLeft = target.position.x - bossTransform.position.x <= 0;
        bossController.showTargetCrosshair = true;
        bossAnimator.SetTrigger("Attack3");
        yield return new WaitForSeconds(0.25f);
        Vector3 startPosition = bossTransform.position + (Vector3.up * 0.5f) + (3 * (bossController.isLeft ? Vector3.left : Vector3.right));
        for(int i = 0; i < SphereCount; i++)
        {
            PoolManager.Instance.Get<FoxSphereProjectile>().Init(damage, startPosition, preDelayTime + (i * 0.2f), target, startSpeed, distance);
        }
        yield return new WaitForSeconds(preDelayTime + SphereCount * 0.2f + 0.5f);
        bossAnimator.SetTrigger("Idle");
        bossController.showTargetCrosshair = false;
        yield return new WaitForSeconds(postDelayTime);
    }
}
