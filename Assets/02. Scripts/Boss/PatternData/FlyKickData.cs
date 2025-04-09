using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Pattern", menuName = "BossPattern/FlyKick")]
public class FlyKickData : BasePatternData
{
    [SerializeField] int damage;
    [SerializeField] float flyupHight = 5f;
    [SerializeField] float flyupTime = 1f;
    [SerializeField] float flyingTime = 0.5f;
    [SerializeField] float kickSpeed = 100f;
    [SerializeField] float explosionSize = 1f;
    [SerializeField] float postDelayTime = 1f;
    public override IEnumerator ExecutePattern(Transform bossTransform, Animator animator)
    {
        animator.SetTrigger("kick1");       //날아오르는 애니메이션 재생
        float elapsed = 0f;
        while (elapsed < flyupTime)         //일정시간동안,일정높이로 날아오름
        {
            Vector3 flyPosition = bossTransform.position + Vector3.up * flyupHight;
            bossTransform.position = Vector3.MoveTowards(bossTransform.position, flyPosition, flyupHight / flyupTime * Time.deltaTime);

            elapsed += Time.deltaTime;
            yield return null;
        }
        yield return new WaitForSeconds(flyingTime);    //공중에서 일정시간 대기
        
        Vector3 targetPosition = target.position;       //찍기직전 타겟 추적중지, 찍을장소 고정
        while (true)
        {
            bossTransform.position = Vector3.MoveTowards(bossTransform.position, targetPosition, kickSpeed * Time.deltaTime);

            if (Vector3.Distance(bossTransform.position, targetPosition) < 0.01f)               // 목표 지점 도달 여부 체크
            {
                PoolManager.Instance.Get<Explosion>().Init(targetPosition + (Vector3.down * 1f), damage, explosionSize);

                bossTransform.position = targetPosition; // 위치 보정
                break;
            }
            yield return null;
        }
        yield return new WaitForSeconds(postDelayTime);
    }
}
