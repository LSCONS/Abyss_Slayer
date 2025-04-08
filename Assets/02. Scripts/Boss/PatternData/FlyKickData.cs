using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Pattern", menuName = "BossPattern/FlyKick")]
public class FlyKickData : BasePatternData
{
    [SerializeField] float flyupHight = 5f;
    [SerializeField] float flyupTime = 1f;
    [SerializeField] float flyingTime = 0.5f;
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




    }
}
