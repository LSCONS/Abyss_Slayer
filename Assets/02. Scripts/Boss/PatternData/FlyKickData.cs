using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Pattern", menuName = "BossPattern/FlyKick")]
public class FlyKickData : BasePatternData
{
    [SerializeField] float flyupHight = 5f;
    [SerializeField] float flyupTime = 1f;
    [SerializeField] float flyingTime = 0.5f;
    [SerializeField] float kickTime = 0.3f;
    [SerializeField] float explosionSize = 1f;
    public override IEnumerator ExecutePattern(Transform bossTransform, Animator animator)
    {
        animator.SetTrigger("kick1");       
        float elapsed = 0f;
        while (elapsed < flyupTime)
        {
            Vector3 flyPosition = bossTransform.position + Vector3.up * flyupHight;
            bossTransform.position = Vector3.MoveTowards(bossTransform.position, flyPosition, flyupHight / flyupTime * Time.deltaTime);

            elapsed += Time.deltaTime;
            yield return null;
        }
        yield return new WaitForSeconds(flyingTime);
        
        elapsed = 0f;
        Vector3 targetPosition = target.position;
        float kickSpeed = Vector3.Distance(boss.transform.position, targetPosition)/kickTime;
        while (true)
        {
            bossTransform.position = Vector3.MoveTowards(bossTransform.position, targetPosition, kickSpeed * Time.deltaTime);

            // 목표 지점 도달 여부 체크
            if (Vector3.Distance(bossTransform.position, targetPosition) < 0.01f)
            {
                PoolManager.Instance.explosionPool.Get(targetPosition, explosionSize);
                bossTransform.position = targetPosition; // 위치 보정
                break;
            }

            yield return null;
        }


    }
}
