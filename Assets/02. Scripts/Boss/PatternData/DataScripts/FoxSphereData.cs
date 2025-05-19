using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Pattern", menuName = "BossPattern/FoxSphere")]
public class FoxSphereData : BasePatternData
{
    enum Color
    {
        blue,
        gray
    }

    [SerializeField] int damage;
    [SerializeField] float preDelayTime = 1f;
    [SerializeField] int sphereCount = 3;
    [SerializeField] float fireIntervalTime = 0.3f;
    [SerializeField] float startSpeed = 3;
    [SerializeField] float distance = 10;
    [SerializeField] float postDelayTime = 0.5f;
    [SerializeField] Color color;
    public override IEnumerator ExecutePattern()
    {
        bossController.isLeft = target.position.x - bossTransform.position.x <= 0;
        bossController.showTargetCrosshair = true;
        bossAnimator.SetTrigger("Attack3");
        yield return new WaitForSeconds(0.25f);
        Vector3 startPosition = bossTransform.position + (Vector3.up * 0.5f) + (3 * (bossController.isLeft ? Vector3.left : Vector3.right));
        for(int i = 0; i < sphereCount; i++)
        {
            PoolManager.Instance.Get<FoxSphereProjectile>().Init(damage, startPosition, preDelayTime + (i * fireIntervalTime), target, startSpeed, distance, (int)color);
        }
        yield return new WaitForSeconds(preDelayTime + sphereCount * fireIntervalTime + 0.5f);
        bossAnimator.SetTrigger("Idle");
        bossController.showTargetCrosshair = false;
        yield return new WaitForSeconds(postDelayTime);
    }
}
