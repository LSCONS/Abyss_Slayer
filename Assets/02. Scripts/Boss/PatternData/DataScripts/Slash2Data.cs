using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Pattern", menuName = "BossPattern/Slash2")]
public class Slash2Data : BasePatternData
{
    [SerializeField] int damage;
    [SerializeField] List<float> attackAngles;
    [SerializeField] float attackIntervalTime;
    [SerializeField] float attackSpeed = 1;
    [SerializeField] float attackDistance = 11;
    [SerializeField] float attackStartDistance = 4;
    [SerializeField] float attackCount;

    [SerializeField] float jumpHight;
    [SerializeField] float jumpTime;

    [SerializeField] float preDelayTime;
    [SerializeField] float attackDelayTime;
    [SerializeField] float comboAttackIntervalTime;
    [SerializeField] float postDelayTime;

    
    public override IEnumerator ExecutePattern()
    {
        bool isLeft = 0 > (target.position.x - bossTransform.position.x);
        boss.IsLeft = isLeft;
        bossController.ShowTargetCrosshair = true;

        if(Mathf.Abs(target.position.x - bossTransform.position.x) > attackStartDistance)
        {
            boss.Rpc_SetTriggerAnimationHash(AnimationHash.ReadyRunParameterHash);
            yield return new WaitForSeconds(preDelayTime);

            bossController.StartCoroutine(bossController.RunMove(isLeft));
            while (Mathf.Abs(target.position.x - bossTransform.position.x) > attackStartDistance)
            {
                yield return null;
            }
            bossController.IsRun = false;
        }
        else if(target.position.y - bossTransform.position.y > attackStartDistance)
        {
            boss.Rpc_SetTriggerAnimationHash(AnimationHash.ReadyRunParameterHash);
            yield return new WaitForSeconds(preDelayTime);
        }
        
        if(target.position.y - bossTransform.position.y > attackStartDistance)
        {
            yield return bossController.StartCoroutine(JumpSlash());
        }
        else 
        {
            if (target.position.y - bossTransform.position.y < -attackStartDistance)
            {
                PhysicsScene2D scene2D = RunnerManager.Instance.GetRunner().GetPhysicsScene2D();
                float posY2 = scene2D.Raycast(target.position, Vector3.down, 40, (LayerData.GroundPlaneLayerMask | LayerData.GroundPlatformLayerMask)).point.y + bossCenterHight;
                posY2 = Mathf.Min(posY2, scene2D.Raycast(bossTransform.position + Vector3.down * (bossCenterHight + 0.5f), Vector3.down, 40, (LayerData.GroundPlaneLayerMask | LayerData.GroundPlatformLayerMask)).point.y + bossCenterHight);
                yield return bossController.StartCoroutine(bossController.JumpMove(new Vector3(bossTransform.position.x, posY2), jumpTime, jumpHight));
            }

            boss.Rpc_SetTriggerAnimationHash(AnimationHash.SlashReadyParameterHash);
            yield return new WaitForSeconds(preDelayTime);

            isLeft = target.position.x - bossTransform.position.x < 0;
            boss.IsLeft = isLeft;

            float posX = bossTransform.position.x;
            float posY = bossTransform.position.y;
            float degree = Mathf.Atan2(Mathf.Max(0, target.position.y - posY), Mathf.Abs(target.position.x - posX)) * Mathf.Rad2Deg;
            for (int i = 0; i < attackAngles.Count; i++)
            {
                if (EAudioClip != null && EAudioClip.Count > 1)
                    SoundManager.Instance.PlaySFX(EAudioClip[1]);

                PoolManager.Instance.Get<NormalSlash>().Rpc_Init(new Vector3(posX, posY), damage, isLeft, degree + attackAngles[i], attackSpeed);
                bossController.StartCoroutine(AttackEffect());
                yield return new WaitForSeconds(attackIntervalTime);
            }
        }
        
        for (int i = 1; i < attackCount; i++)
        {
            yield return new WaitForSeconds(comboAttackIntervalTime);
            yield return bossController.StartCoroutine(JumpSlash());
        }
        bossController.ShowTargetCrosshair = false;
        yield return bossController.StartCoroutine(bossController.Landing());
        boss.Rpc_SetTriggerAnimationHash(AnimationHash.IdleParameterHash);
        yield return new WaitForSeconds(postDelayTime);
        boss.Rpc_SetTriggerAnimationHash(AnimationHash.IdleParameterHash);
    }

    IEnumerator JumpSlash()
    {
        bool isLeft = 0 > (target.position.x - bossTransform.position.x);
        boss.IsLeft = isLeft;

        PhysicsScene2D scene2D = RunnerManager.Instance.GetRunner().GetPhysicsScene2D();
        float distance = Mathf.Min(Mathf.Abs(target.position.x - bossTransform.position.x * 0.9f), attackDistance * 0.8f);
        float posX = target.position.x + (isLeft ? distance : -distance);
        float posY = scene2D.Raycast(target.position, Vector3.down, 40, (LayerData.GroundPlaneLayerMask | LayerData.GroundPlatformLayerMask)).point.y + bossCenterHight + 0.5f;
        posY = Mathf.Max(target.position.y,posY);
        boss.Rpc_SetBoolAnimationHash(AnimationHash.AttackJumpParameterHash, true);
        bossController.StartCoroutine(bossController.JumpMove(new Vector3(posX, posY), jumpTime, (posY <= bossTransform.position.y)? 1f : 0));

        yield return new WaitForSeconds(jumpTime - (0.6f * 1 / attackSpeed) + attackDelayTime);

        isLeft = target.position.x - posX < 0;
        boss.IsLeft = isLeft;

        float degree = Mathf.Atan2(Mathf.Max(0, target.position.y - posY), Mathf.Abs(target.position.x - posX)) * Mathf.Rad2Deg;
        for (int i = 0; i < attackAngles.Count; i++)
        {
            if (EAudioClip != null && EAudioClip.Count > 1)
                SoundManager.Instance.PlaySFX(EAudioClip[1]);

            PoolManager.Instance.Get<NormalSlash>().Rpc_Init(new Vector3(posX, posY), damage, isLeft, degree + attackAngles[i], attackSpeed);
            bossController.StartCoroutine(AttackEffect());
            yield return new WaitForSeconds(attackIntervalTime);
        }
        boss.Rpc_SetBoolAnimationHash(AnimationHash.AttackJumpParameterHash, false);
        yield return new WaitForSeconds(0.7f * 1 / attackSpeed + 0.1f);
        

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
