using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Pattern", menuName = "BossPattern/LaserBox")]
public class LaserBoxData : BasePatternData
{
    [SerializeField] int damage;
    [SerializeField] bool isPiercing;
    [SerializeField] int fireCount = 3;
    [SerializeField] float preDelayTime = 1.5f;
    [SerializeField] Vector3 spawnPoint;
    [SerializeField] List<Vector3> firePositions = new List<Vector3>();
    [SerializeField] float spawnTime = 0.3f;
    [SerializeField] float moveTime = 1.2f;
    [SerializeField] float chasingTime = 1f;
    [SerializeField] float postDelayTime = 5f;
    [SerializeField] bool chasing;

    public override IEnumerator ExecutePattern()
    {
        bossController.isLeft = bossTransform.position.x > 0;
        bossAnimator.SetTrigger("Spawn");
        yield return new WaitForSeconds(preDelayTime);

        for (int i = 0; i < firePositions.Count; ++i)
        {
            Vector3 firePosition = firePositions[i] + (Vector3.up * Random.Range(-3, 3) + Vector3.right * Random.Range(-3, 3));
            PoolManager.Instance.Get<LaserBoxProjectile>().Init(damage, target, bossTransform.position, 1.5f, firePosition, moveTime, chasingTime, spawnTime, isPiercing, fireCount, chasing);
        }
        yield return new WaitForSeconds(spawnTime);
        bossAnimator.SetTrigger("Throw");
        yield return new WaitForSeconds(postDelayTime);
    }
}
