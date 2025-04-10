using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Pattern", menuName = "BossPattern/ShockWave")]
public class ShockWaveData : BasePatternData
{
    [Header("개별 패턴 세부 정보")]
    [SerializeField] int damage;
    [SerializeField] float preDelayTime = 2f;
    [SerializeField] float stompTime = 0.2f;
    [SerializeField] int shockCount = 6;
    [SerializeField] float shockOffsetWidth = 1.0f;
    [SerializeField] float shockOffsetTime = 0.15f;
    [SerializeField] float postDelayTime = 3f;
    public override IEnumerator ExecutePattern(BossController bossController, Transform bossTransform, Animator animator)
    {
        animator.SetTrigger("ShockWave1");
        yield return new WaitForSeconds(preDelayTime);

        animator.SetTrigger("ShockWave2");
        yield return new WaitForSeconds(stompTime);

        Vector3 startPosition = bossTransform.position;

        PoolManager.Instance.Get<Explosion>().Init(startPosition, damage, 0.5f);
        bossController.StartCoroutine(ShockWave(startPosition));

        yield return new WaitForSeconds(postDelayTime);
    }
    IEnumerator ShockWave(Vector3 startPosition)
    {
        for (int i = 0; i < shockCount; i++)
        {
            PoolManager.Instance.Get<ShockWave>().Init(startPosition + (Vector3.right * shockOffsetWidth * (i + 1)), damage);
            PoolManager.Instance.Get<ShockWave>().Init(startPosition + (Vector3.left * shockOffsetWidth * (i + 1)), damage);

            yield return new WaitForSeconds(shockOffsetTime);
        }
    }
}
