using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Pattern", menuName = "BossPattern/LaserBox")]
public class LaserBoxData : BasePatternData
{
    [SerializeField] int damage;
    [SerializeField] float preDelayTime = 1.5f;
    [SerializeField] List<Vector3> firePosition = new List<Vector3>();
    [SerializeField] float moveTime = 1.2f;
    [SerializeField] float chasingTime = 1f;
    [SerializeField] float delayTime = 0.3f;
    [SerializeField] float postDelayTime = 5f;

    public override IEnumerator ExecutePattern()
    {
        bossAnimator.SetTrigger("LaserBox1");
        yield return new WaitForSeconds(preDelayTime);

        for (int i = 0; i < firePosition.Count; ++i)
        {
            PoolManager.Instance.Get<LaserBoxProjectile>().Init(damage, target, bossTransform.position + Vector3.up * 2, 1.5f, firePosition[i], moveTime, chasingTime, delayTime, 3);
        }
        
        yield return new WaitForSeconds(postDelayTime);
    }
}
