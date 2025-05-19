using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Pattern", menuName = "BossPattern/CrossSlash2")]
public class CrossSlash2Data : BasePatternData
{
    [SerializeField] int damage;
    [SerializeField] float speed = 1;
    [SerializeField] float attackDistance;
    [SerializeField] float preDelayTime;
    public override IEnumerator ExecutePattern()
    {
        bool isleft = 0 > (target.position.x - bossTransform.position.x);
        bossController.isLeft = isleft;
        bossController.showTargetCrosshair = true;
        bossAnimator.SetTrigger("ReadyRun");
        yield return new WaitForSeconds(preDelayTime);

        bossController.StartCoroutine(bossController.RunMove(isleft));

        while((Mathf.Abs(target.position.x - bossTransform.position.x) > attackDistance))
        {
            yield return null;
        }
        bossController.showTargetCrosshair = false;
        bossAnimator.SetTrigger("RunSlash");
        yield return new WaitForSeconds(0.1f * speed);

        bossController.isRun = false;
        yield return new WaitForSeconds(0.05f * speed);

        PoolManager.Instance.Get<CrossSlash>().Init(bossTransform.position + 7 * (isleft ? Vector3.left : Vector3.right), isleft, damage, 2, speed);
        yield return new WaitForSeconds((1/ 6) * speed);

        float x = Mathf.Clamp(bossTransform.position.x + (isleft ? -14 : 14), -mapWidth / 2 + 0.7f, mapWidth / 2 - 0.7f);
        bossTransform.position = new Vector3(x, bossTransform.position.y);

        if (Physics2D.Raycast(bossTransform.position, Vector3.down, bossCenterHight + 0.1f, LayerMask.GetMask("GroundPlane", "GroundPlatform")))
        {
            bossAnimator.SetTrigger("SlashEnd");
            yield return new WaitForSeconds(1f);
            bossAnimator.ResetTrigger("SlashEnd");
        }
        

    }
}
