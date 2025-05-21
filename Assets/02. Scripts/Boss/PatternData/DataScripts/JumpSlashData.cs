using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Pattern", menuName = "BossPattern/JumpSlash")]
public class JumpSlashData : BasePatternData
{
    [SerializeField] int damage;
    [SerializeField] List<float> attackAngles = new List<float>();
    [SerializeField] float attackIntervalTime;
    [SerializeField] float attackSpeed;
    [SerializeField] float preDelayTime;
    [SerializeField] float jumpStartDistance = 2;
    [SerializeField] float jumpHight;
    [SerializeField] float jumpSpeedTime = 1;
    [SerializeField] float attackDelayTime = 1;
    [SerializeField] float postDelayTime;

    [SerializeField] BasePatternData normalSlashData;
    public override IEnumerator ExecutePattern()
    {
        float targetY = Physics2D.Raycast(target.position, Vector3.down,20, LayerData.GroundPlaneLayerMask | LayerData.GroundPlatformLayerMask).point.y;
        if(targetY < bossTransform.position.y)
        {
            yield return new WaitForSeconds(1f);
            yield break;
        }

        bool isLeft = target.position.x - bossTransform.position.x < 0;
        boss.IsLeft = isLeft;
        bossController.ShowTargetCrosshair = true;
        yield return new WaitForSeconds(preDelayTime);

        bossController.StartCoroutine(bossController.RunMove(isLeft));
        yield return null;

        while (Mathf.Abs(target.position.x - bossTransform.position.x) > jumpStartDistance)
        {
            yield return null;
        }
        bossController.IsRun = false;
        targetY = Physics2D.Raycast(target.position, Vector3.down, 20, LayerData.GroundPlaneLayerMask | LayerData.GroundPlatformLayerMask).point.y;
        if (targetY < bossTransform.position.y)
        {
            normalSlashData.Init(bossController);
            normalSlashData.target = target;
            yield return bossController.StartCoroutine(normalSlashData.ExecutePattern());
            yield break;
        }
        float targetX = target.position.x + (boss.IsLeft ? 1 : -1);
        targetY += bossCenterHight + jumpHight;
        Vector3 targetPos = new Vector3(targetX, targetY);
        bossAnimator.SetBool("AttackJump", true);
        bossController.StartCoroutine(bossController.JumpMove(targetPos,jumpSpeedTime, 0));

        yield return new WaitForSeconds(jumpSpeedTime - (0.6f * 1 / attackSpeed) +  attackDelayTime);
        bossController.ShowTargetCrosshair = false;

        isLeft = target.position.x - targetX < 0;
        boss.IsLeft = isLeft;

        float degree = Mathf.Atan2(Mathf.Max(0, target.position.y - targetY), Mathf.Abs(target.position.x - targetX)) * Mathf.Rad2Deg;
        for (int i = 0; i < attackAngles.Count; i++)
        {
            PoolManager.Instance.Get<NormalSlash>().Rpc_Init(new Vector3(targetX, targetY), damage, isLeft, degree + attackAngles[i], attackSpeed);
            bossController.StartCoroutine(AttackEffect());
            yield return new WaitForSeconds(attackIntervalTime);
        }
        bossAnimator.SetBool("AttackJump", false);
        yield return new WaitForSeconds(0.7f * 1 / attackSpeed + 0.1f);
        yield return bossController.StartCoroutine(bossController.Landing());
        yield return new WaitForSeconds(postDelayTime);
    }

    IEnumerator AttackEffect()
    {
        yield return new WaitForSeconds(0.6f * 1 / attackSpeed);
        bossController.Sprite.enabled = false;
        PoolManager.Instance.Get<JumpEffect>().Rpc_Init(bossTransform.position + Vector3.down * bossCenterHight);
        yield return new WaitForSeconds(0.1f * 1 / attackSpeed);
        bossController.Sprite.enabled = true;
    }
}
