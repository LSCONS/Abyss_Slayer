using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Pattern", menuName = "BossPattern/Slash")]
public class SlashData : BasePatternData
{
    [SerializeField] int damage;
    [SerializeField] float attackSpeed;
    [SerializeField] List<float> attackAngles;
    [SerializeField] float intervalTime;

    [SerializeField] float preDelayTime;
    [SerializeField] float postDelayTime;
    public override IEnumerator ExecutePattern()
    {
        bossController.isLeft = target.position.x - bossTransform.position.x < 0;
        bossAnimator.SetTrigger("SlashReady");
        yield return new WaitForSeconds(preDelayTime);

        for(int i = 0; i < attackAngles.Count; i++)
        {
            PoolManager.Instance.Get<NormalSlash>().Init(bossTransform.position, damage, bossController.isLeft, attackAngles[i],attackSpeed);
            bossController.StartCoroutine(AttackEffect(i == attackAngles.Count - 1));
            yield return new WaitForSeconds(intervalTime);
        }
        yield return new WaitForSeconds(2f + postDelayTime);
    }
    IEnumerator AttackEffect(bool lastAttack)
    {
        yield return new WaitForSeconds(0.6f * 1/attackSpeed);
        bossController.sprite.enabled = false;
        PoolManager.Instance.Get<JumpEffect>().Init(bossTransform.position + Vector3.down * bossCenterHight);
        yield return new WaitForSeconds(0.1f);
        if (lastAttack)
        {
            bossAnimator.SetTrigger("SlashEnd");
        }
        bossController.sprite.enabled = true;
    }
}
