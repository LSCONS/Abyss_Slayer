using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Pattern", menuName = "BossPattern/SquareLaser")]
public class SquareLaserData : BasePatternData
{
    [SerializeField] int damage;
    [SerializeField] float damageIntervalTime;
    [SerializeField] int projectileCount;
    [SerializeField] float projectileGap = 2;

    [SerializeField] float delayFireTime;
    [SerializeField] float intervalFireTime;
    [SerializeField] float effectSpeed = 1;
    [SerializeField] float postDelayTime;
    public override IEnumerator ExecutePattern()
    {
        bossController.isLeft = target.position.x - bossTransform.position.x < 0f;
        bossAnimator.SetTrigger("Spawn");
        yield return new WaitForSeconds(0.3f);

        Vector3 targetPos;
        for (int i = 0; i < projectileCount; i++)
        {
            targetPos = new Vector3(target.position.x + (projectileGap / 2) * (-projectileCount + 1 + (i * 2)) * (bossController.isLeft ? -1f : 1f), 0);
            PoolManager.Instance.Get<SquareLaserPorjectile>().Init(damage, damageIntervalTime, bossTransform.position, targetPos, delayFireTime, 0.1f + i * intervalFireTime, effectSpeed);
        }
        yield return new WaitForSeconds(delayFireTime);

        bossAnimator.SetTrigger("Throw");
        yield return new WaitForSeconds(postDelayTime);
    }
}
