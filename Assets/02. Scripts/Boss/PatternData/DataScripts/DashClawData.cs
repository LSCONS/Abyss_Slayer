using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Pattern", menuName = "BossPattern/DashClaw")]
public class DashClawData : BasePatternData
{
    
    [SerializeField] int damage;
    [SerializeField] float distance = 15f;
    [SerializeField] float preDelayTime = 1f;
    [SerializeField] float attackDuration = 1f;
    [SerializeField] int comboAttackCount = 3;
    [SerializeField] float comboDelayTime = 0.5f;
    [SerializeField] float postDelayTime = 2f;
    public override IEnumerator ExecutePattern()
    {
        bossAnimator.SetTrigger("Dash1");


        bossController.showTargetCrosshair = true;

        bool isLeft = target.transform.position.x - bossTransform.position.x < 0;
        bossController.isLeft = isLeft;
        PoolManager.Instance.Get<DashClawEffect>().Init(damage, bossTransform.position + 2 * (isLeft? Vector3.right : Vector3.left), isLeft, distance, preDelayTime, attackDuration);
        bossController.chasingTarget = false;

        yield return new WaitForSeconds(preDelayTime);
        bossAnimator.enabled = false;
        bossController.sprite.enabled = false;
        bossController.hitCollider.enabled = false;

        yield return new WaitForSeconds(0.3f * attackDuration);
        float positionX = Mathf.Clamp(bossController.transform.position.x + (bossController.isLeft? -distance : distance), -mapWidth / 2 + 1, mapWidth / 2 - 1);
        bossTransform.position = new Vector3(positionX,bossTransform.position.y,0);
        bossController.sprite.enabled = true;
        bossController.hitCollider.enabled = true;

        int i = 1;
        while(i < comboAttackCount)
        {
            isLeft = target.transform.position.x - bossTransform.position.x < 0;
            PoolManager.Instance.Get<DashClawEffect>().Init(damage, bossTransform.position + 2 * (isLeft ? Vector3.right : Vector3.left), isLeft, distance, comboDelayTime, attackDuration);

            yield return new WaitForSeconds(comboDelayTime);
            bossController.sprite.enabled = false;
            bossController.hitCollider.enabled = false;
            bossController.isLeft = isLeft;

            yield return new WaitForSeconds(0.3f * attackDuration);
            positionX = Mathf.Clamp(bossController.transform.position.x + (bossController.isLeft ? -distance : distance), -mapWidth / 2 + 1, mapWidth / 2 - 1);
            bossTransform.position = new Vector3(positionX, bossTransform.position.y, 0);
            bossController.sprite.enabled = true;
            bossController.hitCollider.enabled = true;
            i++;
        }
        bossAnimator.enabled = true;
        bossAnimator.SetTrigger("Dash3");
        bossController.showTargetCrosshair = false;
        float time = Time.time + (postDelayTime/2);
        Vector3 targetposition = bossTransform.position + 2f * (isLeft ? Vector3.left : Vector3.right);
        while(time >= Time.time)
        {
            bossTransform.position = Vector3.Lerp(bossTransform.position, targetposition, 2 * 0.9f * Time.deltaTime);
            yield return null;
        }
        yield return bossController.StartCoroutine(bossController.Landing());
        yield return new WaitForSeconds(postDelayTime/2);
    }
}
